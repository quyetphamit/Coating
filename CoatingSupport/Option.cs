using System;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

using System.Management;
using System.Management.Instrumentation;
using CoatingSupport.Entities;
using System.Collections.Generic;
using System.IO;

namespace CoatingSupport
{
    public partial class frmOption : MetroFramework.Forms.MetroForm
    {
        public frmOption()
        {
            InitializeComponent();
        }
        private SystemSetting setting;
        private string pathConfig = Path.Combine(Application.StartupPath, "config.xml");
        private void frmSetup_Load(object sender, EventArgs e)
        {
            SystemSetting.ReadXML<SystemSetting>(out setting, pathConfig);
            LoadComponent();
        }
        public void LoadComponent()
        {
            cbbComControl.DataSource = SerialPort.GetPortNames().ToList();
            cbbBaudRate.DataSource = new List<string>() { "4800", "9600", "19200", "56000", "115200" };
            cbbDataBits.DataSource = new List<string>() { "8", "9" };
            cbbParity.DataSource = new List<string>() { "None", "Odd", "Even", "Mark", "Space" };
            cbbStopBits.DataSource = new List<string>() { "0", "1", "1.5", "2" };
            tableLayoutPanel2.Enabled = false;
            // set value
            cbbComControl.Text = setting.portName;
            cbbBaudRate.Text = setting.baudRate.ToString();
            cbbDataBits.Text = setting.dataBits.ToString();
            cbbParity.Text = setting.parity;
            cbbStopBits.Text = setting.stopBits;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanel2.Enabled = chkAdvanced.Checked ? true : false;
        }

        private void btnSaves_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                setting.baudRate = Convert.ToInt32(cbbBaudRate.Text);
                setting.portName = cbbComControl.Text;
                setting.dataBits = Convert.ToInt32(cbbDataBits.Text);
                setting.parity = cbbParity.Text;
                setting.stopBits = cbbStopBits.Text;
                SystemSetting.WriteXML<SystemSetting>(setting, pathConfig);
                Application.Restart();
                this.Close();
            }
        }

    }
}
