using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Homeworld2.RCF;
using SharpFont;
using Glyph = Homeworld2.RCF.Glyph;


namespace HWR_FontCreator
{
    public partial class Form1 : Form
    {
        private readonly RCF _font = new RCF();
        private Form2 _form2;
        private Form3 _form3;
        private Form4 _form4;
        private Form5 _form5;

        public Form1()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            InitializeComponent();
        }

        //打开rcf字体文件
        private void button1_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = false,
                Filter = @"RCF font files (*.rcf)|*.rcf"
            }
            )
            {
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;
                using (Stream fontStream = dialog.OpenFile())
                {
                    _font.Read(fontStream);
                }
            }

            RefreshFontInfo();
        }

        private void RefreshFontInfo()
        {

            listBox1.Items.Clear();
            listBox2.Items.Clear();

            for (int i = 1; i <= _font.Typefaces.Count; i++)
            {
                listBox1.Items.Add(i);
            }

            textBox1.Text = _font.Name;
            textBox7.Text = _font.CharsetCount.ToString();
            textBox8.Text = _font.Version.ToString();

            textBox4.Text = _font.Charset;
        }

        //选择typeface
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            Typeface selectedType = _font.Typefaces[listBox1.SelectedIndex];
            listBox2.Items.Clear();
            for (int i = 1; i <= selectedType.Images.Count; i++)
            {
                listBox2.Items.Add(i);
            }
            textBox2.Text = selectedType.Name;
            textBox9.Text = selectedType.Attributes;
        }

        //选择image
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox2.SelectedIndex < 0)
            {
                return;
            }
            Homeworld2.RCF.Image img = _font.Typefaces[listBox1.SelectedIndex].Images[listBox2.SelectedIndex];
            _form2.pictureBox1.Image = helper.type2img(img);


            textBox3.Text = img.Name;
            textBox10.Text = img.Version.ToString();
            textBox11.Text = img.Width.ToString();
            textBox12.Text = img.Height.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _form2 = new Form2();
            _form2.Show();
            _form3 = new Form3();
            _form3.Show();

            _form5 = new Form5();
        }

        //选择特定文字
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (
                textBox5.Text == "" ||
                listBox1.SelectedIndex < 0
            )
                return;

            var glyphs = from aGlyph in _font.Typefaces[listBox1.SelectedIndex].Glyphs
                         where aGlyph.Character == textBox5.Text[0]
                         select aGlyph;

            if (!glyphs.Any())
            {
                return;
            }

            Glyph glyph = glyphs.First();

            Homeworld2.RCF.Image img =
                _font.Typefaces[listBox1.SelectedIndex].Images[glyph.ImageIndex];

            Bitmap bmp = helper.type2img(img);
            Rectangle rect = new Rectangle(
                glyph.LeftMargin,
                glyph.TopMargin,
                glyph.Width,
                glyph.Height
            );

            _form3.pictureBox1.Image = helper.type2img(img).Clone(rect, bmp.PixelFormat);
            textBox6.Text = glyph.Height.ToString();
            textBox13.Text = glyph.Width.ToString();
            textBox14.Text = glyph.BitmapLeft.ToString(CultureInfo.InvariantCulture);
            textBox15.Text = glyph.BitmapRight.ToString(CultureInfo.InvariantCulture);
            textBox16.Text = glyph.AdvanceX.ToString(CultureInfo.InvariantCulture);
            textBox17.Text = glyph.BitmapTop.ToString(CultureInfo.InvariantCulture);
            textBox18.Text = glyph.Baseline.ToString(CultureInfo.InvariantCulture);
            textBox19.Text = glyph.BitmapBottom.ToString(CultureInfo.InvariantCulture);
            textBox20.Text = glyph.TopMargin.ToString();
            textBox21.Text = glyph.ImageIndex.ToString();
            textBox22.Text = glyph.LeftMargin.ToString();        
        }

        //导入字体文件
        private void button2_Click(object sender, EventArgs e)
        {
            _form4 = new Form4();
            if (_form4.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            _form4.Dispose();

            var aLibrary = new Library();
            var aFaceN = aLibrary.NewFace(Form4Answer.NormalFontPath, 0);
            Face aFaceB = null;
            Face aFaceAN = null;
            Face aFaceAB = null;

            if (Form4Answer.UseBoldFont)
            {
                aFaceB = aLibrary.NewFace(Form4Answer.BoldFontPath, 0);
            }
            if (Form4Answer.UseSpecialFont4Ascii)
            {
                aFaceAN = aLibrary.NewFace(Form4Answer.AsciiNormalFontPath, 0);
                if (Form4Answer.UseBoldFont)
                {
                    aFaceAB = aLibrary.NewFace(Form4Answer.AsciiBoldFontPath, 0);
                }
            }

            for (int i = 0; i < Form4Answer.CharSet.Length; i++)
            {
                if (aFaceN.GetCharIndex(Form4Answer.CharSet[i]) == 0)
                {
                    Form4Answer.CharSet = Form4Answer.CharSet.Remove(i, 1);
                    i--;
                }
            }

            _font.Charset = Form4Answer.CharSet;
            _font.Name = "default";
            _font.Version = 1;

            //Attributes参数的具体含义尚不完全明白。不过第三个字节表示是不是有粗体这一点还是能确定的。
            if (Form4Answer.UseBoldFont)
            {
                _font.Attributes = new byte[] { 14, 0, 255, 0, 0 };
            }
            else
            {
                _font.Attributes = new byte[] { 14, 0, 0, 0, 0 };
            }

            _font.Typefaces.Clear();

            //对每一种type
            foreach (var typeinfo in _typeinfos)
            {
                //如果不打算生成粗体就跳过粗体
                if (typeinfo.Bold && (!Form4Answer.UseBoldFont))
                {
                    continue;
                }

                Face aFaceNonAscii = typeinfo.Bold ? aFaceB : aFaceN;
                Face aFaceAscii = null;

                if (Form4Answer.UseSpecialFont4Ascii)
                {
                    aFaceAscii = typeinfo.Bold ? aFaceAB : aFaceAN;
                }

                //设置渲染字体大小
                aFaceNonAscii.SetCharSize(typeinfo.Pt, 0, 72, 72);
                if (Form4Answer.UseSpecialFont4Ascii)
                {
                    aFaceAscii.SetCharSize(typeinfo.Pt, 0, 72, 72);
                }

                //建立typeface
                string style;
                if (aFaceNonAscii.StyleFlags.HasFlag(StyleFlags.Bold))
                {
                    style = "Bold";
                }
                else if (aFaceNonAscii.StyleFlags.HasFlag(StyleFlags.Italic))
                {
                    style = "Italic";
                }
                else
                {
                    style = "Normal";
                }

                _font.Typefaces.Add(new Typeface
                {
                    Name = typeinfo.Name,
                    Attributes = $"{aFaceNonAscii.FamilyName} {aFaceNonAscii.StyleName}, {typeinfo.Pt} pt, {style}, Default, Smooth"
                });
                var type = _font.Typefaces.Last();
                

                //决定图片大小
                int imgSize = 128;
                if (typeinfo.Pt * typeinfo.Pt * _font.CharsetCount > 128 * 128 * 5)
                {
                    imgSize = 256;
                }
                if (typeinfo.Pt * typeinfo.Pt * _font.CharsetCount > 256 * 256 * 5)
                {
                    imgSize = 512;
                }
                if (typeinfo.Pt * typeinfo.Pt * _font.CharsetCount > 512 * 512 * 5)
                {
                    imgSize = 1024;
                }
                if (typeinfo.Pt * typeinfo.Pt * _font.CharsetCount > 1024 * 1024 * 5)
                {
                    imgSize = 2048;
                }
                if (typeinfo.Pt * typeinfo.Pt * _font.CharsetCount > 2048 * 2048 * 5)
                {
                    imgSize = 4096;
                }

                int topMargin = 0;
                int leftMargin = 0;

                var imgdata = new byte[imgSize * imgSize];
                Array.Clear(imgdata, 0, imgSize * imgSize);
                int imgnum = 0;

                //记录一行里最高的字符
                int maxHeightInLine = 0;

                //渲染每个字
                for (int charaIndex = 0; charaIndex < _font.CharsetCount; charaIndex++)
                {
                    Face aFace;
                    float fontBaselineMod;
                    var chara = _font.Charset[charaIndex];
                    //检查该字是不是需要使用ASCII字符的特殊设置
                    bool ascii = (chara < 128) && Form4Answer.UseSpecialFont4Ascii;
                    if (ascii)
                    {
                        aFace = aFaceAscii;
                        fontBaselineMod = Form4Answer.AsciiFontBaselineMod;
                    }
                    else
                    {
                        aFace = aFaceNonAscii;
                        fontBaselineMod = Form4Answer.FontBaselineMod;
                    }

                    type.Glyphs.Add(new Glyph
                    {
                        Character = chara
                    });
                    var gly = type.Glyphs.Last();

                    //渲染
                    aFace.LoadChar(chara, LoadFlags.Default, LoadTarget.Normal);
                    aFace.Glyph.RenderGlyph(RenderMode.Normal);
                    using (FTBitmap aBmp = aFace.Glyph.Bitmap)
                    {
                        //设置gly相应属性
                        gly.Height = aBmp.Rows;
                        gly.Width = aBmp.Width;
                        if (gly.Width == 0)
                        {
                            gly.Width = 1;
                        }
                        if (gly.Height == 0)
                        {
                            gly.Height = 1;
                        }
                        gly.Baseline = typeinfo.Pt * fontBaselineMod;
                        gly.BitmapTop = gly.Baseline - aFace.Glyph.BitmapTop;                        
                        gly.BitmapBottom = gly.BitmapTop + gly.Height;

                        gly.BitmapLeft = aFace.Glyph.BitmapLeft;
                        gly.BitmapRight = gly.BitmapLeft + gly.Width;
                        gly.AdvanceX = aFace.Glyph.Advance.X.ToSingle();

                        //不知道是干啥的变量，但是似乎永远为0
                        gly.Zero = 0;

                        maxHeightInLine = Math.Max(maxHeightInLine, gly.Height);
                        //换行
                        if (leftMargin + gly.Width + 1 > imgSize)
                        {
                            leftMargin = 0;
                            topMargin += maxHeightInLine + 1;
                            maxHeightInLine = gly.Height;
                        }
                        //换页
                        if (topMargin + maxHeightInLine + 1 > imgSize)
                        {
                            topMargin = 0;
                            leftMargin = 0;
                            maxHeightInLine = gly.Height;

                            type.Images.Add(new Homeworld2.RCF.Image
                            {
                                Name = "",
                                Version = 1
                            });
                            type.Images.Last().ModifyBitmapData(imgSize, imgSize, imgdata);

                            //决定新的图片的大小
                            if (
                                imgSize == 4096 &&
                                typeinfo.Pt * typeinfo.Pt * (_font.CharsetCount - charaIndex) <= 2048 * 2048 * 3
                            )
                            {
                                imgSize = 1024;
                            }
                            if (
                                imgSize == 2048 &&
                                typeinfo.Pt * typeinfo.Pt * (_font.CharsetCount - charaIndex) <= 1024 * 1024 * 3
                            )
                            {
                                imgSize = 1024;
                            }
                            if (
                                imgSize == 1024 &&
                                typeinfo.Pt * typeinfo.Pt * (_font.CharsetCount - charaIndex) <= 512 * 512 * 3
                            )
                            {
                                imgSize = 512;
                            }
                            if (
                                imgSize == 512 &&
                                typeinfo.Pt * typeinfo.Pt * (_font.CharsetCount - charaIndex) <= 256 * 256 * 3
                            )
                            {
                                imgSize = 256;
                            }
                            if (
                                imgSize == 256 &&
                                typeinfo.Pt * typeinfo.Pt * (_font.CharsetCount - charaIndex) <= 128 * 128 * 3
                            )
                            {
                                imgSize = 128;
                            }
                            imgdata = new byte[imgSize * imgSize];
                            Array.Clear(imgdata, 0, imgSize * imgSize);
                            imgnum++;
                        }
                        //设置剩余的gly属性
                        gly.ImageIndex = imgnum;
                        gly.LeftMargin = leftMargin;
                        gly.TopMargin = topMargin;

                        for (int line = 0; line < aBmp.Rows; line++)
                        {
                            for (int i = 0; i < aBmp.Width; i++)
                            {
                                imgdata[(topMargin + line) * imgSize + leftMargin + i] =
                                    aBmp.BufferData[line * aBmp.Pitch + i];
                            }
                        }                        
                    }
                    //移动margin
                    leftMargin += gly.Width + 1;                    
                }
                //最后一页img还没有放入typeface内，得补上。              
                type.Images.Add(new Homeworld2.RCF.Image
                {
                    Name = "",
                    Version = 1
                });
                type.Images.Last().ModifyBitmapData(imgSize, imgSize, imgdata);
            }

            //刷新界面
            RefreshFontInfo();

            MessageBox.Show(this, @"done!");
        }

        //保存RCF文件
        private void button3_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog
            {
                Filter = @"RCF font files (*.rcf)|*.rcf",
                AddExtension = true
            }
            )
            {
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;
                using (Stream fontStream = dialog.OpenFile())
                {
                    _font.Write(fontStream);
                }
            }          
            MessageBox.Show(this, @"done!");
        }


        private class Typeinfo
        {
            public string Name;
            public int Pt;
            public bool Bold;
        }

        private readonly List<Typeinfo> _typeinfos = new List<Typeinfo>
        {
            new Typeinfo
            {
                Name = "Normal_400",
                Pt = 6,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Normal_640",
                Pt = 8,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Normal_800",
                Pt = 11,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Normal_1024",
                Pt = 16,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Normal_1280",
                Pt = 20,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Normal_1600",
                Pt = 22,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Normal_2048",
                Pt = 32,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Normal_3000",
                Pt = 40,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Normal_8000",
                Pt = 75,
                Bold = false
            },
            new Typeinfo
            {
                Name = "Bold_400",
                Pt = 6,
                Bold = true
            },
            new Typeinfo
            {
                Name = "Bold_640",
                Pt = 8,
                Bold = true
            },
            new Typeinfo
            {
                Name = "Bold_800",
                Pt = 11,
                Bold = true
            },
            new Typeinfo
            {
                Name = "Bold_1024",
                Pt = 16,
                Bold = true
            },
            new Typeinfo
            {
                Name = "Bold_1280",
                Pt = 20,
                Bold = true
            },
            new Typeinfo
            {
                Name = "Bold_1600",
                Pt = 22,
                Bold = true
            },
            new Typeinfo
            {
                Name = "Bold_2048",
                Pt = 32,
                Bold = true
            },
            new Typeinfo
            {
                Name = "Bold_3000",
                Pt = 40,
                Bold = true
            },
            new Typeinfo
            {
                Name = "Bold_8000",
                Pt = 75,
                Bold = true
            }
        };

        public class Form4AnswerType
        {
            public string NormalFontPath;
            public string BoldFontPath;
            public bool UseBoldFont;
            public string CharSet;
            public float FontBaselineMod;
            public bool UseSpecialFont4Ascii;
            public string AsciiNormalFontPath;
            public string AsciiBoldFontPath;
            public float AsciiFontBaselineMod;
        }

        public Form4AnswerType Form4Answer = new Form4AnswerType();

        //修改字体信息
        private void button4_Click(object sender, EventArgs e)
        {
            _font.Name = textBox1.Text;
            _font.Version = int.Parse(textBox8.Text);
        }

        //修改typeface信息
        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            Typeface selectedType = _font.Typefaces[listBox1.SelectedIndex];
            selectedType.Name = textBox2.Text;
            selectedType.Attributes = textBox9.Text;
        }

        //修改img信息
        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox2.SelectedIndex < 0)
            {
                return;
            }
            Homeworld2.RCF.Image img = _font.Typefaces[listBox1.SelectedIndex].Images[listBox2.SelectedIndex];
            img.Name = textBox3.Text;
            img.Version = int.Parse(textBox10.Text);
        }

        //修改单个字符信息
        private void button7_Click(object sender, EventArgs e)
        {
            if (
                textBox5.Text == "" ||
                listBox1.SelectedIndex < 0
            )
                return;

            var glyphs = from aGlyph in _font.Typefaces[listBox1.SelectedIndex].Glyphs
                where aGlyph.Character == textBox5.Text[0]
                select aGlyph;

            if (!glyphs.Any())
            {
                return;
            }

            Glyph glyph = glyphs.First();

            glyph.Height = int.Parse(textBox6.Text);
            glyph.Width = int.Parse(textBox13.Text);
            glyph.BitmapLeft = float.Parse(textBox14.Text);
            glyph.BitmapRight = float.Parse(textBox15.Text);
            glyph.AdvanceX = float.Parse(textBox16.Text);
            glyph.BitmapTop = float.Parse(textBox17.Text);
            glyph.Baseline = float.Parse(textBox18.Text);
            glyph.Baseline = float.Parse(textBox19.Text);
            glyph.TopMargin = int.Parse(textBox20.Text);
            glyph.ImageIndex = int.Parse(textBox21.Text);
            glyph.LeftMargin = int.Parse(textBox22.Text);
        }

        //导出选定图像
        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox2.SelectedIndex < 0)
            {
                return;
            }
            Homeworld2.RCF.Image img = _font.Typefaces[listBox1.SelectedIndex].Images[listBox2.SelectedIndex];
            Bitmap bmp = helper.type2img(img);
            using (var dialog = new SaveFileDialog
                {
                    Filter = @"PNG image files (*.png)|*.png",
                    AddExtension = true
                }
            )
            {
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;
                using (Stream pngStream = dialog.OpenFile())
                {
                    bmp.Save(pngStream,ImageFormat.Png);
                }
            }
        }

        //替换选定图像
        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox2.SelectedIndex < 0)
            {
                return;
            }
            using (var dialog = new OpenFileDialog
                {
                    Filter = @"PNG image files (*.png)|*.png",
                    CheckFileExists = true,
                    Multiselect = false
            }
            )
            {
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;
                using (Stream pngStream = dialog.OpenFile())
                {
                    var bmp = new Bitmap(pngStream);
                    _font.Typefaces[listBox1.SelectedIndex].Images[listBox2.SelectedIndex]
                        .ModifyBitmapData(bmp.Width, bmp.Height, helper.img2type(bmp));
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            _form5.Show();
        }
    }
}
