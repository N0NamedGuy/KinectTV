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

        private static Form debugForm;

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
                debugForm = new DebugForm();
                debugForm.Show();
                Application.Run(new MainForm());
            }
            catch (System.Reflection.TargetInvocationException)
            {

            }
        }

        public static void Notify(String s)
        {
            ((DebugForm)debugForm).Notify(s);
        }
        
    }
}
