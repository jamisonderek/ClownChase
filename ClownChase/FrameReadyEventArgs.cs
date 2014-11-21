using System;
namespace ClownChase
{
    public class FrameReadyEventArgs : EventArgs
    {
        public bool DepthReceived;
        public bool ColorReceived;
        public KinectImageData Data;
    }
}
