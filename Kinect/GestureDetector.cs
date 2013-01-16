using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectTV.Kinect
{
    public class Entry
    {
        public DateTime Time { get; set;  }
        public Vector3 Position { get; set; }
        public JointType Joint { get; set; }
    }

    public class GestureEventArgs : EventArgs
    {
        public JointType Joint { get; set; }
        public string Gesture { get; set; }
    }

    public abstract class GestureDetector
    {
        public int MinimalPeriodBetweenGestures { get; set; }
        protected readonly CircularBuffer<Entry>[] entries;
        protected bool[] TrackedJoints;

        public event EventHandler<GestureEventArgs> OnGestureDetected;

        DateTime lastGesture = DateTime.Now;

        readonly int windowSize;

        protected GestureDetector(int windowSize = 20)
        {
            int nJoints = Enum.GetNames(typeof(JointType)).Length;
            TrackedJoints = new bool[nJoints];
            entries = new CircularBuffer<Entry>[nJoints];
            
            foreach (int i in Enumerable.Range(0, entries.Length))
            {
                entries[i] = new CircularBuffer<Entry>(windowSize);
            }

            this.windowSize = windowSize;
            MinimalPeriodBetweenGestures = 0;
        }

        public void Track(JointType joint)
        {
            TrackedJoints[(int)joint] = true;
        }

        public void Untrack(JointType joint)
        {
            TrackedJoints[(int)joint] = false;
        }

        public bool Tracked(JointType joint)
        {
            return TrackedJoints[(int)joint];
        }

        public virtual void Add(Skeleton skel)
        {
            foreach (Joint joint in skel.Joints)
            {
                if (!Tracked(joint.JointType)) continue;

                Entry newEntry = new Entry
                {
                    Position = Vector3.ToVector3(joint.Position),
                    Time = DateTime.Now,
                    Joint =  joint.JointType
                };

                entries[(int)joint.JointType].Add(newEntry);

                LookForGesture(joint.JointType);
            }

            
        }

        public void Clear()
        {
            foreach (int i in Enumerable.Range(0, entries.Length))
            {
                entries[i].Clear();
            }
        }

        protected abstract void LookForGesture(JointType joint);

        protected void RaiseGestureDetected(String gesture, JointType joint)
        {
            if ((DateTime.Now - lastGesture).TotalMilliseconds > MinimalPeriodBetweenGestures)
            {
                if (OnGestureDetected != null) OnGestureDetected(this,
                    new GestureEventArgs { Gesture = gesture, Joint = joint });

                lastGesture = DateTime.Now;
                Program.Notify(gesture + " gesture on " + joint.ToString()); 
            }
            entries[(int)joint].Clear();
        }

        
    }
}
