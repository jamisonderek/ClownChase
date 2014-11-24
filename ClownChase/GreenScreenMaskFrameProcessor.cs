using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ClownChase
{
    class GreenScreenMaskFrameProcessor : IFrameProcessor
    {
        private readonly IMask _mask;

        public GreenScreenMaskFrameProcessor(IMask mask)
        {
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

            return message;
        }
    }
}
