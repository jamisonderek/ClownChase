using System;

namespace ClownChase
{
    public interface IKinectSensor
    {
        bool Connected { get; }
        void Initialize();
        bool Start();
        bool Stop();
        Boundaries Boundaries { get; }

        event EventHandler<FrameReadyEventArgs> FrameReady;
    }
}
