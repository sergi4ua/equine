/*Copyright(C) 2018 Sergi4UA

This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace EQUINE
{
    public partial class frmPing : Form
    {
        Ping pingSender = new Ping();
        string errMsg = "No errors.";
        AutoResetEvent waiter;

        public frmPing()
        {
            InitializeComponent();
            
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                SetText("Ping failed! Click Show Info for details...");
                System.Media.SystemSounds.Hand.Play();
                errMsg = e.Error.ToString();
            }

            PingReply reply = e.Reply;
            if (reply != null)
            {
                if (reply.Status == IPStatus.Success)
                {
                    SetText("Success! Remote host is visible from your PC! Time: " + reply.RoundtripTime + " ms");
                    System.Media.SystemSounds.Exclamation.Play();
                    errMsg = "Ping info:\n" + "TripTime: " + reply.RoundtripTime + "\nBuffer size: " + reply.Buffer.Length + "\nLifeTime: " + reply.Options.Ttl;
           
                }
                else
                {
                    SetText("Ping failed! No reply.");
                    System.Media.SystemSounds.Hand.Play();
                }
            }
            ((AutoResetEvent)e.UserState).Set();
            DisablePingButton(true);
            DisableTextInput(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("IP/Remotehost field must not be empty.", "");
                return;
            }
            backgroundWorker1.RunWorkerAsync();
        }

        private void frmPing_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(errMsg, "Ping Log", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        delegate void SetTextCallback(string text);
        delegate void DisableButton(bool dis);
        delegate void iDisableTextInput(bool dis);

        private void SetText(string text)
        {
            if (label3.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                label3.Text = text;
            }
        }

        private void DisablePingButton(bool ornot)
        {
            if(button1.InvokeRequired)
            {
                DisableButton d = new DisableButton(DisablePingButton);
                Invoke(d, new object[] { ornot });
            }
            else
            {
                button1.Enabled = ornot;
            }
        }

        private void DisableTextInput(bool ornot)
        {
            if (button1.InvokeRequired)
            {
                iDisableTextInput d = new iDisableTextInput(DisableTextInput);
                Invoke(d, new object[] { ornot });
            }
            else
            {
                textBox1.Enabled = ornot;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DisablePingButton(false);
            DisableTextInput(false);
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
            waiter = new AutoResetEvent(false);

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 12000;
            PingOptions options = new PingOptions(64, true);

            pingSender.SendAsync(textBox1.Text, timeout, buffer, options, waiter);
            SetText("Pinging...");
            waiter.WaitOne();
        }
    }
}
