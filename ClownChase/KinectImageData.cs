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
    }
}
