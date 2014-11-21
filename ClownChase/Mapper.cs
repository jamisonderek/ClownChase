using Microsoft.Kinect;

namespace ClownChase
{
    public class Mapper
    {
        public ColorImagePoint[] ColorCoordinates;
        private readonly CoordinateMapper _coordinateMapper;

        public Mapper(Boundaries boundaries, CoordinateMapper coordinateMapper)
        {
            ColorCoordinates = new ColorImagePoint[boundaries.DepthDataLength];
            _coordinateMapper = coordinateMapper;
        }

        public ColorImagePoint[] Map(KinectImageData kinectImageData)
        {
            var depthData = kinectImageData.DepthPixels;
            _coordinateMapper.MapDepthFrameToColorFrame(KinectImageData.DepthFormat, depthData, KinectImageData.ColorFormat, ColorCoordinates);
            return ColorCoordinates;
        }
    }
}
