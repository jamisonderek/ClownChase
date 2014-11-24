using System.Collections.Generic;

namespace ClownChase
{
    public interface ICaptureFrameCollection
    {
        IEnumerable<CapturedFrame> CapturedFrames { get; }
        CapturedFrame Get(int depth);
        void Add(CapturedFrame frame);
    }
}
