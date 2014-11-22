using System;
using System.Diagnostics;
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

            for (var y = 0; y < Boundaries.DepthHeight; y += 20)
            {
                for (var x = Boundaries.DepthMinX; x < Boundaries.DepthMaxX; x++)
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
            int maxMiss = 4;

            var miss = 0;
            for (position.MinX = position.NearestX; position.MinX >= 0; position.MinX--)
            {
                bool found = false;
                for (var y = 0; y < Boundaries.DepthHeight; y += 10)
                {
                    var i = (y * Boundaries.DepthWidth) + position.MinX;
                    var d = DepthPixels[i].Depth;
                    if (showPixel(position.Depth, d) && (d != 0))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    miss = 0;
                }
                else if (++miss==maxMiss)
                {
                    break;
                }
            }

            miss = 0;
            for (position.MaxX = position.NearestX; position.MaxX < Boundaries.DepthWidth; position.MaxX++)
            {
                bool found = false;
                for (var y = 0; y < Boundaries.DepthHeight; y += 10)
                {
                    var i = (y * Boundaries.DepthWidth) + position.MaxX;
                    var d = DepthPixels[i].Depth;
                    if (showPixel(position.Depth, d) && (d != 0))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    miss = 0;
                }
                else if (++miss==maxMiss)
                {
                    break;
                }
            }

            return position;
        }

        public Position PopulateColorPixelMask(ColorPixelMask mask, Mapper mapper, Func<int, int, bool> showPixel)
        {
            var colorCoordinates = mapper.Map(this);
            mask.Clear();

            var nearestObject = NearestObject();
            nearestObject = ClipX(nearestObject, showPixel);

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
