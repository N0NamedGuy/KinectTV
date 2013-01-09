using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectTV.Kinect
{
    class SwipeGestureDetector: GestureDetector
    {
        public float SwipeMinimalLength { get; set; }
        public float SwipeMaximalHeight { get; set; }
        public int SwipeMinimalDuration { get; set; }
        public int SwipeMaximalDuration { get; set; }

        protected bool ScanPositions(Func<Vector3, Vector3, bool> heightFun,
            Func<Vector3, Vector3, bool> dirFun,
            Func<Vector3, Vector3, bool> lenFun,
            int minTime, int maxTime)
        {
            int start = 0;
            for (int i = 1; i < entries.Count - 1; i++)
            {
                if (heightFun(entries[0].Position, entries[i].Position) &&
                    dirFun(entries[0].Position, entries[i + 1].Position))
                {
                    start = i;
                }

                if (!lenFun(entries[i].Position, entries[start].Position))
                {
                    double milisecs = (entries[i].Time - entries[start].Time).TotalMilliseconds;
                    if (milisecs >= minTime && minTime <= maxTime)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public SwipeGestureDetector(JointType joint, int windowSize = 20) : base(joint, windowSize)
        {
            SwipeMinimalLength = 0.5f;
            SwipeMaximalHeight = 0.2f;
            SwipeMinimalDuration = 250;
            SwipeMaximalDuration = 1500;
        }
        
        protected override void LookForGesture()
        {
            Func<Vector3, Vector3, bool> heightFun =
                (p1, p2) => Math.Abs(p2.Y - p1.Y) < SwipeMaximalHeight;
            Func<Vector3, Vector3, bool> lengthFun = 
                (p1, p2) => Math.Abs(p2.X - p1.X) > SwipeMinimalLength;

            // Swipe to right
            if (ScanPositions(
                heightFun, // Height
                (p1, p2) => p2.X - p1.X > -0.01f, // Right direction
                lengthFun,  // Length
                SwipeMinimalDuration, SwipeMinimalDuration
            ))
            {
                RaiseGestureDetected("swipe_right");
                return;
            }

            // Swipe to left
            else if (ScanPositions(
                heightFun, // Height
                (p1, p2) => p2.X - p1.X < 0.01f, // Right direction
                lengthFun,  // Length
                SwipeMinimalDuration, SwipeMinimalDuration
            ))
            {
                RaiseGestureDetected("swipe_left");
                return;
            }
        }
    }
}
