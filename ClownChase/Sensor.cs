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

            _kinectSensor.AllFramesReady += AllFramesReady;
        }


        public event EventHandler<FrameReadyEventArgs> FrameReady;

        private void AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            var frameReady = FrameReady;
            if (frameReady.GetInvocationList().Length > 0)
            {
                var depthStream = _kinectSensor.DepthStream;
                var colorStream = _kinectSensor.ColorStream;

                var frameReadyEvent = new FrameReadyEventArgs();
                var kinectImageData = new KinectImageData(depthStream.FramePixelDataLength, colorStream.FramePixelDataLength);

                using (var depthFrame = e.OpenDepthImageFrame())
                {
                    if (null != depthFrame)
                    {
                        depthFrame.CopyDepthImagePixelDataTo(kinectImageData.DepthPixels);
                        frameReadyEvent.DepthReceived = true;
                    }
                }

                using (var colorFrame = e.OpenColorImageFrame())
                {
                    if (null != colorFrame)
                    {
                        colorFrame.CopyPixelDataTo(kinectImageData.ColorPixels);
                        frameReadyEvent.ColorReceived = true;
                    }
                }

                frameReadyEvent.Data = kinectImageData;
                frameReadyEvent.Boundaries = new Boundaries(depthStream.FrameWidth, depthStream.FrameHeight, depthStream.FramePixelDataLength, colorStream.FrameWidth, colorStream.FrameHeight, colorStream.FramePixelDataLength);

                frameReady(sender, frameReadyEvent);                
            }
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
