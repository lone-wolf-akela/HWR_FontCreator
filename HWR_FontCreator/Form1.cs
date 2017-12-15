using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Homeworld2.RCF;
using Glyph = Homeworld2.RCF.Glyph;


namespace HWR_FontCreator
{
    public partial class Form1 : Form
    {
        private RCF font = new RCF();
        private Form2 form2;
        private Form3 form3;
        public Form1()
        {
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
                    font.Read(fontStream);
                }
            }
            

            listBox1.Items.Clear();
            listBox2.Items.Clear();

            for (int i = 1; i <= font.Typefaces.Count; i++)
            {
                listBox1.Items.Add(i);
            }

            textBox1.Text =
                $@"Name: {font.Name}
CharsetCount: {font.CharsetCount}
Version: {font.Version}";

            textBox4.Text = font.Charset;
        }

        //选择typeface
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            Typeface selectedType = font.Typefaces[listBox1.SelectedIndex];
            listBox2.Items.Clear();
            for (int i = 1; i <= selectedType.Images.Count; i++)
            {
                listBox2.Items.Add(i);
            }
            textBox2.Text =
                $@"Name: {selectedType.Name}
Attr: {selectedType.Attributes}";
        }

        //选择image
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox2.SelectedIndex < 0)
            {
                return;
            }
            Homeworld2.RCF.Image img = font.Typefaces[listBox1.SelectedIndex].Images[listBox2.SelectedIndex];
            form2.pictureBox1.Image = helper.type2img(img);
            textBox3.Text =
                $@"Name: {img.Name}
Version: {img.Version}
Width: {img.Width}
Height: {img.Height}";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            form2 = new Form2();
            form2.Show();
            form3 = new Form3();
            form3.Show();
        }

        //选择特定文字
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (
                textBox5.Text == "" ||
                listBox1.SelectedIndex < 0
            )
                return;

            var glyphs = from __glyph in font.Typefaces[listBox1.SelectedIndex].Glyphs
                         where __glyph.Character == textBox5.Text[0]
                         select __glyph;

            if (!glyphs.Any())
                return;

            Glyph glyph = glyphs.First();

            Homeworld2.RCF.Image img =
                font.Typefaces[listBox1.SelectedIndex].Images[glyph.ImageIndex];

            Bitmap bmp = helper.type2img(img);
            Rectangle rect = new Rectangle(
                glyph.LeftMargin,
                glyph.TopMargin,
                glyph.Width,
                glyph.Height
            );

            form3.pictureBox1.Image = helper.type2img(img).Clone(rect, bmp.PixelFormat);

            textBox6.Text =
                $@"Height: {glyph.Height}
