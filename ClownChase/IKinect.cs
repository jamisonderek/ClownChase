using Microsoft.Kinect;

namespace ClownChase
{
    public interface IKinect
    {
        IKinectSensor GetSensor();
    }
}