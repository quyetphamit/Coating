using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoatingSupport
{
    public partial class frmError : Form
    {
        public frmError()
        {
            InitializeComponent();
            Timer t1 = new Timer();
            t1.Interval = 500;
            t1.Tick += T1_Tick;
            t1.Start();
        }

        private void T1_Tick(object sender, EventArgs e)
        {
            if (this.BackColor == Color.White)
            {
                this.BackColor = Color.Tomato;
                this.lblNG.ForeColor = Color.Yellow;
                this.lblError.ForeColor = Color.Yellow;
            }
            else
            {
                this.BackColor = Color.White;
                this.lblNG.ForeColor = Color.Red;
                this.lblError.ForeColor = Color.Red;
            }
        }
    }
}
