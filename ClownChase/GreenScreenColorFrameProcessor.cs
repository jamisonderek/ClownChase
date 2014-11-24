using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClownChase
{
    class GreenScreenColorFrameProcessor : IFrameProcessor
    {
        private readonly Image _image;

        public GreenScreenColorFrameProcessor(Image image)
        {
            _image = image;
        }

        public string ProcessFrame(FrameReadyEventArgs eventArgs)
        {
            string message = String.Empty;

            if (eventArgs.ColorReceived)
            {
                var sourceBitmap = _image.Source as WriteableBitmap;
                Debug.Assert(null != sourceBitmap);

                var maskBitmap = ((ImageBrush)_image.OpacityMask).ImageSource as WriteableBitmap;
                Debug.Assert(null != maskBitmap);

                sourceBitmap.WritePixels(eventArgs.Data.Boundaries.ColorRect, eventArgs.Data.ColorPixels, sourceBitmap.PixelWidth * sizeof(int), 0);
                if (eventArgs.Mask != null)
                {
                    maskBitmap.WritePixels(eventArgs.Data.Boundaries.DepthRect, eventArgs.Mask.Mask, eventArgs.Data.Boundaries.DepthRect.Width * ((maskBitmap.Format.BitsPerPixel + 7) / 8), 0);
                }
            }

            return message;
        }
    }
}
