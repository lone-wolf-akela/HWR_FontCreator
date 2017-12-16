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

            textBox6.Enabled = checkBox1.Checked && checkBox2.Checked;
            button6.Enabled = checkBox1.Checked && checkBox2.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFontDialog(textBox1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFontDialog(textBox2);
        }

        //确认
        private void button1_Click(object sender, EventArgs e)
        {
            var answer = ((Form1) Owner).Form4Answer;
            answer.NormalFontPath = textBox1.Text;
            answer.BoldFontPath = textBox2.Text;
            answer.UseBoldFont = checkBox1.Checked;
            answer.CharSet = textBox3.Text;
            answer.FontBaselineMod = float.Parse(textBox5.Text);
            answer.UseSpecialFont4Ascii = checkBox2.Checked;
            answer.AsciiNormalFontPath = textBox7.Text;
            answer.AsciiBoldFontPath = textBox6.Text;
            answer.AsciiFontBaselineMod = float.Parse(textBox4.Text);
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

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox6.Enabled = checkBox1.Checked && checkBox2.Checked;
            button6.Enabled = checkBox1.Checked && checkBox2.Checked;

            textBox7.Enabled = checkBox2.Checked;
            button7.Enabled = checkBox2.Checked;
            textBox4.Enabled = checkBox2.Checked;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openFontDialog(textBox7);
        }

        private void openFontDialog(TextBox box)
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
                box.Text = dialog.FileName;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            openFontDialog(textBox6);
        }
    }
}
