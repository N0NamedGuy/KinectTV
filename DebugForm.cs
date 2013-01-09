using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KinectTV
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        public void Notify(string text)
        {
            debugBox.Text += DateTime.Now + "> " + text + "\r\n";
            debugBox.SelectionStart = debugBox.Text.Length;
            debugBox.ScrollToCaret();
        }

    }
}
