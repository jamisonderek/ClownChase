using System;
using System.Collections.Generic;
using ClownChase.Properties;
using Microsoft.Kinect;

namespace ClownChase
{
    public class KinectImageData
    {
        public const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;
        public const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;

        public DepthImagePixel[] DepthPixels;
        public byte[] ColorPixels;
        public Boundaries Boundaries;

        public KinectImageData(Boundaries boundaries)
        {
            Boundaries = boundaries;
            DepthPixels = new DepthImagePixel[Boundaries.DepthDataLength];
            ColorPixels = new byte[Boundaries.ColorDataLength];
        }

        public class Position
        {
            public int Depth;
            public int NearestX;
            public int NearestY;
        }

        internal class Pixel
        {
            public int Index;
            public int X;
            public int Y;
        }

        public Position PopulateColorPixelMask(ColorPixelMask mask, Mapper mapper, Func<int, int, bool> showPixel)
        {
            var nearestObject = NearestObject();
            var goalDepth = nearestObject.Depth;
            if (goalDepth == int.MaxValue)
            {
                return nearestObject;
            }

            mask.Clear();
            var colorCoordinates = mapper.Map(this);

            var checklist = new byte[Boundaries.DepthDataLength];
            var points = new List<Pixel>
            {
                new Pixel()
                {
                    Index = (nearestObject.NearestY * Boundaries.DepthWidth) + nearestObject.NearestX,
                    X = nearestObject.NearestX,
                    Y = nearestObject.NearestY
                }                
            };

            var pointsIndex = 0;
            while (points.Count > pointsIndex)
            {
                ProcessPixel(points, pointsIndex++, goalDepth, checklist, mask, colorCoordinates, showPixel);
            }

            return nearestObject;
        }

        private void ProcessPixel(List<Pixel> points, int pointsIndex, int goalDepth, byte[] checklist, ColorPixelMask mask, ColorImagePoint[] colorCoordinates, Func<int, int, bool> showPixel)
        {
            var p = points[pointsIndex];
            var i = p.Index;
            var depthPixel = DepthPixels[i];

            if (depthPixel.PlayerIndex != 0)
            {
                return;
            }

            if (showPixel(goalDepth, depthPixel.Depth))
            {
                depthPixel.PlayerIndex = 1;

                var colorImagePoint = colorCoordinates[i];
                var colorInDepthX = colorImagePoint.X / Boundaries.ColorToDepthDivisor;
                var colorInDepthY = colorImagePoint.Y / Boundaries.ColorToDepthDivisor;
                mask.Set(colorInDepthX, colorInDepthY);

                AddAdjacent(points, checklist, p);
            }
            else
            {
                depthPixel.PlayerIndex = -1;
            }
        }

        private Position NearestObject()
        {
            var minDepth = int.MaxValue;
            var position = new Position();
            for (var y = Boundaries.DepthMinY; y < Boundaries.DepthMaxY; y += Settings.Default.objectYDelta)
            {
                var offset = (y * Boundaries.DepthWidth);
                for (var x = Boundaries.DepthMinX; x < Boundaries.DepthMaxX; x += Settings.Default.objectXDelta)
                {
                    var i = offset + x;
                    var d = DepthPixels[i].Depth;
                    if (d > 0 && d < minDepth)
                    {
                        minDepth = d;
                        position.NearestX = x;
                        position.NearestY = y;
                    }
                }
            }
            position.Depth = minDepth;

            return position;
        }

        private void AddAdjacent(List<Pixel> points, byte[] checklist, Pixel p)
        {
            var i = p.Index;
            var x = p.X;
            var y = p.Y;

            if (x > 0)
            {
                // left
                AddToList(points, checklist, i - 1, x-1, y);
            }
            if (x + 1 < Boundaries.DepthWidth)
            {
                // right
                AddToList(points, checklist, i + 1, x+1, y);
            }
            if (y > 0)
            {
                // up
                AddToList(points, checklist, i - Boundaries.DepthWidth, x, y-1);
            }
            if (y + 1 < Boundaries.DepthHeight)
            {
                // down
                AddToList(points, checklist, i + Boundaries.DepthWidth, x, y+1);
            }
        }

        private void AddToList(List<Pixel> points, byte[] checklist, int i, int x, int y)
        {
            if (0==checklist[i] && DepthPixels[i].PlayerIndex == 0)
            {
                lock (checklist)
                {
                    if (checklist[i] == 0)
                    {
                        checklist[i] = 1;
                        points.Add(new Pixel
                        {
                            Index = i,
                            X = x,
                            Y = y
                        });   
                    }                    
                }
            }            
        }
    }
}
