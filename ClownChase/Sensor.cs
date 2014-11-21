using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;

namespace ClownChase
{
    public class Sensor : IKinectSensor
    {
        private readonly KinectSensor _kinectSensor;

        public Sensor(KinectSensor kinectSensor)
        {
            _kinectSensor = kinectSensor;
        }

        public bool Connected
        {
            get { return _kinectSensor != null && (_kinectSensor.Status == KinectStatus.Connected); }
        }

    }
}
