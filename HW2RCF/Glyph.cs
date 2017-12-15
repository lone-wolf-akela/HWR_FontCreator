using System.Text;
using System.Windows;
using Homeworld2.IFF;

namespace Homeworld2.RCF
{
    public class Glyph
    {
        private float _tmp1;
        private float _tmp2;
        private byte _temp;

        public char Character { get; set; }
        public int ImageIndex { get; set; }
        public int LeftMargin { get; set; }
        public int TopMargin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float WidthInPoints { get; set; }
        public float FloatWidth { get; set; }
        public float HeightInPoints { get; set; }
        public float FloatHeight { get; set; }

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
                _tmp1 = iff.ReadSingle(),
                WidthInPoints = iff.ReadSingle(),
                FloatWidth = iff.ReadSingle(),
                _tmp2 = iff.ReadSingle(),
                HeightInPoints = iff.ReadSingle(),
                FloatHeight = iff.ReadSingle(),
                _temp = iff.ReadByte()
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

            iff.Write(_tmp1);
            iff.Write(WidthInPoints);
            iff.Write(FloatWidth);
            iff.Write(_tmp2);
            iff.Write(HeightInPoints);
            iff.Write(FloatHeight);
            iff.Write(_temp);
        }
    }
}
