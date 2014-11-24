using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClownChase
{
    public class RenderFrameProcessor : IFrameProcessor
    {
        private readonly CaptureFrameProcessor _captures;
        private readonly Image _image;

        public RenderFrameProcessor(CaptureFrameProcessor captures, Image image)
        {
            _captures = captures;
            _image = image;
        }

        public string ProcessFrame(FrameReadyEventArgs eventArgs)
        {
            var depth = eventArgs.Data.NearestObject().Depth;
            depth += 100;

            var capturedFrame = _captures.Get(depth);
            var mask = new int[eventArgs.Data.Boundaries.DepthDataLength];
            if (capturedFrame.Mask != null)
            {
                for (var i = 0; i < mask.Length; i++)
                {
                    mask[i] = (capturedFrame.Mask[i] == 0) ? 0 : -1;
                }
            }
            else
            {
                for (var i = 0; i < mask.Length; i++)
                {
                    mask[i] = -1;
                }
            }

            var sourceBitmap = _image.Source as WriteableBitmap;
            Debug.Assert(null != sourceBitmap);

            var maskBitmap = ((ImageBrush)_image.OpacityMask).ImageSource as WriteableBitmap;
            Debug.Assert(null != maskBitmap);

            sourceBitmap.WritePixels(eventArgs.Data.Boundaries.ColorRect, capturedFrame.ColorPixels, sourceBitmap.PixelWidth * sizeof(int), 0);
            if (eventArgs.Mask != null)
            {
                maskBitmap.WritePixels(eventArgs.Data.Boundaries.DepthRect, mask, eventArgs.Data.Boundaries.DepthRect.Width * ((maskBitmap.Format.BitsPerPixel + 7) / 8), 0);
            }

            return String.Format(" C:{0}", capturedFrame.NearPosition.Depth);
        }
    }
}
