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
        public Vector3 Position { get; set;  }
    }

    public class GestureEventArgs : EventArgs
    {
        public JointType Joint { get; set; }
        public string Gesture { get; set; }
    }

    public abstract class GestureDetector
    {
        public int MinimalPeriodBetweenGestures { get; set; }
        protected readonly List<Entry> entries = new List<Entry>();

        public event EventHandler<GestureEventArgs> OnGestureDetected;
        public JointType Joint { get; set; }

        DateTime lastGesture = DateTime.Now;

        readonly int windowSize;

        protected GestureDetector(JointType joint, int windowSize = 20)
        {
            this.Joint = joint;
            this.windowSize = windowSize;
            MinimalPeriodBetweenGestures = 0;
        }

        public virtual void Add(SkeletonPoint pos, KinectSensor sensor)
        {
            Entry newEntry = new Entry { Position = Vector3.ToVector3(pos), Time = DateTime.Now };
            entries.Add(newEntry);

            if (entries.Count > windowSize)
            {
                entries.RemoveAt(0);
            }

            LookForGesture();
        }

        protected abstract void LookForGesture();

        protected void RaiseGestureDetected(String gesture)
        {
            if ((DateTime.Now - lastGesture).TotalMilliseconds > MinimalPeriodBetweenGestures)
            {
                if (OnGestureDetected != null) OnGestureDetected(this,
                    new GestureEventArgs { Gesture = gesture, Joint = Joint });

                lastGesture = DateTime.Now;
            }
            entries.Clear();
        }

        
    }
}
