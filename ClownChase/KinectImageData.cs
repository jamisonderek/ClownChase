using Microsoft.Kinect;

namespace ClownChase
{
    public class KinectImageData
    {
        public const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;
        public const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;

        public DepthImagePixel[] DepthPixels;
        public byte[] ColorPixels;

        public KinectImageData(int depthDataLength, int colorDataLength)
        {
            DepthPixels = new DepthImagePixel[depthDataLength];
            ColorPixels = new byte[colorDataLength]; 
        }
    }
}
