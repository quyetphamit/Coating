using CoatingSupport.Business;
using CoatingSupport.Entities;
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
    public partial class frmHistory : MetroFramework.Forms.MetroForm
    {
        private ConnectFactory factory = new ConnectFactory();
        public frmHistory()
        {
            InitializeComponent();
        }

        private void frmHistory_Load(object sender, EventArgs e)
        {
            string query = "select * from tbHistories order by date desc limit 25";
            StringBuilder sb = new StringBuilder();
            factory.ExecuteQuery(query).AsEnumerable().Select(r => new History { model = r.Field<string>(0), date = r.Field<string>(1), status = r.Field<string>(2) })
                   .ToList().ForEach(r =>
                   {
                       switch (r.status)
                       {
                           case "start": sb.AppendLine("Application has started at " + r.date); break;
                           case "end": sb.AppendLine("Application has stopped at " + r.date); break;
                           case "finish": sb.AppendLine("Model " + r.model + " has finished at " + r.date); break;
                           default:
                               break;
                       }

                   });
            txtHistories.Text = sb.ToString();
        }
    }
}
