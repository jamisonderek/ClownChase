using System;

namespace ClownChase
{
    public interface IKinectSensor
    {
        bool Connected { get; }
        void Initialize();
        bool Start();
        bool Stop();

        event EventHandler<FrameReadyEventArgs> FrameReady;
    }
}
