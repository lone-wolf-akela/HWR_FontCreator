using System.Text;
using System.Windows;
using Homeworld2.IFF;

namespace Homeworld2.RCF
{
    public class Glyph
    {
        public float BitmapLeft { get; set; }
        public float BitmapTop { get; set; }
        public byte Zero { get; set; }

        public char Character { get; set; }
        public int ImageIndex { get; set; }
        public int LeftMargin { get; set; }
        public int TopMargin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float BitmapRight { get; set; }
        public float AdvanceX { get; set; }
        public float Baseline { get; set; }
        public float BitmapBottom { get; set; }

        public static Glyph Read(IFFReader iff)
        {
            var glyph = new Glyph
            {
                Character = Encoding.Unicode.GetChars(iff.ReadBytes(2))[0],
                ImageIndex = iff.ReadInt32(),
                LeftMargin = iff.ReadInt32(),
                TopMargin = iff.ReadInt32(),
                Width = iff.ReadInt32(),
                Height = iff.ReadInt32(),
                BitmapLeft = iff.ReadSingle(),
                BitmapRight = iff.ReadSingle(),
                AdvanceX = iff.ReadSingle(),
                BitmapTop = iff.ReadSingle(),
                Baseline = iff.ReadSingle(),
                BitmapBottom = iff.ReadSingle(),
                Zero = iff.ReadByte()
            };

            return glyph;
        }

        public void Write(IFFWriter iff)
        {
            iff.Write(Encoding.Unicode.GetBytes(new[] { Character }));
            iff.WriteInt32(ImageIndex);
            iff.WriteInt32(LeftMargin);
            iff.WriteInt32(TopMargin);
            iff.WriteInt32(Width);
            iff.WriteInt32(Height);

            iff.Write(BitmapLeft);
            iff.Write(BitmapRight);
            iff.Write(AdvanceX);
            iff.Write(BitmapTop);
            iff.Write(Baseline);
            iff.Write(BitmapBottom);
            iff.Write(Zero);
        }
    }
}
