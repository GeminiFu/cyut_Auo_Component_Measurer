using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Euresys.Open_eVision_22_08;

namespace cyut_Auo_Component_Measurer
{
    public partial class Form1 : Form
    {
        EImageBW8 EBW8Image1 = new EImageBW8();
        OpenFileDialog openFileDialog1 = new OpenFileDialog();


        View c_view;
        Control c_control;

        public Form1()
        {
            InitializeComponent();

            c_view = new View(ref pictureBox1);
            c_control = new Control();
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            c_control.LoadEImageBW8(ref EBW8Image1);

            c_view.DrawEBW8Image(ref EBW8Image1);
        }

       
    }
}
