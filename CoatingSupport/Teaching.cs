using CoatingSupport.Entities;
using CoatingSupport.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CoatingSupport.Utilities.Common;

namespace CoatingSupport
{
    public partial class frmTeaching : MetroFramework.Forms.MetroForm
    {
        private List<Position> lstPosition = new List<Position>();
        private SerialPort comControl = new SerialPort();
        private int STEP = 0;
        private Tuple<int, int, int> data;
        public frmTeaching()
        {
            InitializeComponent();
        }

        private void frmSetup_Load(object sender, EventArgs e)
        {
            comControl.DataReceived += new SerialDataReceivedEventHandler(ComControl_DataReceived);
            CheckForIllegalCrossThreadCalls = false;

            IsSpray();
            // Sigle SETUP
            Common.SendData(comControl, "#SETUP*");
            Thread.Sleep(100);
            Common.SendData(comControl, string.Format("#T{0:0000}*", nrMain.Value));
            btnClear.Enabled = false;
        }
        private void ComControl_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ReceicedTex(comControl.ReadExisting());
        }
        delegate void SetTextCallback(string text);
        public void ReceicedTex(string text)
        {
            if (txtRecei.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ReceicedTex);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                txtRecei.Text += text;
            }
        }
        public void Setcom(SerialPort port)
        {
            this.comControl = port;
        }
        private void btnZ1_Click(object sender, EventArgs e)
        {
            btnZ1.BackColor = btnZ1.BackColor == SystemColors.Control ? Color.Crimson : SystemColors.Control;
            if (btnZ1.BackColor == Color.Crimson)
            {
                btnZ2.BackColor = SystemColors.Control;
                btnZ2.Enabled = false;
                Common.SendData(comControl, Const.Z1_ON);
            }
            else
            {
                btnZ2.Enabled = true;
                Common.SendData(comControl, Const.Z1_OFF);
            }
            IsSpray();
        }

        private void btnZ2_Click(object sender, EventArgs e)
        {
            btnZ2.BackColor = btnZ2.BackColor == SystemColors.Control ? Color.Crimson : SystemColors.Control;
            if (btnZ2.BackColor == Color.Crimson)
            {
                btnZ1.BackColor = SystemColors.Control;
                btnZ1.Enabled = false;
                Common.SendData(comControl, Const.Z2_ON);
            }
            else
            {
                btnZ1.Enabled = true;
                Common.SendData(comControl, Const.Z2_OFF);
            }
            IsSpray();
        }

        private void btnXTop_MouseDown(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.X_INCREMENT);
            btnXTop.BackColor = Color.PeachPuff;
        }

        private void btnXTop_MouseUp(object sender, MouseEventArgs e)
        {
            // STOP
            Common.SendData(comControl, Const.X_STOP);
            btnXTop.BackColor = SystemColors.Control;
        }

        private void btnXBottom_MouseDown(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.X_DECREASE);
            btnXBottom.BackColor = Color.PeachPuff;
        }

        private void btnXBottom_MouseUp(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.X_STOP);
            btnXBottom.BackColor = SystemColors.Control;
        }

        private void btnYRight_MouseDown(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.Y_INCREMENT);
            btnYRight.BackColor = Color.PeachPuff;
        }

        private void btnYRight_MouseUp(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.Y_STOP);
            btnYRight.BackColor = SystemColors.Control;
        }

        private void btnYLeft_MouseDown(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.Y_DECREASE);
            btnYLeft.BackColor = Color.PeachPuff;
        }

        private void btnYLeft_MouseUp(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.Y_STOP);
            btnYLeft.BackColor = SystemColors.Control;
        }

        private void btnZTop_MouseDown(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.Z_INCREMENT);
            btnZTop.BackColor = Color.PeachPuff;
        }

        private void btnZTop_MouseUp(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.Z_STOP);
            btnZTop.BackColor = SystemColors.Control;
        }

        private void btnZBottom_MouseDown(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.Z_DECREASE);
            btnZBottom.BackColor = Color.PeachPuff;
        }

        private void btnZBottom_MouseUp(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, Const.Z_STOP);
            btnZBottom.BackColor = SystemColors.Control;
        }

        private void txtRecei_TextChanged(object sender, EventArgs e)
        {
            Regex pattern1 = new Regex(@"^[#]+$");
            Regex pattern2 = new Regex(@"^[#]+[\d]+$");
            Regex pattern = new Regex(@"^[#]+[\d{15}]+[*]$");
            string result = txtRecei.Text.Trim();
            if (pattern1.IsMatch(result) || pattern2.IsMatch(result) || pattern.IsMatch(result))
            {
                if (result.Length == 17)
                {
                    if (pattern.IsMatch(result))
                    {
                        data = new Tuple<int, int, int>(Convert.ToInt32(result.Substring(1, 5)), Convert.ToInt32(result.Substring(6, 5)), Convert.ToInt32(result.Substring(11, 5)));
                        lblX.Text = string.Format("{0:0.000} mm", Common.Pulse2Millimetter(data.Item1, Const.D1));
                        lblY.Text = string.Format("{0:0.000} mm", Common.Pulse2Millimetter(data.Item2, Const.D2));
                        lblZ.Text = string.Format("{0:0.000} mm", Common.Pulse2Millimetter(data.Item3, Const.D3));
                        Thread.Sleep(500);
                        txtRecei.Clear();
                    }
                    else
                    {
                        txtRecei.Clear();
                    }
                }
            }
            else
            {
                txtRecei.Clear();
            }
        }
        public void IsSpray()
        {
            chkSpray.Enabled = btnZ1.BackColor == Color.Crimson || btnZ2.BackColor == Color.Crimson ? true : false;
        }


        private void btnMotor_Click(object sender, EventArgs e)
        {
            if (btnMotor.Text.Equals("OFF"))
            {
                btnMotor.Text = "ON";
                btnMotor.BackColor = Color.Blue;
                Common.SendData(comControl, Const.MOTOR_ON);
            }
            else
            {
                btnMotor.Text = "OFF";
                btnMotor.BackColor = Color.Red;
                Common.SendData(comControl, Const.MOTOR_OFF);
            }
        }
        private void btnStopper_Click(object sender, EventArgs e)
        {
            if (btnStopper.Text.Equals("OFF"))
            {
                btnStopper.Text = "ON";
                btnStopper.BackColor = Color.Blue;
                Common.SendData(comControl, Const.STOPPER_ON);
            }
            else
            {
                btnStopper.Text = "OFF";
                btnStopper.BackColor = Color.Red;
                Common.SendData(comControl, Const.STOPPER_OFF);
            }
        }

        private void btnHolder_Click(object sender, EventArgs e)
        {
            if (btnHolder.Text.Equals("OFF"))
            {
                btnHolder.Text = "ON";
                btnHolder.BackColor = Color.Blue;
                Common.SendData(comControl, Const.HOLDER_ON);
            }
            else
            {
                btnHolder.Text = "OFF";
                btnHolder.BackColor = Color.Red;
                Common.SendData(comControl, Const.HOLDER_OFF);
            }
        }



        private void nrMain_ValueChanged(object sender, EventArgs e)
        {
            trMain.Value = Convert.ToInt32(nrMain.Value);
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Const.pathModel;
            sfd.FileName = "*.csv";
            sfd.Filter = "csv File(.csv)|*.csv|All files(*.*)|*.*";
            sfd.Title = "Save model ...";
            if (sfd.ShowDialog() == DialogResult.OK) Common.WriteCSV(sfd.FileName, lstPosition);
        }

        private void trMain_ValueChanged(object sender, EventArgs e)
        {
            nrMain.Value = trMain.Value;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                btnClear.Enabled = true;
                STEP++;
                lstPosition.Add(new Position()
                {
                    step = STEP,
                    x = data.Item1,
                    y = data.Item2,
                    z = data.Item3,
                    delay = Convert.ToInt32(nrDelay.Value),
                    speed = Convert.ToInt32(nrSpeed.Value),
                    spray = chkSpray.Checked ? "1" : "0",
                    up = Common.Millimetter2Pulse(nrUp.Value.ToString(), Const.D3),
                    z1 = btnZ1.BackColor == SystemColors.Control ? "0" : "1",
                    z2 = btnZ2.BackColor == SystemColors.Control ? "0" : "1"
                });
                lblStep.Text = STEP.ToString();
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                lstPosition.RemoveAt(lstPosition.Count - 1);
                btnClear.Enabled = false;
                STEP--;
                lblStep.Text = STEP.ToString();

            }
        }

        private void btnOrigin_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, Const.ORIGIN);
        }

        private void btnSpeedTeaching_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, string.Format("#T{0:0000}*", nrMain.Value));
        }

        private void frmTeaching_FormClosed(object sender, FormClosedEventArgs e)
        {
            comControl.DataReceived -= new SerialDataReceivedEventHandler(ComControl_DataReceived);
        }
    }
}
