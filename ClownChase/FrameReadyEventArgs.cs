using System;
namespace ClownChase
{
    public class FrameReadyEventArgs : EventArgs
    {
        public bool DepthReceived;
        public bool ColorReceived;
        public KinectImageData Data;
        public ColorPixelMask Mask;
        public Mapper Mapper;
    }
}
