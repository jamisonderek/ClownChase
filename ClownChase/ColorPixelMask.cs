﻿using System;

namespace ClownChase
{
    public class ColorPixelMask
    {
        public int[] Mask;

        private const int OpaquePixelValue = -1;
        private int _depthWidth;
        private int _depthHeight;

        public ColorPixelMask(Boundaries boundaries)
        {
            Mask = new int[boundaries.ColorDataLength];
            _depthHeight = boundaries.DepthHeight;
            _depthWidth = boundaries.DepthWidth;
        }

        public void Clear()
        {
            Array.Clear(Mask, 0, Mask.Length);
        }

        public void Set(int x, int y)
        {
            if ((x > 0 && x < _depthWidth) && (y >= 0 && y < _depthHeight))
            {
                int colorIndex = x + (y * _depthWidth);
                Mask[colorIndex] = OpaquePixelValue;
                Mask[colorIndex - 1] = OpaquePixelValue;
            }
        }
    }
}
