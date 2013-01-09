using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

using Awesomium.Core;
using Awesomium.Windows.Forms;

namespace KinectTV
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new MainForm());
            }
            catch (System.Reflection.TargetInvocationException)
            {

            }
        }

        public static void Notify(String s)
        {
        }
        
    }
}
