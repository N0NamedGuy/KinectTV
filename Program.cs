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
        private static Form mainForm;


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
                mainForm = new MainForm();
                debugForm = new DebugForm();
                debugForm.Show(mainForm);

                Application.Run(mainForm);
            }
            catch (System.Reflection.TargetInvocationException)
            {

            }
        }

        public static void Notify(String s)
        {
            ((DebugForm)debugForm).Notify(s);
            debugForm.Refresh();
            //((MainForm)mainForm).Debug(s);
        }
        
    }
}
