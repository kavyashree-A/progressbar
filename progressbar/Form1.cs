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

namespace progressbar
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cts; // nullable CTS
        public Form1()
        {
            // Form settings
            this.Text = "Progress Bar with Reverse Cancel";
            this.Width = 420;
            this.Height = 220;

            // Progress bar
            ProgressBar progressBar = new ProgressBar();
            progressBar.Name = "progressBar1";
            progressBar.Width = 300;
            progressBar.Height = 30;
            progressBar.Left = 40;
            progressBar.Top = 30;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Maximum = 100;
            progressBar.Minimum = 0;

            // Label
            Label lblStatus = new Label();
            lblStatus.Name = "lblStatus";
            lblStatus.Text = "Progress: 0%";
            lblStatus.Top = 70;
            lblStatus.Left = 40;
            lblStatus.Width = 300;

            // Start Button
            Button btnStart = new Button();
            btnStart.Text = "Start";
            btnStart.Top = 120;
            btnStart.Left = 40;

            // Cancel Button
            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Top = 120;
            btnCancel.Left = 120;
            btnCancel.Enabled = false;

            // Start button click
            btnStart.Click += async (s, e) =>
            {
                cts = new CancellationTokenSource();
                btnStart.Enabled = false;
                btnCancel.Enabled = true;
                progressBar.Value = 0;
                lblStatus.Text = "Progress: 0%";

                try
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        if (cts.Token.IsCancellationRequested)
                        {
                            lblStatus.Text = "Cancelling...";
                            // Reverse progress bar
                            for (int j = progressBar.Value; j >= 0; j--)
                            {
                                progressBar.Value = j;
                                lblStatus.Text = $"Cancelling... {j}%";
                                await Task.Delay(20); // smooth reverse
                            }
                            throw new OperationCanceledException();
                        }

                        await Task.Delay(50); // simulate work
                        progressBar.Value = i;
                        lblStatus.Text = $"Progress: {i}%";
                    }

                    MessageBox.Show("Task Completed!", "Info",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (OperationCanceledException)
                {
                    lblStatus.Text = "Progress Cancelled!";
                    MessageBox.Show("Task Cancelled!", "Info",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    btnStart.Enabled = true;
                    btnCancel.Enabled = false;
                }
            };

            // Cancel button click
            btnCancel.Click += (s, e) =>
            {
                cts?.Cancel();
            };

            // Add controls
            this.Controls.Add(progressBar);
            this.Controls.Add(lblStatus);
            this.Controls.Add(btnStart);
            this.Controls.Add(btnCancel);
        }
    }
}
