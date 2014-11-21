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

        bool Start();
        bool Stop();

    }
}
