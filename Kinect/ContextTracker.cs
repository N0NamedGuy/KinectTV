using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectTV.Kinect
{
    public class ContextTracker
    {
        readonly Dictionary<int, List<ContextPoint>> positions = new Dictionary<int, List<ContextPoint>>();
        readonly int windowSize;

        public float Threshold { get; set; }

        public ContextTracker(int windowSize = 40, float threshold = 0.05f)
        {
            this.windowSize = windowSize;
            Threshold = threshold;
        }

        public void Add(Vector3 position, int trackID)
        {
            if (!positions.ContainsKey(trackID))
                positions.Add(trackID, new List<ContextPoint>());

            positions[trackID].Add(new ContextPoint { Position = position, Time = DateTime.Now } );

            if (positions[trackID].Count > windowSize)
            {
                positions[trackID].RemoveAt(0);
            }
        }

        public bool IsStable(int trackID)
        {
            List<ContextPoint> currentPoints = positions[trackID];
            if (currentPoints.Count != windowSize) return false;

            Vector3 current = currentPoints.Last().Position;
            Vector3 average = Vector3.Zero;

            for (int index = 0; index < currentPoints.Count - 2; index++)
            {
                average += currentPoints[index].Position;
            }

            average /= currentPoints.Count - 1;

            return (average - current).Length() < Threshold;
        }

        private float getSpeed(ContextPoint prevPoint, ContextPoint curPoint)
        {
            return (float)((curPoint.Position - prevPoint.Position).Length()
                / (curPoint.Time - prevPoint.Time).TotalMilliseconds);
        }

        public bool IsStableRelativeToCurrentSpeed(int trackID)
        {
            List<ContextPoint> currentPoints = positions[trackID];
            if (currentPoints.Count < 2) return false;

            return getSpeed(
                currentPoints[currentPoints.Count - 2],
                currentPoints[currentPoints.Count - 1])
                
                < Threshold;
        }

        public bool IsStableRelativeToAverageSpeed(int trackID)
        {
            List<ContextPoint> currentPoints = positions[trackID];
            if (currentPoints.Count < 2) return false;

            return getSpeed(
                currentPoints.First(),
                currentPoints.Last())

                < Threshold;
        }

        private Vector3 getJoint(Skeleton skel, JointType joint)
        {
            return Vector3.ToVector3(skel.Joints.First(j => j.JointType == joint).Position);
        }

        public bool IsShouldersTowardsSensor(Skeleton skel)
        {
            Vector3 leftShoulderPos = getJoint(skel, JointType.ShoulderLeft);
            Vector3 rightShoulderPos = getJoint(skel, JointType.ShoulderRight);

            float leftDist = leftShoulderPos.Z;
            float rightDist = rightShoulderPos.Z;

            return Math.Abs(leftDist - rightDist) < Threshold;
        }
    }
}
