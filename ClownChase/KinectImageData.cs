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

        public int PopulateColorPixelMask(ColorPixelMask mask, Mapper mapper, Func<int, int, bool> showPixel)
        {
            int nearestObject = 10000;

            var colorCoordinates = mapper.Map(this);

            mask.Clear();

            for (var y = 0; y < Boundaries.DepthHeight; y += 20)
            {
                for (var x = Boundaries.DepthMinX; x < Boundaries.DepthMaxX; x++)
                {
                    var i = (y * Boundaries.DepthWidth) + x;
                    var d = DepthPixels[i].Depth;
                    if (d > 0 && d < nearestObject)
                    {
                        nearestObject = d;
                    }
                }
            }

            var depthOffset = 0;
            for (var y = 0; y < Boundaries.DepthHeight; ++y)
            {
                Debug.Assert(depthOffset == (y * Boundaries.DepthWidth));

                for (var x = 0; x < Boundaries.DepthWidth; x++)
                {
                    var depthIndex = x + depthOffset;
                    Debug.Assert(depthIndex == x + depthOffset);

                    var pixelDistance = DepthPixels[depthIndex].Depth;

                    var pixelContainsObject = showPixel(nearestObject, pixelDistance) && (pixelDistance != 0);

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
