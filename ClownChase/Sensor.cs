using System;
using Microsoft.Kinect;

namespace ClownChase
{
    public class Sensor : IKinectSensor
    {
        private KinectSensor _kinectSensor;

        public Sensor(KinectSensor kinectSensor)
        {
            _kinectSensor = kinectSensor;
        }

        public bool Connected
        {
            get { return _kinectSensor != null && (_kinectSensor.Status == KinectStatus.Connected); }
        }

        public void Initialize()
        {
            var depthStream = _kinectSensor.DepthStream;
            var colorStream = _kinectSensor.ColorStream;

            depthStream.Enable(KinectImageData.DepthFormat);
            depthStream.Range = DepthRange.Default;
            colorStream.Enable(KinectImageData.ColorFormat);
        }

        public bool Start()
        {
            if (_kinectSensor != null)
            {
                try
                {
                    _kinectSensor.Start();
                }
                catch (Exception)
                {
                    _kinectSensor = null;
                }
            }

            return _kinectSensor != null;
        }

        public bool Stop()
        {
            if (_kinectSensor != null)
            {
                _kinectSensor.Stop();
            }

            return _kinectSensor != null;
        }    
    }
}
