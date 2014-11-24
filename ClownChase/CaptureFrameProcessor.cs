using System;
using System.Collections.Generic;

namespace ClownChase
{
    public class CaptureFrameProcessor : IFrameProcessor
    {
        private readonly ICaptureFrameCollection _captureFrameCollection;
        public bool Capture;

        public CaptureFrameProcessor(ICaptureFrameCollection captureFrameCollection)
        {
            _captureFrameCollection = captureFrameCollection;
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
                _captureFrameCollection.Add(capturedFrame);
            }

            return String.Empty;
        }
    }
}
