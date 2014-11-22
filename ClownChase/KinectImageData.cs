using System;
using System.Diagnostics;
using System.Dynamic;
using ClownChase.Properties;
using Microsoft.Kinect;

namespace ClownChase
{
    public class KinectImageData
    {
        public const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;
        public const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;

        public DepthImagePixel[] DepthPixels;
        public byte[] ColorPixels;
        public Boundaries Boundaries;

        public KinectImageData(Boundaries boundaries)
        {
            Boundaries = boundaries;
            DepthPixels = new DepthImagePixel[Boundaries.DepthDataLength];
            ColorPixels = new byte[Boundaries.ColorDataLength];
        }

        public class Position
        {
            public int Depth;
            public int NearestX;
            public int NearestY;
            public int MinX;
            public int MaxX;
        }

        private Position NearestObject()
        {
            var position = new Position {Depth = 10000};
            for (var y = Boundaries.DepthMinY; y < Boundaries.DepthMaxY; y += Settings.Default.objectYDelta)
            {
                for (var x = Boundaries.DepthMinX; x < Boundaries.DepthMaxX; x += Settings.Default.objectXDelta)
                {
                    var i = (y*Boundaries.DepthWidth) + x;
                    var d = DepthPixels[i].Depth;
                    if (d > 0 && d < position.Depth)
                    {
                        position.Depth = d;
                        position.NearestX = x;
                        position.NearestY = y;
                    }
                }
            }

            position.MinX = 0;
            position.MaxX = Boundaries.DepthWidth;

            return position;
        }

        private Position ClipX(Position position, Func<int, int, bool> showPixel)
        {
            position.MinX = GetXEdge(0-Settings.Default.clipXDelta, position, showPixel);
            position.MaxX = GetXEdge(Settings.Default.clipXDelta, position, showPixel);
            return position;
        }

        private int GetXEdge(int xDelta, Position position, Func<int, int, bool> showPixel)
        {
            var miss = 0;
            var x = position.NearestX;

            do
            {
                if (!PixelAtX(x, position, showPixel))
                {
                    if (++miss == Settings.Default.maxMiss)
                    {
                        break;
                    }
                }
                else
                {
                    miss = 0;
                }

                x += xDelta;
            } while (x > 0 && x < Boundaries.DepthWidth);
            
            return x;
        }

        private bool PixelAtX(int x, Position position, Func<int, int, bool> showPixel)
        {
            bool found = false;
            for (var y = Boundaries.DepthMinY; y < Boundaries.DepthMaxY; y += Settings.Default.clipYDelta)
            {
                var i = (y * Boundaries.DepthWidth) + x;
                var d = DepthPixels[i].Depth;
                if (showPixel(position.Depth, d) && (d != 0))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public Position PopulateColorPixelMask(ColorPixelMask mask, Mapper mapper, Func<int, int, bool> showPixel)
        {
            var colorCoordinates = mapper.Map(this);
            mask.Clear();

            var nearestObject = NearestObject();
            nearestObject = ClipX(nearestObject, showPixel);

            if (nearestObject.NearestX == 0)
            {
                return nearestObject;                
            }

            var depthOffset = 0;
            for (var y = 0; y < Boundaries.DepthHeight; ++y)
            {
                Debug.Assert(depthOffset == (y * Boundaries.DepthWidth));

                for (var x=nearestObject.MinX; x<nearestObject.MaxX; ++x)
                {
                    var depthIndex = x + depthOffset;
                    Debug.Assert(depthIndex == x + depthOffset);

                    var pixelDistance = DepthPixels[depthIndex].Depth;

                    var pixelContainsObject = showPixel(nearestObject.Depth, pixelDistance) && (pixelDistance != 0);

                    if (pixelContainsObject)
                    {
                        var colorImagePoint = colorCoordinates[depthIndex];
                        var colorInDepthX = colorImagePoint.X / Boundaries.ColorToDepthDivisor;
                        var colorInDepthY = colorImagePoint.Y / Boundaries.ColorToDepthDivisor;

                        mask.Set(colorInDepthX, colorInDepthY);
                    }
                }

                depthOffset += Boundaries.DepthWidth;
            }

            return nearestObject;
        }
    }
}
