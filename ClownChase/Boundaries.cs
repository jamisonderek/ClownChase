using System.Diagnostics;
using System.Windows;
using ClownChase.Properties;

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
        public readonly int DepthMinY;
        public readonly int DepthMaxY;



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

            var xRange = Settings.Default.xRange;
            DepthMinX = (int)(DepthWidth * xRange);
            DepthMaxX = (int)(DepthWidth * (1 - xRange));

            var yRange = Settings.Default.yRange;
            DepthMinY = (int)(DepthHeight * yRange);
            DepthMaxY = (int)(DepthHeight * (1 - yRange));

            Debug.Assert(DepthMinX >= 0);
            Debug.Assert(DepthMinX < DepthMaxX);
            Debug.Assert(DepthMaxX <= DepthWidth);
        }
    }
}
