using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cyut_Auo_Component_Measurer
{
    public partial class Form_Dot_Grid : Form
    {
        public Form_Dot_Grid(float x, float y)
        {
            InitializeComponent();
            numX.Value = (int)x;
            numY.Value = (int)y;

        }

        private void Form_Closed(object sender, FormClosedEventArgs e)
        {

        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)this.Owner;

            f1.x = (int)numX.Value;
            f1.y = (int)numY.Value;

            this.Close();

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
