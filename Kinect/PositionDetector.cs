using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectTV.Kinect
{
    using PositionEvaluator = Func<Skeleton, bool>;

    class PositionEventArgs : EventArgs
    {
        
        public PositionEvaluator Evaluator { get; set; }
        public int TimeElapsed { get; set; }
        public Skeleton Skeleton { get; set; }
    }

    class PositionDetector
    {
        public PositionEvaluator Evaluator { get; set; }
        public int Time { get; set; }

        protected DateTime lastTime;
        public event EventHandler<PositionEventArgs> OnPositionDetected;

        public PositionDetector()
        {
            Time = 1000;
            lastTime = DateTime.Now;
        }

        public PositionDetector(PositionEvaluator evaluator) : this()
        {
            Evaluator = evaluator;
        }

        public void Update(Skeleton skel)
        {
            if (Evaluator == null) return;
            
            DateTime now = DateTime.Now;
            if (Evaluator(skel))
            {
                double diff = (now - lastTime).TotalMilliseconds;

                if (diff >= Time)
                {
                    OnPositionDetected(this, new PositionEventArgs
                    {
                        Evaluator   = Evaluator,
                        TimeElapsed = (int)diff,
                        Skeleton    = skel
                    });
                    lastTime = now;
                }
            }
            else
            {
                lastTime = now;
            }
        }

    }
}
