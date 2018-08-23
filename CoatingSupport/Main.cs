using CoatingSupport.Business;
using CoatingSupport.Entities;
using CoatingSupport.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Text;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace CoatingSupport
{
    public partial class frmMain : MetroFramework.Forms.MetroForm
    {
        private delegate void PassingData(SerialPort port);
        private PassingData passing;
        private Position position = new Position();
        private string pathLog = Path.Combine(Application.StartupPath, "log");
        private string pathError = Path.Combine(Application.StartupPath, "error.txt");
        private Dictionary<string, string> dicMessage;
        private List<Position> lstPosition = new List<Position>();
        private SystemSetting setting; // Lấy dữ liệu từ file config.xml
        private SerialPort comControl = new SerialPort();
        private string arraySend = string.Empty; // Chuỗi kí tự cần gửi xuống Comport
        private int index = 0;
        private bool start = false;
        private string inputData = String.Empty;
        private int quantity = 0;
        private System.Windows.Forms.Timer t1 = new System.Windows.Forms.Timer();
        private ConnectFactory factory = new ConnectFactory();
        private string query = string.Empty;
        public frmMain()
        {
            InitializeComponent();
            comControl.DataReceived += new SerialDataReceivedEventHandler(ComControl_DataReceived);
            CheckForIllegalCrossThreadCalls = false;
            t1.Tick += T1_Tick;
            t1.Interval = 1000;
            t1.Enabled = true;
            t1.Start();
        }

        private void T1_Tick(object sender, EventArgs e)
        {
            float fCpu = pCpu.NextValue();
            float fRam = pRam.NextValue();
            mpgrCpu.Value = (int)fCpu;
            mpgrRam.Value = (int)fRam;
            lblCpu.Text = string.Format("{0:0.00}%", fCpu);
            lblRam.Text = string.Format("{0:0.00}%", fRam);
            chart1.Series["CPU"].Points.AddY(fCpu);
            chart1.Series["RAM"].Points.AddY(fRam);
            if (DateTime.Now.ToString("hh:mm:ss").Equals("07:55:00"))
            {
                quantity = 0;
                lblQuantity.Text = quantity.ToString();
            }
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void ComControl_DataReceived(object sender, SerialDataReceivedEventArgs e)
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

        private void frmMain_Load(object sender, EventArgs e)
        {
            Init();
            LoadComponent();
            checkComport();
            ReadErrorMessage(pathError);
        }

        /// <summary>
        /// Hàm khởi tạo
        /// Đọc dữ liệu từ file config.xml
        /// </summary>
        public void Init()
        {
            if (!Directory.Exists(pathLog)) Directory.CreateDirectory(pathLog);
            if (!Directory.Exists(Const.pathModel)) Directory.CreateDirectory(Const.pathModel);
            if (!File.Exists(Const.pathConfig)) SaveInitSystem();
            SystemSetting.ReadXML<SystemSetting>(out setting, Const.pathConfig);
            // Insert database
            query = string.Format("insert into tbHistories(model,date,status) values('{0}','{1}','{2}')", "all", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "start");
            factory.ExecuteNonQuery(query);
        }

        public Dictionary<string, string> ReadErrorMessage(string path)
        {
            dicMessage = new Dictionary<string, string>();
            File.ReadLines(path).ToList().ForEach(r =>
            {
                var col = r.Split(',');
                dicMessage.Add(col[0], col[1]);
            });
            return dicMessage;
        }

        /// <summary>
        /// Load thông tin về model
        /// </summary>
        public void LoadComponent()
        {
            // Load Model
            cbbModel.DataSource = Directory.GetFiles(Const.pathModel, "*.csv").Select(Path.GetFileNameWithoutExtension).ToList();
            cStep.Value = 0;
            btnStart.Enabled = false;
            //PrivateFontCollection pfc = new PrivateFontCollection();
            //pfc.AddFontFile(Application.StartupPath + "\\Res\\DS-DIGIT.TTF");
            //lblTime.Font = new System.Drawing.Font(pfc.Families[0], 23, System.Drawing.FontStyle.Regular);
        }

        /// <summary>
        /// Kiểm tra Comport
        /// </summary>
        /// <returns></returns>
        public bool checkComport()
        {
            try
            {
                comControl.PortName = setting.portName;
                comControl.BaudRate = setting.baudRate;
                comControl.DataBits = setting.dataBits;
                comControl.Parity = setting.parity == "None" ? Parity.None :
                                      setting.parity == "Old" ? Parity.Odd :
                                      setting.parity == "Mark" ? Parity.Mark :
                                      setting.parity == "Even" ? Parity.Even : Parity.Space;
                comControl.StopBits = setting.stopBits == "1" ? StopBits.One :
                                      setting.stopBits == "1.5" ? StopBits.OnePointFive :
                                      setting.stopBits == "2" ? StopBits.Two : StopBits.None;
                comControl.ReadTimeout = 500;
                comControl.WriteTimeout = 500;
                comControl.Open();
                comControl.DiscardInBuffer();
                comControl.DiscardOutBuffer();
                //comControl.DtrEnable = true;
                //comControl.RtsEnable = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Tạo file config.xml trong trường hợp file này không tồn tại
        /// </summary>
        public void SaveInitSystem()
        {
            setting = new SystemSetting();
            setting.portName = "COM5";
            setting.baudRate = 9600;
            setting.dataBits = 8;
            setting.parity = "None";
            setting.stopBits = "1";
            SystemSetting.WriteXML<SystemSetting>(this.setting, Const.pathConfig);
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            query = string.Format("insert into tbHistories(model,date,status) values('{0}','{1}','{2}')", "all", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "end");
            factory.ExecuteNonQuery(query);
            t1.Stop();
            comControl.Close();
            comControl.Dispose();
        }

        private void cbbModel_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string pathCsv = Path.Combine(Const.pathModel, cbbModel.Text + ".csv");
            lstPosition = new List<Position>();
            try
            {
                lstPosition = Common.ReadCsv(pathCsv);
            }
            catch
            {
                MessageBox.Show("Database not valid!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            index = 0;
            if (start) btnStart.Enabled = true;
            string startTime = string.Empty;
            string endTime = string.Empty;
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 8)
            {
                startTime = new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month, DateTime.Now.AddDays(-1).Day, 20, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
                endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");

            }
            else if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 20)
            {
                startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
                endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
                endTime = new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, 8, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            }
            query = string.Format("select count(*) from tbHistories where model = '{0}' and date between '{1}' and '{2}'", cbbModel.Text.Trim(), startTime, endTime);
            quantity = factory.GetTotalRecord(query);
            lblQuantity.Text = quantity.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, "#RUN*");
            btnStart.Enabled = cbbModel.Enabled = false;
            btnStop.Enabled = true;
            status.Text = "RUNNING";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            btnStart.Enabled = cbbModel.Enabled = true;
            status.Text = "WAIT...";
            index = 0;
            bPercent.Value = 0;
            cStep.Value = 0;
            cStep.ResetText();
        }

        private void txtRecei_OnValueChanged(object sender, EventArgs e)
        {
            Regex patternStart = new Regex(@"^[#]+[R]+[*]$");
            Regex pattern8 = new Regex(@"^[#]+[R]+$");
            Regex pattern4 = new Regex(@"^[#]+$");
            Regex pattern1 = new Regex(@"^[#]+[M]+$");
            Regex pattern2 = new Regex(@"^[#]+[M]+[\d]+$");
            Regex pattern3 = new Regex(@"^[#]+[O]+[K]+[*]+$");
            Regex pattern5 = new Regex(@"^[#]+[O]+$");
            Regex pattern6 = new Regex(@"^[#]+[O]+[K]+$");
            Regex pattern7 = new Regex(@"^[#]+[S]+$");
            Regex patternInit = new Regex(@"^[#]+[S]+[*]+$");
            Regex pattern = new Regex(@"^[#]+[M]+[\d{3}]+[*]+$");
            Regex patternError = new Regex(@"^[#]+[\d{2}]+[*]+$");
            Thread.Sleep(100);
            string result = txtRecei.Text.Trim();
            if (pattern.IsMatch(result) || pattern1.IsMatch(result) || pattern2.IsMatch(result) || pattern3.IsMatch(result) || pattern4.IsMatch(result) || patternError.IsMatch(result) || pattern5.IsMatch(result) || pattern6.IsMatch(result) || pattern7.IsMatch(result) || pattern8.IsMatch(result) || patternInit.IsMatch(result) || patternStart.IsMatch(result))
            {
                if (pattern3.IsMatch(result))
                {
                    position = lstPosition.ElementAtOrDefault(0);
                    // =====> #001X12345Y12345Z12345Z00S0500D0000S0U0000*
                    arraySend = String.Format("#{0:D3}X{1:D5}Y{2:D5}Z{3:D5}Z{4}{5}S{6:D4}D{7:D4}S{8}U{9:D4}*", position.step, position.x, position.y, position.z, position.z1, position.z2, position.speed, position.delay, position.spray, position.up);
                    //comControl.DiscardInBuffer();
                    //comControl.DiscardOutBuffer();

                    Common.SendData(comControl, arraySend);
                    index++;
                    bPercent.Value = (index * 100 / lstPosition.Count);

                    cStep.Value = index * 100 / lstPosition.Count();
                    cStep.Text = index.ToString();

                    lblX.Text = string.Format("{0:0.000 mm}", Common.Pulse2Millimetter(position.x, Const.D1));
                    lblY.Text = string.Format("{0:0.000 mm}", Common.Pulse2Millimetter(position.y, Const.D2));
                    lblZ.Text = string.Format("{0:0.000 mm}", Common.Pulse2Millimetter(position.z, Const.D3));
                    lblSpeed.Text = position.speed.ToString();
                    lblDelay.Text = position.delay.ToString();
                    lblUp.Text = string.Format("{0:0.000 mm}", position.up);
                    chkSpray.Checked = position.spray == "1" ? true : false;
                    txtRecei.ResetText();
                }
                if (pattern.IsMatch(result))
                {
                    if (index < lstPosition.Count)
                    {
                        int step = Convert.ToInt32(result.Substring(2, 3));
                        position = lstPosition.ElementAtOrDefault(index);
                        // =====> #001X12345Y12345Z12345Z00S0500D0000S0U0000*
                        arraySend = String.Format("#{0:D3}X{1:D5}Y{2:D5}Z{3:D5}Z{4}{5}S{6:D4}D{7:D4}S{8}U{9:D4}*", position.step, position.x, position.y, position.z, position.z1, position.z2, position.speed, position.delay, position.spray, position.up);
                        if (step == index)
                        {
                            lblX.Text = string.Format("{0:0.000 mm}", Common.Pulse2Millimetter(position.x, Const.D1));
                            lblY.Text = string.Format("{0:0.000 mm}", Common.Pulse2Millimetter(position.y, Const.D2));
                            lblZ.Text = string.Format("{0:0.000 mm}", Common.Pulse2Millimetter(position.z, Const.D3));
                            lblSpeed.Text = position.speed.ToString();
                            lblDelay.Text = position.delay.ToString();
                            lblUp.Text = string.Format("{0:0.000 mm}", position.up);
                            chkSpray.Checked = position.spray == "1" ? true : false;
                            Console.WriteLine("=====> " + arraySend);
                            index++;
                            Common.SendData(comControl, arraySend);
                            bPercent.Value = (index * 100 / lstPosition.Count);

                            cStep.Value = index * 100 / lstPosition.Count();
                            cStep.Text = index.ToString();

                            comControl.DiscardInBuffer();
                            comControl.DiscardOutBuffer();
                            txtRecei.ResetText();
                        }
                    }
                    else
                    {
                        Common.SendData(comControl, "#FN*");
                        bPercent.Value = 0;
                        quantity++;
                        lblQuantity.Text = quantity.ToString();
                        index = 0;
                        query = string.Format("insert into tbHistories(model,date,status) values('{0}','{1}','{2}')", cbbModel.Text.Trim(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "finish");
                        factory.ExecuteNonQuery(query);

                        cStep.Value = 0;
                        cStep.Text = "0";
                        comControl.DiscardInBuffer();
                        comControl.DiscardOutBuffer();
                        //Thread.Sleep(500);
                        lblX.Text = lblY.Text = lblZ.Text = lblSpeed.Text = lblDelay.Text = lblUp.Text = "0";
                        chkSpray.Checked = false;
                        txtRecei.ResetText();

                    }

                }
                if (patternInit.IsMatch(result))
                {
                    btnStart.Enabled = true;
                    start = true;
                    status.Text = "READY!";
                    txtRecei.ResetText();
                }
                if (patternStart.IsMatch(result))
                {
                    btnStart_Click(sender, null);
                    txtRecei.ResetText();
                }
                if (patternError.IsMatch(result))
                {
                    try
                    {
                        frmError frmError = new frmError();
                        Label lblError = frmError.Controls.Find("lblError", true).FirstOrDefault() as Label;
                        lblError.Text = dicMessage[result];
                        frmError.ShowDialog();
                        txtRecei.ResetText();
                    }
                    catch
                    {
                        MessageBox.Show("UNKNOWN ERROR!");
                    }
                }
            }
            else
            {
                txtRecei.ResetText();
            }
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            frmTeaching frmTeaching = new frmTeaching();
            passing += new PassingData(frmTeaching.Setcom);
            passing(comControl);
            comControl.DataReceived -= new SerialDataReceivedEventHandler(ComControl_DataReceived);
            frmTeaching.FormClosing += FrmTeaching_FormClosing;
            frmTeaching.ShowDialog();

        }

        private void FrmTeaching_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Recei");
            comControl.DataReceived += new SerialDataReceivedEventHandler(ComControl_DataReceived);
            this.Refresh();

        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            frmEdit frmEdit = new frmEdit();
            passing += new PassingData(frmEdit.SetCom);
            passing(comControl);
            comControl.DataReceived -= ComControl_DataReceived;
            frmEdit.FormClosing += FrmEdit_FormClosing;
            frmEdit.ShowDialog();
        }

        private void FrmEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            comControl.DataReceived += ComControl_DataReceived;
        }

        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            frmSetup frmSetup = new frmSetup();
            passing += new PassingData(frmSetup.Init);
            passing(this.comControl);
            comControl.DataReceived -= ComControl_DataReceived;
            frmSetup.FormClosing += FrmSetup_FormClosing;
            frmSetup.ShowDialog();
        }

        private void FrmSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            comControl.DataReceived += ComControl_DataReceived;
        }

        private void bunifuFlatButton7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void bunifuFlatButton4_Click(object sender, EventArgs e)
        {
            frmOption frmOption = new frmOption();
            frmOption.ShowDialog();
        }

        private void bunifuFlatButton6_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Common.SendData(comControl, Const.RESET);
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            frmHistory frmHistories = new frmHistory();
            frmHistories.ShowDialog();
        }
    }
}
