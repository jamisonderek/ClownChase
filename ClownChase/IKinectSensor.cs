using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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
