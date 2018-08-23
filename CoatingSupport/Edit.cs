using CoatingSupport.Entities;
using CoatingSupport.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    public partial class frmEdit : MetroFramework.Forms.MetroForm
    {
        private SerialPort comControl = new SerialPort();
        private string inputData = string.Empty;
        private List<Position> lstPosition = new List<Position>();
        private string pathCsv = string.Empty;
        private Position position = new Position();
        private int step = 0;
        private string arraySend = string.Empty;
        private Tuple<int, int, int> data;

        public void SaveDuplicate(string input, double d, string mm)
        {
            int duplicate = Common.Millimetter2Pulse(mm, d);
            List<Position> positionDuplicateX = new List<Position>();
            if (lstPosition.Any(r => r.area != "1")) lstPosition = lstPosition.Where(r => r.area == "1").ToList() as List<Position>;
            positionDuplicateX = input.Contains("X")
                ? lstPosition.Select(r => new Position { step = r.step, x = r.x + duplicate, y = r.y, z = r.z, delay = r.delay, speed = r.speed, spray = r.spray, up = r.up, z1 = r.z1, z2 = r.z2, area = input }).ToList()
                : lstPosition.Select(r => new Position { step = r.step, x = r.x, y = r.y + duplicate, z = r.z, delay = r.delay, speed = r.speed, spray = r.spray, up = r.up, z1 = r.z1, z2 = r.z2, area = input }).ToList();
            lstPosition.AddRange(positionDuplicateX);
            Common.WriteCSV(pathCsv, lstPosition);
        }

        public frmEdit()
        {
            InitializeComponent();
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

        private void frmEdit_Load(object sender, EventArgs e)
        {
            Init();
            comControl.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            Common.SendData(comControl, "#EDIT*");
            Thread.Sleep(100);
            // #T0100*
            Common.SendData(comControl, string.Format("#T{0}*", nrMain.Value.ToString("0000")));
        }
        public void Init()
        {
            cbbModel.DataSource = Directory.GetFiles(Const.pathModel, "*.csv").Select(Path.GetFileNameWithoutExtension).ToList();
            if (dgvStep.RowCount > 0)
            {
                var rowSelect = dgvStep.SelectedCells[0].Value;
                position = lstPosition.FirstOrDefault(r => r.step.Equals(rowSelect));
            }
            btnZ1.Enabled = btnZ2.Enabled = false;
            btnXTop.Enabled = btnXBottom.Enabled = btnYLeft.Enabled = btnYRight.Enabled = btnZTop.Enabled = btnZBottom.Enabled = false;
            chkAdd.Enabled = chkEdit.Enabled = btnAdd.Enabled = false;
            chkAdd.Checked = chkEdit.Checked = false;
            btnZ1.BackColor = btnZ2.BackColor = SystemColors.Control;
            btnSaves.Enabled = false;
        }
        public void SetCom(SerialPort comControl)
        {
            this.comControl = comControl;
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

        private void chkAdd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAdd.Checked)
            {
                btnXTop.Enabled = btnXBottom.Enabled = btnYLeft.Enabled = btnYRight.Enabled = btnZTop.Enabled = btnZBottom.Enabled = true;
                btnZ1.Enabled = btnZ2.Enabled = true;
                btnAdd.Enabled = true;
            }
            else
            {
                btnXTop.Enabled = btnXBottom.Enabled = btnYLeft.Enabled = btnYRight.Enabled = btnZTop.Enabled = btnZBottom.Enabled = false;
                btnZ1.Enabled = btnZ2.Enabled = false;
                btnAdd.Enabled = false;
            }
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

        private void trMain_ValueChanged(object sender, EventArgs e)
        {
            nrMain.Value = trMain.Value;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                //Position p = new Position()
                //{
                //    x = data.Item1,
                //    y = data.Item2,
                //    z = data.Item3,
                //    delay = Convert.ToInt32(nrDelay.Value),
                //    speed = Convert.ToInt32(nrSpeed.Value),
                //    spray = chkSpray.Checked ? "1" : "0",
                //    up = chkUp.Checked ? "1" : "0",
                //    z1 = btnZ1.BackColor == SystemColors.Control ? "0" : "1",
                //    z2 = btnZ2.BackColor == SystemColors.Control ? "0" : "1"
                //};
                Position p = data == null
                    ? position
                    : new Position()
                    {
                        x = data.Item1,
                        y = data.Item2,
                        z = data.Item3,
                        delay = Convert.ToInt32(nrDelay.Value),
                        speed = Convert.ToInt32(nrSpeed.Value),
                        spray = chkSpray.Checked ? "1" : "0",
                        up = Common.Millimetter2Pulse(nrUp.Value.ToString(), Const.D3),
                        z1 = btnZ1.BackColor == SystemColors.Control ? "0" : "1",
                        z2 = btnZ2.BackColor == SystemColors.Control ? "0" : "1"
                    };
                lstPosition.Insert(position.step++, p);
                step = position.step;
                Common.WriteCSV(Path.Combine(Const.pathModel, cbbModel.Text + ".csv"), lstPosition);
                LoadPosition();
                btnSaves.Enabled = lstPosition.Count % 2 == 0 ? true : false;
                dgvStep.Rows[step - 1].Selected = true;
            }
        }

        private void btnSpeedTeaching_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, string.Format("#T{0:0000}*", nrMain.Value));
        }

        private void btnOrigin_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, Const.ORIGIN);
        }

        private void cbbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPosition();
        }
        public void LoadPosition()
        {
            pathCsv = Path.Combine(Const.pathModel, cbbModel.Text + ".csv");
            lstPosition = Common.ReadCsv(pathCsv);
            dgvStep.DataSource = lstPosition.Select(r => new { step = r.step, x = Common.Pulse2Millimetter(r.x, Const.D1), y = Common.Pulse2Millimetter(r.y, Const.D2), z = Common.Pulse2Millimetter(r.z, Const.D3), origin = r.area }).ToList();
        }
        private void btnSaves_Click(object sender, EventArgs e)
        {
            lstPosition[position.step - 1] = data != null ? new Position()
            {
                x = data.Item1,
                y = data.Item2,
                z = data.Item3,
                speed = Convert.ToInt32(nrSpeed.Value),
                delay = Convert.ToInt32(nrDelay.Value),
                z1 = btnZ1.BackColor == SystemColors.Control ? "0" : "1",
                z2 = btnZ2.BackColor == SystemColors.Control ? "0" : "1",
                spray = chkSpray.Checked ? "1" : "0",
                up = Common.Millimetter2Pulse(nrUp.Value.ToString(),Const.D3)
            } : new Position()
            {
                x = position.x,
                y = position.y,
                z = position.z,
                speed = Convert.ToInt32(nrSpeed.Value),
                delay = Convert.ToInt32(nrDelay.Value),
                z1 = btnZ1.BackColor == SystemColors.Control ? "0" : "1",
                z2 = btnZ2.BackColor == SystemColors.Control ? "0" : "1",
                spray = chkSpray.Checked ? "1" : "0",
                up = Common.Millimetter2Pulse(nrUp.Value.ToString(), Const.D3)
            };
            //SaveFileDialog sfd = new SaveFileDialog()
            //{
            //    InitialDirectory = modelPath,
            //    FileName = cbbModel.SelectedValue + ".csv",
            //    Filter = "csv File(.csv)|*.csv|All files(*.*)|*.*",
            //    Title = "Save model ..."
            //};
            //if (sfd.ShowDialog() == DialogResult.OK) Common.WriteCSV(sfd.FileName, lstPosition);
            if (MessageBox.Show("Are you save model?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Common.WriteCSV(Path.Combine(Const.pathModel, cbbModel.Text + ".csv"), lstPosition);
            LoadPosition();
            dgvStep.Rows[position.step - 1].Selected = true; // Set vị trí dòng đang được chọn
            chkAdd.Checked = false;
        }
        //public bool ValidateData()
        //{
        //    Regex pattern = new Regex(@"^[\d]$");
        //    Tuple<double, double, double> valid = new Tuple<double, double, double>(Convert.ToDouble(65000 * Const.D1 / 4000), Convert.ToDouble(65000 * Const.D2 / 4000), Convert.ToDouble(65000 * Const.D3) / 4000);
        //    try
        //    {
        //        var x = Convert.ToDouble(lblX.Text);
        //        if (x < 0)
        //        {
        //            MessageBox.Show("X greater than 0", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            return false;
        //        }
        //        if (x > valid.Item1)
        //        {
        //            MessageBox.Show(string.Format("X less than {0:0.000}", valid.Item1.ToString()), "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            return false;
        //        }
        //    }
        //    catch (FormatException)
        //    {
        //        MessageBox.Show(txtX.Text + " not number!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return false;
        //    }
        //    // Validate Y
        //    try
        //    {
        //        var y = Convert.ToDouble(txtY.Text.Trim());
        //        if (y < 0)
        //        {
        //            MessageBox.Show("Y greater than 0", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            return false;
        //        }
        //        if (y > valid.Item2)
        //        {
        //            MessageBox.Show(string.Format("Y less than {0:0.000}", valid.Item2.ToString()), "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            return false;
        //        }
        //    }
        //    catch (FormatException)
        //    {
        //        MessageBox.Show(txtY.Text + " not number!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return false;
        //    }
        //    // Validate Z
        //    try
        //    {
        //        var z = Convert.ToDouble(txtZ.Text.Trim());
        //        if (z < 0)
        //        {
        //            MessageBox.Show("Z greater than 0", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            return false;
        //        }
        //        if (z > valid.Item3)
        //        {
        //            MessageBox.Show(string.Format("Z less than {0:0.000}", valid.Item3.ToString()), "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            return false;
        //        }
        //    }
        //    catch (FormatException)
        //    {
        //        MessageBox.Show(txtY.Text + " not number!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return false;
        //    }
        //    return true;
        //}
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (step > 0)
                {
                    lstPosition.RemoveAt(step - 1);
                    Common.WriteCSV(Path.Combine(Const.pathModel, cbbModel.Text + ".csv"), lstPosition);
                    btnSaves.Enabled = lstPosition.Count % 2 == 0 ? true : false;
                    LoadPosition();
                }
            }
        }

        private void btnOpenModel_Click(object sender, EventArgs e)
        {
            Process openModel = new Process();
            openModel.StartInfo.FileName = Const.pathModel;
            openModel.Start();
        }

        private void dgvStep_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (dgvStep.Rows[e.RowIndex].Cells[4].Value.ToString() == "1")
                {
                    int rowSelect = Convert.ToInt32(dgvStep.Rows[e.RowIndex].Cells[0].Value);
                    step = Convert.ToInt32(dgvStep.Rows[e.RowIndex].Cells[0].Value);
                    position = lstPosition.FirstOrDefault(r => r.step.Equals(rowSelect));
                    // =====> #001X12345Y12345Z12345Z00S0500D0000S0U0000*
                    arraySend = String.Format("#{0:D3}X{1:D5}Y{2:D5}Z{3:D5}Z{4}{5}S{6:D4}D{7:D4}S{8}U{9:D4}*", position.step, position.x, position.y, position.z, position.z1, position.z2, position.speed, position.delay, position.spray, position.up);
                    Console.WriteLine(arraySend);
                    chkAdd.Enabled = rowSelect % 2 == 0 ? true : false;
                    lblX.Text = string.Format("{0:0.000} mm", dgvStep.Rows[e.RowIndex].Cells[1].Value);
                    lblY.Text = string.Format("{0:0.000} mm", dgvStep.Rows[e.RowIndex].Cells[2].Value);
                    lblZ.Text = string.Format("{0:0.000} mm", dgvStep.Rows[e.RowIndex].Cells[3].Value);
                    nrSpeed.Value = position.speed;
                    nrDelay.Value = position.delay;
                    nrUp.Value = Convert.ToDecimal(Common.Pulse2Millimetter(position.up, Const.D3));
                    chkEdit.Enabled = true;
                    chkEdit.Checked = false;
                    chkSpray.Checked = position.spray == "1" ? true : false;
                    btnZ1.BackColor = position.z1 == "0" ? SystemColors.Control : Color.Crimson;
                    btnZ2.BackColor = position.z2 == "0" ? SystemColors.Control : Color.Crimson;
                }
                // View data in text
                //if (dgvStep.Rows[e.RowIndex].Cells[4].Value.ToString() == "1")
                //{
                //    txtRecei.Text = string.Format("#{0}{1}{2}*", position.x.ToString("D5"), position.y.ToString("D5"), position.z.ToString("D5"));
                //}
                //else
                //{
                //    txtRecei.ResetText();
                //}
            }
        }

        //public void ViewDetails(Position p)
        //{
        //    nrX.Value = p.x;
        //    nrY.Value = p.y;
        //    nrZ.Value = p.z;
        //    nrSpeed.Value = p.speed;
        //    nrDelay.Value = p.delay;
        //    chkUp.Checked = p.up == "1" ? true : false;
        //    chkSpray.Checked = p.spray == "1" ? true : false;
        //}

        private void btnStep_Click(object sender, EventArgs e)
        {
            // =====> #001X12345Y12345Z12345Z00S0500D0000S0U0000*
            arraySend = String.Format("#{0:D3}X{1:D5}Y{2:D5}Z{3:D5}Z{4}{5}S{6:D4}D{7:D4}S{8}U{9:D4}*", position.step, position.x, position.y, position.z, position.z1, position.z2, position.speed, position.delay, position.spray, position.up);
            Common.SendData(comControl, arraySend);
        }

        private void chkEdit_OnChange(object sender, EventArgs e)
        {
            if (chkEdit.Checked)
            {
                btnXTop.Enabled = btnXBottom.Enabled = btnYLeft.Enabled = btnYRight.Enabled = btnZTop.Enabled = btnZBottom.Enabled = true;
                btnSaves.Enabled = true;
                btnZ1.Enabled = btnZ2.Enabled = true;
                chkAdd.Checked = false;
            }
            else
            {
                btnSaves.Enabled = false;
                btnZ1.Enabled = btnZ2.Enabled = false;
            }
        }

        private void chkAdd_OnChange(object sender, EventArgs e)
        {
            if (chkAdd.Checked)
            {
                btnXTop.Enabled = btnXBottom.Enabled = btnYLeft.Enabled = btnYRight.Enabled = btnZTop.Enabled = btnZBottom.Enabled = true;
                btnZ1.Enabled = btnZ2.Enabled = true;
                btnAdd.Enabled = true;
                chkEdit.Checked = false;
            }
            else
            {
                btnXTop.Enabled = btnXBottom.Enabled = btnYLeft.Enabled = btnYRight.Enabled = btnZTop.Enabled = btnZBottom.Enabled = false;
                btnZ1.Enabled = btnZ2.Enabled = false;
                btnAdd.Enabled = false;
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            string xDuplicate = txtX.Text.Trim();
            try
            {
                Convert.ToDecimal(xDuplicate);
            }
            catch
            {
                MessageBox.Show(string.Format("{0} not valid!", xDuplicate), "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (MessageBox.Show("Are you save model?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveDuplicate("2-X", Const.D1, xDuplicate);
                LoadPosition();
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            string yDuplicate = txtY.Text.Trim();
            try
            {
                Convert.ToDecimal(yDuplicate);
            }
            catch
            {
                MessageBox.Show(string.Format("{0} not valid!", yDuplicate), "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (MessageBox.Show("Are you save model?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveDuplicate("2-Y", Const.D2, yDuplicate);
                LoadPosition();
            }
        }

        private void frmEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            comControl.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
        }
    }
}
