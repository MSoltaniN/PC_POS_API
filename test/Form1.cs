using System;
using System.Windows.Forms;
using PcPosApi;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private PcPosApi.CPcPosApi pcposapi=new CPcPosApi();
        private void btnTestPos_Click(object sender, EventArgs e)
        {
            

            pcposapi.Run(1121, "7000;192.168.126.34;com1;10000;20000");

           MessageBox.Show( pcposapi.RefNo);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pcposapi.close(1121, "7000;192.168.126.34;com1;10000;20000");
        }
    }
}
