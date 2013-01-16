using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectTV.Kinect
{
    class PositionEventArgs : EventArgs
    {
        public Func<Skeleton, bool> Evaluator { get; set; }
        public int TimeElapsed { get; set; }
        public Skeleton Skeleton { get; set; }
    }

    class PositionDetector
    {
        public Func<Skeleton, bool> Evaluator { get; set; }
        public int Time { get; set; }

        protected int lastTime;
        public event EventHandler<PositionEventArgs> OnPositionDetected;

        PositionDetector()
        {
            lastTime = DateTime.Now.Millisecond;
        }

        public void Update(Skeleton skel)
        {
            if (Evaluator == null) return;
            
            int now = DateTime.Now.Millisecond;
            if (Evaluator(skel))
            {
                int diff = now - lastTime; 
                if (diff >= Time)
                {
                    OnPositionDetected(this, new PositionEventArgs {
                        Evaluator = Evaluator, TimeElapsed = diff, Skeleton = skel });
                }
            }
            lastTime = now;
            
        }

    }
}
