using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ClownChase
{
    class GreenScreenMaskFrameProcessor : IFrameProcessor
    {
        private readonly Image _image;
        private readonly IMask _mask;

        public GreenScreenMaskFrameProcessor(Image image, IMask mask)
        {
            _image = image;
            _mask = mask;
        }

        public string ProcessFrame(FrameReadyEventArgs eventArgs)
        {
            string message = String.Empty;

            if (eventArgs.DepthReceived)
            {
                eventArgs.Mask = new ColorPixelMask(eventArgs.Data.Boundaries);
                var nearestObject = _mask.PopulateColorPixelMask(eventArgs.Data, eventArgs.Mask, eventArgs.Mapper);
                message += String.Format(" @{0}x{1}[{2},{3:00.0}]", nearestObject.X, nearestObject.Y, nearestObject.Depth, 0.003280 * nearestObject.Depth);
            }

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
