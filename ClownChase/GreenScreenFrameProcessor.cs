using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ClownChase
{
    class GreenScreenFrameProcessor : IFrameProcessor
    {
        private readonly Image _image;
        private readonly IMask _mask;

        public GreenScreenFrameProcessor(Image image, IMask mask)
        {
            _image = image;
            _mask = mask;
        }

        public string ProcessFrame(FrameReadyEventArgs eventArgs)
        {
            ColorPixelMask mask = null;

            string message = String.Empty;

            if (eventArgs.DepthReceived)
            {
                mask = new ColorPixelMask(eventArgs.Data.Boundaries);
                var nearestObject = _mask.PopulateColorPixelMask(eventArgs.Data, mask, eventArgs.Mapper);
                message += String.Format(" @{0}x{1}[{2},{3:00.0}]", nearestObject.X, nearestObject.Y, nearestObject.Depth, 0.003280 * nearestObject.Depth);
            }

            if (eventArgs.ColorReceived)
            {
                var sourceBitmap = _image.Source as WriteableBitmap;
                Debug.Assert(null != sourceBitmap);

                var maskBitmap = ((ImageBrush)_image.OpacityMask).ImageSource as WriteableBitmap;
                Debug.Assert(null != maskBitmap);

                sourceBitmap.WritePixels(eventArgs.Data.Boundaries.ColorRect, eventArgs.Data.ColorPixels, sourceBitmap.PixelWidth * sizeof(int), 0);
                if (mask != null)
                {
                    maskBitmap.WritePixels(eventArgs.Data.Boundaries.DepthRect, mask.Mask, eventArgs.Data.Boundaries.DepthRect.Width * ((maskBitmap.Format.BitsPerPixel + 7) / 8), 0);
                }
            }

            return message;
        }
    }
}
