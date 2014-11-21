using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;

namespace ClownChase
{
    public class Kinect : IKinect
    {
        public KinectSensor GetSensor()
        {
            return Sensors.FirstOrDefault(k => k.Status == KinectStatus.Connected);
        }

        private IEnumerable<KinectSensor> Sensors
        {
            get { return KinectSensor.KinectSensors; }
        }
    }
}
