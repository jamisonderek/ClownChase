using System.Diagnostics;
using System.Windows;

namespace ClownChase
{
    public class Boundaries
    {
        public readonly Int32Rect DepthRect;
        public readonly Int32Rect ColorRect;
        public readonly int ColorToDepthDivisor;
        public readonly int DepthDataLength;
        public readonly int ColorDataLength;

        public readonly int DepthMinX;
        public readonly int DepthMaxX;

        public int DepthWidth
        {
            get { return DepthRect.Width; }
        }

        public int DepthHeight
        {
            get { return DepthRect.Height; }
        }

        public Boundaries(int depthWidth, int depthHeight, int depthDataLength, int colorWidth, int colorHeight, int colorDataLength)
        {
            ColorToDepthDivisor = colorWidth / depthWidth;
            DepthRect = new Int32Rect(0, 0, depthWidth, depthHeight);
            ColorRect = new Int32Rect(0, 0, colorWidth, colorHeight);
            DepthDataLength = depthDataLength;
            ColorDataLength = colorDataLength;

            const double edges = 0.3;
            DepthMinX = (int)(DepthWidth * edges);
            DepthMaxX = (int)(DepthWidth * (1 - edges));

            Debug.Assert(DepthMinX >= 0);
            Debug.Assert(DepthMinX < DepthMaxX);
            Debug.Assert(DepthMaxX <= DepthWidth);
        }
    }
}
