using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectTV.Kinect
{
    class LinearGestureDetector: GestureDetector
    {
        public float MinimalLength { get; set; }
        public float MaximalHeight { get; set; }
        public int MinimalDuration { get; set; }
        public int MaximalDuration { get; set; }

        public enum Direction
        {
            Left, Right, 
            Up, Down,
            Front, Back
        }
        
        public bool[,] SwipeDirections = new bool
            [Enum.GetValues(typeof(JointType)).Length,
            Enum.GetValues(typeof(JointType)).Length];

        public void AddDirection(JointType joint, Direction dir)
        {
            Track(joint);
            SwipeDirections[(int)joint, (int)dir] = true; 
        }

        public void RemoveDirection(JointType joint, Direction dir)
        {
            Untrack(joint);
            SwipeDirections[(int)joint, (int)dir] = false;
        }
        
        public LinearGestureDetector(int windowSize = 20)
            : base(windowSize)
        {
            MinimalLength = 0.4f;
            MaximalHeight = 0.2f;
            MinimalDuration = 250;
            MaximalDuration = 1500;
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
                (p1, p2) =>  Math.Abs(p2.Y - p1.Y) < MaximalHeight, // X axis
                (p1, p2) =>  Math.Abs(p2.Y - p1.Y) < MaximalHeight, // X axis
                (p1, p2) =>  Math.Abs(p2.X - p1.X) < MaximalHeight, // Y axis
                (p1, p2) =>  Math.Abs(p2.X - p1.X) < MaximalHeight, // Y axis
                (p1, p2) =>  Math.Abs(p2.X - p1.X) < MaximalHeight, // Z axis
                (p1, p2) =>  Math.Abs(p2.X - p1.X) < MaximalHeight  // Z axis
            };

            Func<Vector3, Vector3, bool>[] lengthFuns =
            {
                (p1, p2) => Math.Abs(p2.X - p1.X) > MinimalLength, // X axis
                (p1, p2) => Math.Abs(p2.X - p1.X) > MinimalLength, // X axis
                (p1, p2) => Math.Abs(p2.Y - p1.Y) > MinimalLength, // Y axis
                (p1, p2) => Math.Abs(p2.Y - p1.Y) > MinimalLength, // Y axis
                (p1, p2) => Math.Abs(p2.Z - p1.Z) > MinimalLength, // Z axis
                (p1, p2) => Math.Abs(p2.Z - p1.Z) > MinimalLength  // Z axis
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
                foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {

                    if (SwipeDirections[(int)joint, (int)dir] && ScanPositions(joint,
                        heightFuns[(int)dir],
                        directionFuns[(int)dir],
                        lengthFuns[(int)dir],
                        MinimalDuration, MaximalDuration))
                    {

                        RaiseGestureDetected(eventNames[(int)dir], joint);
                        continue;
                    }
                }
            }
        }
    }
}
