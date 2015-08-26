using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CruiseDAL.CodeGenEngine
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
        }

        private void _start_BTN_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            DataDictionaryReader dr = new DataDictionaryReader();
            foreach (Table t in dr.AllTables.Values)
            {
                foreach (Field f in t.Fields)
                {
                    sb.AppendLine(f.DBDefault);
                }
            }

            MessageBox.Show(sb.ToString());
        }
    }
}
