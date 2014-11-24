using System;

namespace ClownChase
{
    interface IMask
    {
        Position PopulateColorPixelMask(KinectImageData kinectImageData, ColorPixelMask mask, Mapper mapper);
    }
}
