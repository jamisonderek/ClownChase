using System;
using System.Collections.Generic;
using System.Linq;

namespace ClownChase
{
    public class CaptureFrameProcessor : IFrameProcessor
    {
        public bool Capture;

        private readonly List<CapturedFrame> _capturedFrames;
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

        public CaptureFrameProcessor()
        {
            _capturedFrames = new List<CapturedFrame>();
        }

        public string ProcessFrame(FrameReadyEventArgs eventArgs)
        {
            if (Capture)
            {
                byte[] m = null;
                if (eventArgs.Mask != null)
                {
                    m = new byte[eventArgs.Mask.Mask.Length];
                    for (int i = 0; i < m.Length; i++) m[i] = (eventArgs.Mask.Mask[i] == 0) ? (byte)0 : (byte)255;
                }
                var capturedFrame = new CapturedFrame
                {
                    ColorPixels = (byte[])eventArgs.Data.ColorPixels.Clone(), 
                    Mask = m, 
                    NearPosition = eventArgs.Data.NearestObject()
                };
                _capturedFrames.Add(capturedFrame);
            }

            return String.Empty;
        }
    }
}
