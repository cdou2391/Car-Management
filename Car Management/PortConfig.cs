using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace Car_Management
{
    public partial class PortConfig: Form
    {
        public string PortName = "";          
        public int BuadRate = 0;
        public int dataBits = 0;           
        public Parity parity = 0;
        public StopBits stopbits = 0;         
        public bool result = false;
        public PortConfig()
        {
            InitializeComponent();
            string[] names = SerialPort.GetPortNames();
            foreach (string name in names)
            {
                comboBox1.Items.Add(name);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 7;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 3;
            comboBox5.SelectedIndex = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PortName = comboBox1.Text;
            BuadRate = int.Parse(comboBox2.Text);
            dataBits = int.Parse(comboBox4.Text);
            parity = (Parity)comboBox3.SelectedIndex;
            stopbits = (StopBits)comboBox5.SelectedIndex;
            result = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
