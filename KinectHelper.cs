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
        LinearGestureDetector linearDetect;
        PositionDetector handAboveHead;

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
            wv.ProcessCreated += wv_ProcessCreated;

            linearDetect = new LinearGestureDetector();
            linearDetect.OnGestureDetected += linearDetect_OnGestureDetected;

            handAboveHead = new PositionDetector((skel) =>
            {
                Joint headJoint = skel.Joints.First((j) => j.JointType == JointType.Head);
                Joint handJoint = skel.Joints.First((j) => j.JointType == JointType.HandRight);
                
                return (handJoint.Position.Y - headJoint.Position.Y) > 0.02f;
            });
            handAboveHead.OnPositionDetected += handAboveHead_OnPositionDetected;
            
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

        void handAboveHead_OnPositionDetected(object sender, PositionEventArgs e)
        {
            JSObject kinectObj = getJSKinectHandler();
            if (kinectObj == null) return;

            try
            {
                kinectObj.Invoke("onPosition", e.TimeElapsed);
            }
            catch (Exception)
            {
                ReportJSError("onPosition");
            }
        }

        void wv_ProcessCreated(object sender, EventArgs e_)
        {
            Func<Boolean, JavascriptMethodEventArgs, Boolean> linearTrack = (track, e) =>
            {
                if (e.Arguments.Length < 2) return false;
                JSValue joint = e.Arguments[0];
                JSValue dir = e.Arguments[1];
                if (!(joint.IsString && dir.IsString)) return false;

                LinearGestureDetector.Direction eDir =
                    (LinearGestureDetector.Direction)
                    Enum.Parse(typeof(LinearGestureDetector.Direction), dir.ToString(), true);

                try
                {
                    if (track) linearDetect.AddDirection(StringToJoint(e.Arguments[0]), eDir);
                    else linearDetect.RemoveDirection(StringToJoint(e.Arguments[0]), eDir);
                }
                catch (Exception) { return false; }
                return true;
            };

            kinectGlobalObj = wv.CreateGlobalJavascriptObject("KinectHelper");

            kinectGlobalObj.Bind("userIsFacing", true, (s, e) => e.Result = Listening);

            kinectGlobalObj.Bind("linearTrack", true, (s, e) => e.Result = linearTrack(true, e));
            kinectGlobalObj.Bind("linearUntrack", false, (s, e) => e.Result = linearTrack(false, e));
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
                linearDetect.Clear();
            }
        }

        void linearDetect_OnGestureDetected(object sender, GestureEventArgs e)
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
                case JointType.AnkleLeft:       return "ankle_left";
                case JointType.AnkleRight:      return "ankle_right";
                case JointType.ElbowLeft:       return "elbow_left";
                case JointType.ElbowRight:      return "elbow_right";
                case JointType.FootLeft:        return "foot_left";
                case JointType.FootRight:       return "foot_right";
                case JointType.HandLeft:        return "hand_left";
                case JointType.HandRight:       return "hand_right";
                case JointType.Head:            return "head";
                case JointType.HipCenter:       return "hip_center";
                case JointType.HipLeft:         return "hip_left";
                case JointType.HipRight:        return "hip_right";
                case JointType.KneeLeft:        return "knee_left";
                case JointType.KneeRight:       return "knee_right";
                case JointType.ShoulderCenter:  return "shoulder_center";
                case JointType.ShoulderLeft:    return  "shoulder_left";
                case JointType.ShoulderRight:   return "shoulder_right";
                case JointType.Spine:           return "spine";
                case JointType.WristLeft:       return "wrist_left";
                case JointType.WristRight:      return "wrist_right";
                default: return "";
            }
        }

        JointType StringToJoint(String type)
        {
            switch (type)
            {
                case "ankle_left":      return JointType.AnkleLeft; 
                case "ankle_right":     return JointType.AnkleRight;     
                case "elbow_left":      return JointType.ElbowLeft;    
                case "elbow_right":     return JointType.ElbowRight;     
                case "foot_left":       return JointType.FootLeft;       
                case "foot_right":      return JointType.FootRight;      
                case "hand_left":       return JointType.HandLeft;       
                case "hand_right":      return JointType.HandRight;      
                case "head":            return JointType.Head;
                case "hip_center":      return JointType.HipCenter;      
                case "hip_left":        return JointType.HipLeft;    
                case "hip_right":       return JointType.HipRight;       
                case "knee_left":       return JointType.KneeLeft;        
                case "knee_right":      return JointType.KneeRight;      
                case "shoulder_center": return JointType.ShoulderCenter; 
                case "shoulder_left":   return JointType.ShoulderLeft;   
                case "shoulder_right":  return JointType.ShoulderRight;  
                case "spine":           return JointType.Spine;
                case "wrist_left":      return JointType.WristLeft;
                case "wrist_right":     return JointType.WristRight;     
                default: throw new Exception("Unknown joint");
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
            if (skel == null) return;

            handAboveHead.Update(skel);
            linearDetect.Update(skel);
            sendSkeleton(kinectObj, skel);          
        }

        private void CleanSensor()
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor = null;

                ct = null;
            }
        }
    }
}
