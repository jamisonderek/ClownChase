using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;

namespace ClownChase
{
    public class ConnectedToNearestMask : IMask
    {
        internal class Pixel
        {
            public int Index;
            public int X;
            public int Y;
        }

        public Position PopulateColorPixelMask(KinectImageData kinectImageData, ColorPixelMask mask, Mapper mapper)
        {
            var nearestObject = kinectImageData.NearestObject();
            var goalDepth = nearestObject.Depth;
            if (goalDepth == int.MaxValue)
            {
                return nearestObject;
            }

            mask.Clear();
            var colorCoordinates = mapper.Map(kinectImageData);

            var checklist = new byte[kinectImageData.Boundaries.DepthDataLength];
            var points = new List<Pixel>
            {
                new Pixel
                {
                    Index = (nearestObject.Y * kinectImageData.Boundaries.DepthWidth) + nearestObject.X,
                    X = nearestObject.X,
                    Y = nearestObject.Y
                }                
            };

            var pointsIndex = 0;
            while (points.Count > pointsIndex)
            {
                ProcessPixel(kinectImageData, points, pointsIndex, checklist, mask, colorCoordinates, goalDepth);
                pointsIndex++;
            }

            return nearestObject;
        }

        private bool ShowPixel(int goalDepth, int depth)
        {
            return Math.Abs(goalDepth - depth) < 300;
        }

        private void ProcessPixel(KinectImageData kinectImageData, List<Pixel> points, int pointsIndex, byte[] checklist, ColorPixelMask mask, ColorImagePoint[] colorCoordinates, int goalDepth)
        {
            var p = points[pointsIndex];
            var i = p.Index;
            var depthPixel = kinectImageData.DepthPixels[i];

            Debug.Assert(depthPixel.PlayerIndex==0);

            if (ShowPixel(goalDepth, depthPixel.Depth))
            {
                depthPixel.PlayerIndex = 1;

                var colorImagePoint = colorCoordinates[i];
                var colorInDepthX = colorImagePoint.X / kinectImageData.Boundaries.ColorToDepthDivisor;
                var colorInDepthY = colorImagePoint.Y / kinectImageData.Boundaries.ColorToDepthDivisor;
                mask.Set(colorInDepthX, colorInDepthY);

                AddAdjacent(kinectImageData, points, checklist, p);
            }
            else
            {
                depthPixel.PlayerIndex = -1;
            }
        }

        private void AddAdjacent(KinectImageData kinectImageData, List<Pixel> points, byte[] checklist, Pixel p)
        {
            var i = p.Index;
            var x = p.X;
            var y = p.Y;

            if (x > 0)
            {
                AddToList(kinectImageData, points, checklist, i - 1, x-1, y);
            }
            if (x + 1 < kinectImageData.Boundaries.DepthWidth)
            {
                AddToList(kinectImageData, points, checklist, i + 1, x+1, y);
            }
            if (y > 0)
            {
                AddToList(kinectImageData, points, checklist, i - kinectImageData.Boundaries.DepthWidth, x, y-1);
            }
            if (y + 1 < kinectImageData.Boundaries.DepthHeight)
            {
                AddToList(kinectImageData, points, checklist, i + kinectImageData.Boundaries.DepthWidth, x, y+1);
            }
        }

        private void AddToList(KinectImageData kinectImageData, List<Pixel> points, byte[] checklist, int i, int x, int y)
        {
            if (0==checklist[i] && kinectImageData.DepthPixels[i].PlayerIndex == 0)
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
