using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Awesomium.Core;
using Awesomium.Windows.Forms;

namespace KinectTV
{
    public partial class MainForm : Form
    {
        public static Form form;
        static WebControl wb;

        static SpeechHelper sp;
        static KinectHelper kh;

        const String webPage = "file:///html/index.html";

        public MainForm()
        {
            InitializeComponent();
            InitBrowser();
        }

        private void InitBrowser()
        {
            wb = new WebControl();

            wb.Visible = true;
            wb.Dock = DockStyle.Fill;

            wb.DocumentReady += wb_DocumentReady;

            this.Controls.Add(wb);
        }

        void wb_DocumentReady(object sender, UrlEventArgs e)
        {
            using (JSObject app = wb.CreateGlobalJavascriptObject("App"))
            {
                app.Bind("exit", false, (s, e_) =>
                {
                    Application.Exit();
                    kh = null;
                    sp = null;
                });
            }
        }

        private void InitHelpers()
        {
            sp = new SpeechHelper(wb);
            kh = new KinectHelper(wb);
        }
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitHelpers();
            wb.Source = webPage.ToUri();
        }
    }
}
