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

namespace CoatingSupport
{
    public partial class frmSetup : MetroFramework.Forms.MetroForm
    {
        private Position position = new Position();
        private List<Position> lstPosition = new List<Position>();
        private string modelPath = Path.Combine(Application.StartupPath, "model");
        private SerialPort comControl = new SerialPort();
        private string pathConfig = Application.StartupPath + "\\config.xml";
        private string inputData = string.Empty;
        private Tuple<int, int, int> data;
        public frmSetup()
        {
            InitializeComponent();
            comControl.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
        }
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            inputData = comControl.ReadExisting();
            if (inputData != String.Empty)
            {
                ReceicedTex(inputData);
            }
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
        }


        private void frmSetup_Load(object sender, EventArgs e)
        {
            Common.SendData(comControl, "#WAITPOINT*");
            Thread.Sleep(100);
            Common.SendData(comControl, "#T" + nrMain.Value.ToString("0000") + "*");
        }

        public void Init(SerialPort comControl)
        {
            this.comControl = comControl;
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

        private void chkWait_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWait.Checked)
            {
                chkXa1.Checked = chkXa2.Checked = chkTest.Checked = false;
            }
        }

        private void chkXa1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkXa1.Checked)
            {
                chkXa2.Checked = chkWait.Checked = chkTest.Checked = false;
            }
        }

        private void chkXa2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkXa2.Checked)
            {
                chkXa1.Checked = chkWait.Checked = chkTest.Checked = false;
            }
        }

        private void chkTest_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTest.Checked)
            {
                chkXa1.Checked = chkWait.Checked = chkXa2.Checked = false;
            }
        }

        private void nrMain_ValueChanged(object sender, EventArgs e)
        {
            trMain.Value = Convert.ToInt32(nrMain.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, "#P" + nrPcb.Value.ToString("000") + "*");
            Console.WriteLine("#P" + nrPcb.Value.ToString("000") + "*");
        }

        private void btnOrigin_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, Const.ORIGIN);
        }

        private void btnSaves_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("#");
                builder.Append("X");
                builder.Append(data.Item1);
                builder.Append("Y");
                builder.Append(data.Item2);
                builder.Append("Z");
                builder.Append(data.Item3);
                if (chkWait.Checked)
                {
                    builder.Append("W");
                }
                else if (chkXa1.Checked)
                {
                    builder.Append("U");
                }
                else if (chkXa2.Checked)
                {
                    builder.Append("V");
                }
                else if (chkTest.Checked)
                {
                    builder.Append("T");
                }
                else
                {
                    MessageBox.Show("Errors");
                    return;
                }
                builder.Append("*");
                Common.SendData(comControl, builder.ToString());
                Console.WriteLine("Send: " + builder);
            }
        }

        private void trMain_Scroll(object sender, ScrollEventArgs e)
        {
            nrMain.Value = trMain.Value;
        }

        private void btnWait_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, Const.WAIT);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, Const.TEST);
        }

        private void btnXa_1_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, Const.XA_1);
        }

        private void btnXa_2_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, Const.XA_2);
        }

        private void btnSpeedTeaching_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, "#T" + nrMain.Value.ToString("0000") + "*");
        }


        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, "#E*");
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            Common.SendData(comControl, "#S*");
        }

        private void frmSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            comControl.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
        }

    }
}
