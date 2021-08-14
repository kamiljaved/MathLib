using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mlgui
{
    public partial class Splash : Form
    {
        private delegate void CloseDelegate();
        private static Splash splashForm;

        public Splash()
        {
            InitializeComponent();
        }

        static public void ShowSplashScreen()
        {
            // Make sure it is only launched once.

            if (splashForm != null)
                return;
            Thread thread = new Thread(new ThreadStart(ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            Thread.Sleep(1000);
        }

        static private void ShowForm()
        {
            splashForm = new Splash();
            Application.Run(splashForm);
        }

        static public void CloseForm()
        {
            //Thread.Sleep(10000);
            splashForm.Invoke(new CloseDelegate(Splash.CloseFormInternal));
        }

        static private void CloseFormInternal()
        {
            
            splashForm.Close();
            
            splashForm = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(10);
            if (progressBar1.Value == 100)
            {
                timer1.Stop();
            }
        }
    }
}