Width: {glyph.Width}
FloatHeight: {glyph.FloatHeight}
FloatWidth: {glyph.FloatWidth}
HeightInPoints: {glyph.HeightInPoints}
WidthInPoints: {glyph.WidthInPoints}";
        }

        //导入字体文件
        private void button2_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            if (form4.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            form4.Dispose();

            var aLibraryN = new SharpFont.Library();
            var aFaceN = aLibraryN.NewFace(form4answer.normalFontPath, 0);
            SharpFont.Face aFaceB = null;
            if (form4answer.useBoldFont)
            {
                var aLibraryB = new SharpFont.Library();
                aFaceB = aLibraryB.NewFace(form4answer.boldFontPath, 0);
            }
            for (int i = 0; i < form4answer.charSet.Length; i++)
            {
                if (aFaceN.GetCharIndex(form4answer.charSet[i]) == 0)
                {
                    form4answer.charSet = form4answer.charSet.Remove(i, 1);
                    i--;
                }
            }

            font.Charset = form4answer.charSet;
            font.Name = "default";
            font.Version = 1;
            if (form4answer.useBoldFont)
            {
                font.Attributes = new byte[] { 14, 0, 255, 0, 0 };
            }
            else
            {
                font.Attributes = new byte[] { 14, 0, 0, 0, 0 };
            }

            font.Typefaces.Clear();
            int Font_Size_Mod = form4answer.fontSizeMod;
            double Margin_Mod = form4answer.fontMarginMod;

            //对每一种type
            foreach (var typeinfo in typeinfos)
            {
                SharpFont.Face aFace;
                if (typeinfo.bold)
                {
                    if (form4answer.useBoldFont)
                    {
                        aFace = aFaceB;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    aFace = aFaceN;
                }

                //建立typeface
                font.Typefaces.Add(new Typeface
                {
                    Name = typeinfo.Name,
                    Attributes = typeinfo.Attr
                });
                var type = font.Typefaces.Last();

                //决定图片大小
                int IMG_SIZE = 128;
                if (typeinfo.height * typeinfo.height * font.CharsetCount > 128 * 128 * 5)
                {
                    IMG_SIZE = 256;
                }
                if (typeinfo.height * typeinfo.height * font.CharsetCount > 256 * 256 * 5)
                {
                    IMG_SIZE = 512;
                }
                if (typeinfo.height * typeinfo.height * font.CharsetCount > 512 * 512 * 5)
                {
                    IMG_SIZE = 1024;
                }
                if (typeinfo.height * typeinfo.height * font.CharsetCount > 1024 * 1024 * 5)
                {
                    IMG_SIZE = 2048;
                }
                if (typeinfo.height * typeinfo.height * font.CharsetCount > 2048 * 2048 * 5)
                {
                    IMG_SIZE = 4096;
                }

                int topMargin = 0;
                int leftMargin = 0;

                var imgdata = new byte[IMG_SIZE * IMG_SIZE];
                Array.Clear(imgdata, 0, IMG_SIZE * IMG_SIZE);
                int imgnum = 0;

                for (int charaIndex = 0; charaIndex < font.CharsetCount; charaIndex++)
                {
                    var chara = font.Charset[charaIndex];

                    //设置渲染字体大小
                    if (chara >= 'a' && chara <= 'z')
                    {
                        //英文小写字母最好格外再小一点
                        aFace.SetPixelSizes(0, (uint)(typeinfo.height + Font_Size_Mod - 1));
                    }
                    else
                    {
                        aFace.SetPixelSizes(0, (uint)(typeinfo.height + Font_Size_Mod));
                    }
                    
                    type.Glyphs.Add(new Glyph
                    {
                        Character = chara
                    });
                    var gly = type.Glyphs.Last();

                    //渲染
                    aFace.LoadChar(chara, SharpFont.LoadFlags.Default, SharpFont.LoadTarget.Normal);
                    aFace.Glyph.RenderGlyph(SharpFont.RenderMode.Normal);
                    using (SharpFont.FTBitmap aBmp = aFace.Glyph.Bitmap)
                    {
                        //设置gly相应属性
                        gly.Height = typeinfo.height;
                        gly.Width = (aFace.Glyph.LinearHorizontalAdvance.Value >> 16) + 1;
                        if (gly.Width == 0)
                        {
                            gly.Width = 1;
                        }
                        gly.FloatHeight = typeinfo.height;
                        gly.FloatWidth = aFace.Glyph.LinearHorizontalAdvance.Value / 65536f;
                        gly.HeightInPoints = typeinfo.height;
                        gly.WidthInPoints = aFace.Glyph.LinearHorizontalAdvance.Value / 65536f;

                        //换行
                        if (leftMargin + gly.Width + 1 > IMG_SIZE)
                        {
                            leftMargin = 0;
                            topMargin += typeinfo.height + 1;
                        }
                        //换页
                        if (topMargin + typeinfo.height + 2 > IMG_SIZE)
                        {
                            topMargin = 0;
                            leftMargin = 0;

                            type.Images.Add(new Homeworld2.RCF.Image
                            {
                                Name = "",
                                Version = 1
                            });
                            type.Images.Last().ModifyBitmapData(IMG_SIZE, IMG_SIZE, imgdata);

                            //决定新的图片的大小
                            if (
                                IMG_SIZE == 4096 &&
                                typeinfo.height * typeinfo.height * (font.CharsetCount - charaIndex) <= 2048 * 2048 * 3
                            )
                            {
                                IMG_SIZE = 1024;
                            }
                            if (
                                IMG_SIZE == 2048 &&
                                typeinfo.height * typeinfo.height * (font.CharsetCount - charaIndex) <= 1024 * 1024 * 3
                            )
                            {
                                IMG_SIZE = 1024;
                            }
                            if (
                                IMG_SIZE == 1024 &&
                                typeinfo.height * typeinfo.height * (font.CharsetCount - charaIndex) <= 512 * 512 * 3
                            )
                            {
                                IMG_SIZE = 512;
                            }
                            if (
                                IMG_SIZE == 512 &&
                                typeinfo.height * typeinfo.height * (font.CharsetCount - charaIndex) <= 256 * 256 * 3
                            )
                            {
                                IMG_SIZE = 256;
                            }
                            if (
                                IMG_SIZE == 256 &&
                                typeinfo.height * typeinfo.height * (font.CharsetCount - charaIndex) <= 128 * 128 * 3
                            )
                            {
                                IMG_SIZE = 128;
                            }
                            imgdata = new byte[IMG_SIZE * IMG_SIZE];
                            Array.Clear(imgdata, 0, IMG_SIZE * IMG_SIZE);
                            imgnum++;
                        }
                        //设置剩余的gly属性
                        gly.ImageIndex = imgnum;
                        gly.LeftMargin = leftMargin;
                        gly.TopMargin = topMargin;

                        //计算下笔位置
                        int pen_x = leftMargin + aFace.Glyph.BitmapLeft;
                        int pen_y;
                        if (chara >= 'a' && chara <= 'z')
                        {
                            //英文小写字母的位置特殊处理，尽量使其显得不要那么奇怪。。。
                            pen_y = Math.Max(topMargin,
                                topMargin + (aFace.Glyph.LinearVerticalAdvance.Value >> 16) - aFace.Glyph.BitmapTop +
                                (int) (Margin_Mod * typeinfo.height)
                            );
                        }
                        else
                        {
                            pen_y = Math.Max(topMargin,
                                topMargin + typeinfo.height - aFace.Glyph.BitmapTop +
                                (int)(Margin_Mod * typeinfo.height)
                            );
                        }

                        if (pen_y + aBmp.Rows - topMargin > typeinfo.height)
                        {
                            pen_y = Math.Max(topMargin,
                                topMargin + typeinfo.height - aBmp.Rows
                            );
                        }

                        int pen_height = Math.Min(aBmp.Rows, typeinfo.height - (pen_y - topMargin));

                        for (int line = 0; line < pen_height; line++)
                        {
                            for (int i = 0; i < aBmp.Width; i++)
                            {
                                imgdata[(pen_y + line) * IMG_SIZE + pen_x + i] =
                                    aBmp.BufferData[line * aBmp.Pitch + i];
                            }
                        }                        
                    }
                    //移动margin
                    leftMargin += gly.Width + 2;                    
                }
                //最后一页img还没有放入typeface内，得补上。              
                type.Images.Add(new Homeworld2.RCF.Image
                {
                    Name = "",
                    Version = 1
                });
                type.Images.Last().ModifyBitmapData(IMG_SIZE, IMG_SIZE, imgdata);
            }



            //刷新界面
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            for (int i = 1; i <= font.Typefaces.Count; i++)
            {
                listBox1.Items.Add(i);
            }

            textBox1.Text =
                $@"Name: {font.Name}
CharsetCount: {font.CharsetCount}
Version: {font.Version}";

            textBox4.Text = font.Charset;

            MessageBox.Show(this, "done!");
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
                    font.Write(fontStream);
                }
            }          
            MessageBox.Show(this, "done!");
        }


        private class Typeinfo
        {
            public string Name;
            public string Attr;
            public int height;
            public bool bold;
        }

        private readonly List<Typeinfo> typeinfos = new List<Typeinfo>
        {
            new Typeinfo
            {
                Name = "Normal_400",
                Attr = "Blender Pro Thin, 6 pt, Normal, Default, Smooth",
                height = 12,
                bold = false
            },
            new Typeinfo
            {
                Name = "Normal_640",
                Attr = "Blender Pro Thin, 8 pt, Normal, Default, Smooth",
                height = 14,
                bold = false
            },
            new Typeinfo
            {
                Name = "Normal_800",
                Attr = "Blender Pro Medium, 11 pt, Normal, Default, Smooth",
                height = 16,
                bold = false
            },
            new Typeinfo
            {
                Name = "Normal_1024",
                Attr = "Blender Pro Medium, 16 pt, Normal, Default, Smooth",
                height = 18,
                bold = false
            },
            new Typeinfo
            {
                Name = "Normal_1280",
                Attr = "Blender Pro Medium, 20 pt, Normal, Default, Smooth",
                height = 18,
                bold = false
            },
            new Typeinfo
            {
                Name = "Normal_1600",
                Attr = "Blender Pro Book, 22 pt, Thin, Default, Smooth",
                height = 20,
                bold = false
            },
            new Typeinfo
            {
                Name = "Normal_2048",
                Attr = "Blender Pro Book, 32 pt, Thin, Default, Smooth",
                height = 25,
                bold = false
            },
            new Typeinfo
            {
                Name = "Normal_3000",
                Attr = "Blender Pro Book, 40 pt, Thin, Default, Smooth",
                height = 28,
                bold = false
            },
            new Typeinfo
            {
                Name = "Normal_8000",
                Attr = "Blender Pro Book, 75 pt, Thin, Default, Smooth",
                height = 58,
                bold = false
            },
            new Typeinfo
            {
                Name = "Bold_400",
                Attr = "Blender Pro Bold, 6 pt, Bold, Default, Smooth",
                height = 12,
                bold = true
            },
            new Typeinfo
            {
                Name = "Bold_640",
                Attr = "Blender Pro Bold, 8 pt, Bold, Default, Smooth",
                height = 14,
                bold = true
            },
            new Typeinfo
            {
                Name = "Bold_800",
                Attr = "Blender Pro Bold, 11 pt, Bold, Default, Smooth",
                height = 16,
                bold = true
            },
            new Typeinfo
            {
                Name = "Bold_1024",
                Attr = "Blender Pro Medium, 16 pt, Normal, Default, Smooth",
                height = 18,
                bold = true
            },
            new Typeinfo
            {
                Name = "Normal_1280",
                Attr = "Blender Pro Bold, 16 pt, Bold, Default, Smooth",
                height = 18,
                bold = true
            },
            new Typeinfo
            {
                Name = "Bold_1600",
                Attr = "Blender Pro Book, 22 pt, Thin, Default, Smooth",
                height = 20,
                bold = true
            },
            new Typeinfo
            {
                Name = "Bold_2048",
                Attr = "Blender Pro Bold, 32 pt, Bold, Default, Smooth",
                height = 25,
                bold = true
            },
            new Typeinfo
            {
                Name = "Bold_3000",
                Attr = "Blender Pro Bold, 40 pt, Bold, Default, Smooth",
                height = 28,
                bold = true
            },
            new Typeinfo
            {
                Name = "Bold_8000",
                Attr = "Blender Pro Bold, 75 pt, Bold, Default, Smooth",
                height = 58,
                bold = true
            }
        };

        public class Form4Answer
        {
            public string normalFontPath;
            public string boldFontPath;
            public bool useBoldFont;
            public string charSet;
            public int fontSizeMod;
            public double fontMarginMod;
        }

        public Form4Answer form4answer = new Form4Answer();
    }
}
