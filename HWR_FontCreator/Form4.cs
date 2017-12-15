using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HWR_FontCreator
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = checkBox1.Checked;
            button4.Enabled = checkBox1.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = @"TTF font files (*.ttf)|*.ttf"
                }
            )
            {
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;
                textBox1.Text = dialog.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = @"TTF font files (*.ttf)|*.ttf"
                }
            )
            {
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;
                textBox2.Text = dialog.FileName;
            }
        }

        //确认
        private void button1_Click(object sender, EventArgs e)
        {
            var answer = ((Form1) Owner).form4answer;
            answer.normalFontPath = textBox1.Text;
            answer.boldFontPath = textBox2.Text;
            answer.useBoldFont = checkBox1.Checked;
            answer.charSet = textBox3.Text;
            answer.fontSizeMod = int.Parse(textBox4.Text);
            answer.fontMarginMod = double.Parse(textBox5.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = @"Text files (*.txt)|*.txt"
                }
            )
            {
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;
                textBox3.Text = File.ReadAllText(dialog.FileName);
            }
        }
    }
}
