﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Kinect;

namespace ClownChase
{
    public class Kinect : IKinect
    {
        public IKinectSensor GetSensor()
        {
            return new Sensor(Sensors.FirstOrDefault(k => k.Status == KinectStatus.Connected));
        }

        private IEnumerable<KinectSensor> Sensors
        {
            get { return KinectSensor.KinectSensors; }
        }
    }
}
