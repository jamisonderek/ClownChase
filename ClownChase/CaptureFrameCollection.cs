using System.Collections.Generic;

namespace ClownChase
{
    public class CaptureFrameCollection : ICaptureFrameCollection
    {
        private readonly List<CapturedFrame> _capturedFrames;

        public CaptureFrameCollection()
        {
            _capturedFrames = new List<CapturedFrame>();
        }

        public IEnumerable<CapturedFrame> CapturedFrames
        {
            get { return _capturedFrames.AsReadOnly(); }
        }

        public CapturedFrame Get(int depth)
        {
            if (_capturedFrames.Count == 0)
            {
                return null;
            }

            CapturedFrame bestFrame = null;
            foreach (CapturedFrame frame in _capturedFrames)
            {
                var d = frame.NearPosition.Depth;
                if (d > depth)
                {
                    if (bestFrame == null)
                    {
                        bestFrame = frame;
                    }
                    else
                    {
                        if (d < bestFrame.NearPosition.Depth)
                        {
                            bestFrame = frame;
                        }
                    }
                }
            }

            return bestFrame;
        }

        public void Add(CapturedFrame frame)
        {
            _capturedFrames.Add(frame);
        }
    }
}
