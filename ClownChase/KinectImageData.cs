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
            public int MinY;
            public int MaxY;
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



            return position;
        }

        private Position Clip(Position position, Func<int, int, bool> showPixel)
        {
            position.MinX = GetXEdge(0-Settings.Default.clipXDelta, position, showPixel);
            position.MaxX = GetXEdge(Settings.Default.clipXDelta, position, showPixel);
            position.MinY = GetYEdge(0 - Settings.Default.clipYDelta, position, showPixel);
            position.MaxY = GetYEdge(Settings.Default.clipYDelta, position, showPixel);
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
            
            return Math.Max(0, Math.Min(x, Boundaries.DepthWidth));
        }

        private int GetYEdge(int yDelta, Position position, Func<int, int, bool> showPixel)
        {
            var miss = 0;
            var y = position.NearestY;

            do
            {
                if (!PixelAtY(y, position, showPixel))
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

                y += yDelta;
            } while (y > 0 && y < Boundaries.DepthHeight);

            return Math.Max(0, Math.Min(y, Boundaries.DepthHeight));
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

        private bool PixelAtY(int y, Position position, Func<int, int, bool> showPixel)
        {
            bool found = false;
            for (var x = Boundaries.DepthMinX; x < Boundaries.DepthMaxX; x += Settings.Default.clipXDelta)
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
            nearestObject = Clip(nearestObject, showPixel);

            if (nearestObject.NearestX == 0)
            {
                return nearestObject;                
            }

            for (var y = nearestObject.MinY; y < nearestObject.MaxY; ++y)
            {
                var depthOffset = y * Boundaries.DepthWidth;

                for (var x=nearestObject.MinX; x<nearestObject.MaxX; ++x)
                {
                    var depthIndex = x + depthOffset;
                    Debug.Assert(depthIndex == x + depthOffset);

                    var pixelDistance = DepthPixels[depthIndex].Depth;

                    var pixelContainsObject = showPixel(nearestObject.Depth, pixelDistance) && (pixelDistance != 0);

                    var colorImagePoint = colorCoordinates[depthIndex];
                    var colorInDepthX = colorImagePoint.X / Boundaries.ColorToDepthDivisor;
                    var colorInDepthY = colorImagePoint.Y / Boundaries.ColorToDepthDivisor;

                    mask.Set(colorInDepthX, colorInDepthY, pixelContainsObject);
                }
            }

            return nearestObject;
        }
    }
}
