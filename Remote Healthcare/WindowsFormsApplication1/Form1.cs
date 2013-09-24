﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private Connection connect;
        public Form1()
        {
            InitializeComponent();
            connect = Program.connect;
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

       

        private void toolTipSetDistance_Popup(object sender, PopupEventArgs e)
        {

        }

        

        private void textBoxSetPower_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                connect.sendMessage(textBox1.Text);
                textBox1.ResetText();
            }
        }

        private void buttonSetDistance_Click(object sender, EventArgs e)
        {
            if(textBoxSetDistance != null)
            {
                connect.sendCommand("PD"+textBoxSetDistance.Text);
            }
        }

        private void buttonSetEnergy_Click(object sender, EventArgs e)
        {
            if(textBoxSetEnergy != null)
            {
                connect.sendCommand("PE"+textBoxSetEnergy.Text);
            }
        
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBoxSetTime != null)
            {
                connect.sendCommand("PT"+textBoxSetTime.Text);
            }
        }

        private void buttonSetPower_Click(object sender, EventArgs e)
        {
            if(textBoxSetTime != null)
            {
                connect.sendCommand("PW" + comboBoxPower.Text);
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            connect.sendCommand("RS");
        }

        private void textBoxSetDistance_Click(object sender, EventArgs e)
        {
            textBoxSetDistance.Clear();
        }

        private void textBoxSetEnergy_Click(object sender, EventArgs e)
        {
            textBoxSetEnergy.Clear();
        }

        private void textBoxSetTime_Click(object sender, EventArgs e)
        {
            textBoxSetTime.Clear();
        }

        

    }
}
