using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HWR_FontCreator
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkboxChanged();
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
            
            answer.UseBoldFont = checkBox1.Checked;
            answer.CharSet = textBox3.Text;
            answer.FontBaselineMod = float.Parse(textBox5.Text);
            answer.UseSpecialFont4Ascii = checkBox2.Checked;
            answer.AsciiNormalFontPath = textBox7.Text;           
            answer.AsciiFontBaselineMod = float.Parse(textBox4.Text);

            //自动生成粗体
            if (checkBox3.Checked)
            {
                answer.BoldFontPath = answer.NormalFontPath;
                answer.AsciiBoldFontPath = answer.AsciiNormalFontPath;
            }
            else
            {
                answer.BoldFontPath = textBox2.Text;
                answer.AsciiBoldFontPath = textBox6.Text;
            }

            answer.autoBold = checkBox3.Checked;
            answer.autoBoldStrength = double.Parse(textBox8.Text);
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
            checkboxChanged();
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

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            checkboxChanged();
        }

        private void checkboxChanged()
        {
            
            //粗体选择
            textBox2.Enabled = checkBox1.Checked && !checkBox3.Checked;
            button4.Enabled = checkBox1.Checked && !checkBox3.Checked;

            //ASCII选择
            textBox7.Enabled = checkBox2.Checked;
            button7.Enabled = checkBox2.Checked;

            //ASCII粗体选择
            textBox6.Enabled = checkBox1.Checked && checkBox2.Checked && !checkBox3.Checked;
            button6.Enabled = checkBox1.Checked && checkBox2.Checked && !checkBox3.Checked;

            //ASCII基线修正
            textBox4.Enabled = checkBox2.Checked;

            //自动生成粗体
            checkBox3.Enabled = checkBox1.Checked;
            textBox8.Enabled = checkBox1.Checked && checkBox3.Checked;
        }
    }
}
