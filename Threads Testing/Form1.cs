using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Threads_Testing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public struct backgroundStuff
        {
            public string message; // The message to send to the other thread
            public int times; // the amount of times to repeat the for loop (has it was some kind of load)
        }

        private backgroundStuff Background;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string message = ((backgroundStuff)e.Argument).message;
            int times = ((backgroundStuff)e.Argument).times;
            int j = 1;
            try
            {
                MessageBox.Show(string.Format("This got here: \"{0}\"", message));
                for (int i = 0; i < times; i++)
                {
                    if (backgroundWorker1.CancellationPending) // checks if the user asked to cancel the opertation
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        backgroundWorker1.ReportProgress(j++ * 100 / times, string.Format("Process Data {0}", i)); // reports the progress back to the backgroundWorker1_ProgressChanged
                        Thread.Sleep(10); // simulates actual work
                    }
                }
            }catch(Exception ex)
            {
                backgroundWorker1.CancelAsync();
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            pglbl.Text = string.Format("Processing {0}%", e.ProgressPercentage);
            progressBar1.Update();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error");
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Cancelled");
            }
            else
            {
                MessageBox.Show("Done");
            }
            button1.Enabled = true;
            button2.Enabled = false;
            textBox1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                Background.message = textBox1.Text;
                Background.times = 1200;
                backgroundWorker1.RunWorkerAsync(Background);
                button1.Enabled = false;
                button2.Enabled = true;
                textBox1.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
        }
    }
}

/**
 * Sources:
 * C# Tutorial - BackgroudWorker | FoxLearn, https://www.youtube.com/watch?v=G3zRhhGCJJA, in 27/06/2020
 * BackgroundWorker.RunWorkerCompleted Event (System.ComponentModel) | Microsoft Docs, https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.backgroundworker.runworkercompleted?view=netcore-3.1, in 27/06/2020
 * backgroundworker - C#- background worker's CancelAsync() not working? - Stack Overflow, https://stackoverflow.com/questions/18314873/c-background-workers-cancelasync-not-working, in 27/06/2020
 */
