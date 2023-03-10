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
        public Form_Dot_Grid(int x, int y)
        {
            InitializeComponent();
            numX.Value = x;
            numY.Value = y;
        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)this.Owner;

            if(numX.Value == 0 || numY.Value == 0)
            {
                MessageBox.Show("X 或 Y 不能為 0。");
                return;
            }

            f1.calibrationX = (int)numX.Value;
            f1.calibrationY = (int)numY.Value;

            f1.isDoCalibration = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
        }

        private void Form_Dot_Grid_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}
