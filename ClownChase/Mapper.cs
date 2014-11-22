using Microsoft.Kinect;

namespace ClownChase
{
    public class Mapper
    {
        private readonly int _length;
        private readonly CoordinateMapper _coordinateMapper;

        public Mapper(Boundaries boundaries, CoordinateMapper coordinateMapper)
        {
            _length = boundaries.DepthDataLength;
            _coordinateMapper = coordinateMapper;
        }

        public ColorImagePoint[] Map(KinectImageData kinectImageData)
        {
            var depthData = kinectImageData.DepthPixels;
            var colorCoordinates = new ColorImagePoint[_length];
            _coordinateMapper.MapDepthFrameToColorFrame(KinectImageData.DepthFormat, depthData, KinectImageData.ColorFormat, colorCoordinates);
            return colorCoordinates;
        }
    }
}
