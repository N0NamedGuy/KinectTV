using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Awesomium.Core;

using KinectTV.Kinect;

namespace KinectTV
{
    class KinectHelper
    {
        IWebView wv;
        KinectSensor sensor;

        ContextTracker ct;
        SwipeGestureDetector rhandDetect;
        SwipeGestureDetector lhandDetect;

        JSObject kinectGlobalObj;
        
        public bool Listening { get; set; }

        private JSObject getJSKinectHandler()
        {
            JSValue kinectObj = wv.ExecuteJavascriptWithResult("KinectHandler");
            return !kinectObj.IsObject ? null : (JSObject)kinectObj;
        }

        public KinectHelper(IWebView wv)
        {
            this.wv = wv;
            Listening = false;
            wv.DocumentReady += new UrlEventHandler(wv_DocumentReady);

            try
            {
                // Listen to kinect status change
                KinectSensor.KinectSensors.StatusChanged += Kinects_StatusChanged;

                // Get the first connected kinect
                foreach (KinectSensor kinect in KinectSensor.KinectSensors)
                {
                    if (kinect.Status == KinectStatus.Connected)
                    {
                        Program.Notify("Kinect is connected");
                        sensor = kinect;
                        break;
                    }
                }

                InitSensor();
            }
            catch (Exception ex)
            {
                Program.Notify("Kinect exception: " + ex.ToString());
            }
        }

        void wv_DocumentReady(object sender, UrlEventArgs e_)
        {
            kinectGlobalObj = wv.CreateGlobalJavascriptObject("KinectHelper");

            kinectGlobalObj.Bind("userIsFacing", true, (s, e) => e.Result = Listening);
        }

        ~KinectHelper()
        {
            CleanSensor();
        }

        void Kinects_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            JSObject kinectObj = getJSKinectHandler();
            if (kinectObj == null) return;

            switch (e.Status)
            {
                case KinectStatus.Connected:
                    try
                    {
                        kinectObj.Invoke("onConnected");
                    }
                    catch (Exception)
                    {
                        ReportJSError("onConnected");
                    }

                    if (sensor == null)
                    {
                        sensor = e.Sensor;
                        InitSensor();
                    }
                    break;

                case KinectStatus.NotPowered:
                    Program.Notify("Kinect is not powered");
                    break;

                case KinectStatus.Disconnected:
                    try
                    {
                        kinectObj.Invoke("onDisconnected");
                    }
                    catch (Exception)
                    {
                        ReportJSError("onDisconnected");
                    }
                    if (sensor == e.Sensor)
                    {
                        CleanSensor();
                    }
                    break;

                default:
                    break;
            }
        }

        private void ReportJSError(string evt)
        {
            Error err = wv.GetLastError();
            Program.Notify(
                evt + " error: " + err.ToString() + " (" + err.GetTypeCode() + ")"
            ); 
        }

        private void InitSensor()
        {
            if (sensor != null)
            {
                Program.Notify("Kinect sensor enabled");
                sensor.Start();
                sensor.SkeletonStream.Enable();
                sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;

                ct = new ContextTracker();
                
                lhandDetect = new SwipeGestureDetector(JointType.HandLeft);
                rhandDetect = new SwipeGestureDetector(JointType.HandRight);
                
                lhandDetect.OnGestureDetected += swipe_OnGestureDetected;
                rhandDetect.OnGestureDetected += swipe_OnGestureDetected;
            }
        }

        void swipe_OnGestureDetected(object sender, GestureEventArgs e)
        {
            JSObject kinectObj = getJSKinectHandler();
            if (kinectObj == null) return;

            try
            {
                kinectObj.Invoke("onGesture", e.Gesture, JointToString(e.Joint));
            }
            catch (Exception)
            {
                ReportJSError("onGesture");
            }
        }


        private static Skeleton[] GetSkeletons(SkeletonFrame frame)
        {
            if (frame == null) return null;

            var skeletons = new Skeleton[frame.SkeletonArrayLength];
            frame.CopySkeletonDataTo(skeletons);
            return skeletons;
        }

        String JointToString(JointType type)
        {
            switch (type)
            {
                case JointType.AnkleLeft: return "ankle_left";
                case JointType.AnkleRight: return "ankle_right";
                case JointType.ElbowLeft: return "elbow_left";
                case JointType.ElbowRight: return "elbow_right";
                case JointType.FootLeft: return "foot_left";
                case JointType.FootRight: return "foot_right";
                case JointType.HandLeft: return "hand_left";
                case JointType.HandRight: return "hand_right";
                case JointType.Head: return "head";
                case JointType.HipCenter: return "hip_center";
                case JointType.HipLeft: return "hip_left";
                case JointType.HipRight: return "hip_right";
                case JointType.KneeLeft: return "knee_left";
                case JointType.KneeRight: return "knee_right";
                case JointType.ShoulderCenter: return "shoulder_center";
                case JointType.ShoulderLeft: return  "shoulder_left";
                case JointType.ShoulderRight: return "shoulder_right";
                case JointType.Spine: return "spine";
                case JointType.WristLeft: return "wrist_left";
                case JointType.WristRight: return "wrist_right";
                default: return "";
            }
        }

        private void sendSkeleton(JSObject kinectObj, Skeleton skel)
        {
            JSObject jointsObj = new JSObject();
            foreach (Joint joint in skel.Joints)
            {
                JSObject jointObj = new JSObject();
                jointObj["x"] = joint.Position.X;
                jointObj["y"] = joint.Position.Y;
                jointObj["z"] = joint.Position.Z;

                jointsObj[JointToString(joint.JointType)] = jointObj;
            }

            try
            {
                kinectObj.Invoke("onSkeleton", jointsObj);
            }
            catch (Exception)
            {
                ReportJSError("onSkeleton");
            }
        }

        private void sendGesture(JSObject kinectObj, Skeleton skel)
        {
            // Do gesture stuff
            Listening = ct.IsShouldersTowardsSensor(skel);
            
            Joint rightHand = skel.Joints.First(j => j.JointType == JointType.HandRight);
            Joint leftHand = skel.Joints.First(j => j.JointType == JointType.HandLeft);

            rhandDetect.Add(rightHand.Position, sensor);
            lhandDetect.Add(leftHand.Position, sensor);
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Is there any skeleton to play with?
            SkeletonFrame frame = e.OpenSkeletonFrame();
            if (frame == null) return;

            // Is there any Kinect object on the javascript?
            JSObject kinectObj = getJSKinectHandler();
            if (kinectObj == null) return;
            
            Skeleton[] skels = GetSkeletons(frame);
            
            Skeleton skel = skels.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
            if (skel == null)
            {
                return;
            }

            sendGesture(kinectObj, skel);  
            sendSkeleton(kinectObj, skel);          
        }

        private void CleanSensor()
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor = null;

                ct = null;
                rhandDetect = null;
                lhandDetect = null;
            }
        }
    }
}
