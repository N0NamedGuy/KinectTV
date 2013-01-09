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
        public static Form form;
        static WebControl wb;

        static SpeechHelper sp;
        static KinectHelper kh;
        
        const String webPage = "file:///html/index.html";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(GetForm());
        }

        public static void Notify(String s)
        {
            //wb.ExecuteJavascript("debug('" + s + "');");
            form.Text = s;
        }
        
        static WebControl GetBrowser()
        {
            WebControl wb;
            wb = new WebControl();

            wb.Source = webPage.ToUri();
            wb.Visible = true;
            wb.Dock = DockStyle.Fill;

            return wb;
        }
        
        static Form GetForm()
        {
            form = new Form();
            wb = GetBrowser();

            form.Controls.Add(wb);
            form.Size = new Size(1280, 720);
            
            form.Load += new EventHandler(form_Load);
            form.FormClosing += new FormClosingEventHandler(form_FormClosing);

            return form;   
        }

        static void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        static void form_Load(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            wb.DocumentReady += new UrlEventHandler(wb_DocumentReady);
        }

        static void wb_DocumentReady(object sender, UrlEventArgs e)
        {
            kh = new KinectHelper(wb);
            sp = new SpeechHelper(wb);

            using (JSObject app = wb.CreateGlobalJavascriptObject("App"))
            {
                app.Bind("exit", false, (s, e_) =>
                {
                    Application.Exit();
                });
            }
        }
    }
}
