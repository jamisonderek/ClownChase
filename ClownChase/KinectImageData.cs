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
        public Position NearestObject()
        {
            return NearestObject(Boundaries.DepthMinY, Boundaries.DepthMaxY, Settings.Default.objectYDelta,
                Boundaries.DepthMinX, Boundaries.DepthMaxX, Settings.Default.objectXDelta);
        }

        public Position NearestObject(int minY, int maxY, int deltaY, int minX, int maxX, int deltaX)
        {
            var minDepth = int.MaxValue;
            var position = new Position();

            for (var y = minY; y < maxY; y += deltaY)
            {
                var offset = (y * Boundaries.DepthWidth);
                for (var x = minX; x < maxX; x += deltaX)
                {
                    var d = DepthPixels[offset + x].Depth;
                    if (d < minDepth && d > 0)
                    {
                        minDepth = d;
                        position.X = x;
                        position.Y = y;
                    }
                }
            }
            position.Depth = minDepth;

            return position;
        }

    }
}
