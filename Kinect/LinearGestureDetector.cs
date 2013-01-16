using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectTV.Kinect
{
    class LinearGestureDetector: GestureDetector
    {
        public float SwipeMinimalLength { get; set; }
        public float SwipeMaximalHeight { get; set; }
        public int SwipeMinimalDuration { get; set; }
        public int SwipeMaximalDuration { get; set; }

        public enum SwipeDirection
        {
            Left, Right, 
            Up, Down,
            Front, Back
        }


        public bool[,] SwipeDirections = new bool
            [Enum.GetValues(typeof(JointType)).Length,
            Enum.GetValues(typeof(JointType)).Length];

        public void AddDirection(JointType joint, SwipeDirection dir)
        {
            Track(joint);
            SwipeDirections[(int)joint, (int)dir] = true; 
        }

        public void RemoveDirection(JointType joint, SwipeDirection dir)
        {
            Untrack(joint);
            SwipeDirections[(int)joint, (int)dir] = false;
        }
        
        public LinearGestureDetector(int windowSize = 20)
            : base(windowSize)
        {
            SwipeMinimalLength = 0.4f;
            SwipeMaximalHeight = 0.2f;
            SwipeMinimalDuration = 250;
            SwipeMaximalDuration = 1500;
        }

        protected bool ScanPositions(JointType joint,
            Func<Vector3, Vector3, bool> heightFun,
            Func<Vector3, Vector3, bool> dirFun,
            Func<Vector3, Vector3, bool> lenFun,
            int minTime, int maxTime)
        {
            CircularBuffer<Entry> entries = this.entries[(int)joint];

            int start = 0;
            for (int i = 1; i < entries.Count - 1; i++)
            {
                if (
                    !heightFun(entries[start].Position, entries[i].Position) ||
                    !dirFun(entries[i].Position, entries[i + 1].Position)
                   )
                {
                    start = i;
                }

                if (lenFun(entries[i].Position, entries[start].Position))
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
        
        protected override void LookForGesture()
        {
            Func<Vector3, Vector3, bool>[] heightFuns =
            {
                (p1, p2) =>  Math.Abs(p2.Y - p1.Y) < SwipeMaximalHeight, // X axis
                (p1, p2) =>  Math.Abs(p2.Y - p1.Y) < SwipeMaximalHeight, // X axis
                (p1, p2) =>  Math.Abs(p2.X - p1.X) < SwipeMaximalHeight, // Y axis
                (p1, p2) =>  Math.Abs(p2.X - p1.X) < SwipeMaximalHeight, // Y axis
                (p1, p2) =>  Math.Abs(p2.X - p1.X) < SwipeMaximalHeight, // Z axis
                (p1, p2) =>  Math.Abs(p2.X - p1.X) < SwipeMaximalHeight  // Z axis
            };

            Func<Vector3, Vector3, bool>[] lengthFuns =
            {
                (p1, p2) => Math.Abs(p2.X - p1.X) > SwipeMinimalLength, // X axis
                (p1, p2) => Math.Abs(p2.X - p1.X) > SwipeMinimalLength, // X axis
                (p1, p2) => Math.Abs(p2.Y - p1.Y) > SwipeMinimalLength, // Y axis
                (p1, p2) => Math.Abs(p2.Y - p1.Y) > SwipeMinimalLength, // Y axis
                (p1, p2) => Math.Abs(p2.Z - p1.Z) > SwipeMinimalLength, // Z axis
                (p1, p2) => Math.Abs(p2.Z - p1.Z) > SwipeMinimalLength  // Z axis
            };

            Func<Vector3, Vector3, bool>[] directionFuns =
            {
                (p1, p2) => p2.X - p1.X < 0.01f,  // left
                (p1, p2) => p2.X - p1.X > -0.01f, // right
                
                (p1, p2) => p2.Y - p1.Y > -0.01f, // down
                (p1, p2) => p2.Y - p1.Y < 0.01f,  // up
                
                (p1, p2) => p2.Y - p1.Y < 0.01f,  // back
                (p1, p2) => p2.Y - p1.Y > -0.01f, // front
            };

            string[] eventNames =
            {
                "swipe_left", "swipe_right",
                "swipe_up", "swipe_down",
                "push", "pull"
            };

            foreach (JointType joint in Enum.GetValues(typeof(JointType)))
            {
                if (!this.Tracked(joint)) continue;
                foreach (SwipeDirection dir in Enum.GetValues(typeof(SwipeDirection)))
                {

                    if (SwipeDirections[(int)joint, (int)dir] && ScanPositions(joint,
                        heightFuns[(int)dir],
                        directionFuns[(int)dir],
                        lengthFuns[(int)dir],
                        SwipeMinimalDuration, SwipeMaximalDuration))
                    {

                        RaiseGestureDetected(eventNames[(int)dir], joint);
                        continue;
                    }
                }
            }
        }
    }
}
