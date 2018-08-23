using CoatingSupport.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoatingSupport.Utilities
{
    public class Common
    {
        public enum Teaching
        {
            XDuong, XAm, YDuong, YAm, ZDuong, ZAm
        }
        /// <summary>
        /// Tạo file CSV
        /// </summary>
        /// <param name="path">Đường dẫn</param>
        /// <param name="lstPosition">Danh sách những vị trí cần lưu</param>
        public static void WriteCSV(string path, List<Position> lstPosition)
        {
            int step = 0;
            using (StreamWriter file = new StreamWriter(path, false))
            {
                string header = string.Join(",", "Step", "X", "Y", "Z", "Z1", "Z2", "Delay", "Speed", "Spray", "Up", "Area");
                file.WriteLine(header);
                foreach (var p in lstPosition)
                {
                    string csvData = string.Join(",", ++step, p.x, p.y, p.z, p.z1, p.z2, p.delay, p.speed, p.spray, p.up, p.area);
                    file.WriteLine(csvData);
                }
            }
        }
        /// <summary>
        /// Gửi dữ liệu qua Serial Port
        /// </summary>
        /// <param name="comport">Serial Port</param>
        /// <param name="data">Dữ liệu</param>
        public static void SendData(SerialPort comport, string data)
        {
            if (comport.IsOpen) comport.WriteLine(data);
            else System.Windows.Forms.MessageBox.Show(comport.PortName + " not open!");

        }
        /// <summary>
        /// Thiết lập các thông số cổng com
        /// </summary>
        /// <param name="comport"></param>
        /// <param name="setting"></param>
        public static void SetComport(SerialPort comport, SystemSetting setting)
        {
            try
            {
                comport.PortName = setting.portName;
                comport.BaudRate = setting.baudRate;
                comport.DataBits = setting.dataBits;
                comport.Parity = setting.parity == "None" ? Parity.None :
                                      setting.parity == "Old" ? Parity.Odd :
                                      setting.parity == "Mark" ? Parity.Mark :
                                      setting.parity == "Even" ? Parity.Even : Parity.Space;
                comport.StopBits = setting.stopBits == "1" ? StopBits.One :
                                      setting.stopBits == "1.5" ? StopBits.OnePointFive :
                                      setting.stopBits == "2" ? StopBits.Two : StopBits.None;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// Đọc file CSV file
        /// </summary>
        /// <param name="path">Đường dẫn</param>
        /// <returns>Danh sách tọa độ</returns>
        public static List<Position> ReadCsv(string path)
        {
            List<Position> result = File.ReadLines(path).Skip(1).Select(r => Position.FromCsv(r)).ToList();
            return result;
        }
        /// <summary>
        /// Chuyển từ xung sang mm
        /// </summary>
        /// <param name="pulse">xung</param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Pulse2Millimetter(int pulse, double d)
        {
            return Convert.ToDouble(pulse * d / 4000);
        }
        /// <summary>
        /// Chuyển từ mm sang xung
        /// </summary>
        /// <param name="millimetter">mm</param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int Millimetter2Pulse(string millimetter, double d)
        {
            return Convert.ToInt32(Convert.ToDouble(millimetter) * 4000 / d);
        }
    }
}
