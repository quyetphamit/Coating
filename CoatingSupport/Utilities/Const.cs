using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoatingSupport.Utilities
{
    public class Const
    {
        // path
        public readonly static string pathConfig = Path.Combine(Application.StartupPath, "config.xml");
        public readonly static string pathModel = Path.Combine(Application.StartupPath, "model");
        public readonly static string X_INCREMENT = "#X+*";
        public readonly static string X_DECREASE = "#X-*";
        public readonly static string X_STOP = "#XS*";
        public readonly static string Y_INCREMENT = "#Y+*";
        public readonly static string Y_DECREASE = "#Y-*";
        public readonly static string Y_STOP = "#YS*";
        public readonly static string Z_INCREMENT = "#Z+*";
        public readonly static string Z_DECREASE = "#Z-*";
        public readonly static string Z_STOP = "#ZS*";
        // Reset
        public readonly static string RESET = "#RST*";
        // Origin
        public readonly static string ORIGIN = "#ORG*";

        public readonly static string Z1_ON = "#Z10*";
        public readonly static string Z1_OFF = "#Z00*";
        public readonly static string Z2_ON = "#Z01*";
        public readonly static string Z2_OFF = "#Z00*";

        // Motor
        public readonly static string MOTOR_ON = "#MO1*";
        public readonly static string MOTOR_OFF = "#MO0*";

        // Stopper
        public readonly static string STOPPER_ON = "#ST1*";
        public readonly static string STOPPER_OFF = "#ST0*";

        // Holder
        public readonly static string HOLDER_ON = "#HO1*";
        public readonly static string HOLDER_OFF = "#HO0*";
        // Điểm chờ
        public readonly static string WAIT = "#W*";
        // Điểm Test
        public readonly static string TEST = "#T*";

        // Điểm Xả 1
        public readonly static string XA_1 = "#U*";
        // Điểm Xả 2
        public readonly static string XA_2 = "#V*";

        public readonly static double D1 = 20.1 * 3.14;
        public readonly static double D2 = 22.1 * 3.14;
        public readonly static double D3 = 10;
    }
}
