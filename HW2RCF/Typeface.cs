using System.Collections.Generic;
using Homeworld2.IFF;

namespace Homeworld2.RCF
{
    public class Typeface
    {
        private readonly List<Image> _images = new List<Image>();
        private readonly List<Glyph> _glyphs = new List<Glyph>();

        public string Name { get; set; }
        public string Attributes { get; set; }

        public List<Image> Images
        {
            get { return _images; }
        }

        public List<Glyph> Glyphs
        {
            get { return _glyphs; }
        }

        private void ReadNAMEChunk(IFFReader iff, ChunkAttributes attr)
        {
            Name = iff.ReadString();
        }

        private void ReadATTRChunk(IFFReader iff, ChunkAttributes attr)
        {
            Attributes = iff.ReadString();
        }

        private void ReadIMAGChunk(IFFReader iff, ChunkAttributes attr)
        {
            _images.Add(Image.Read(iff));
        }

        private void ReadGLPHChunk(IFFReader iff, ChunkAttributes attr)
        {
            _glyphs.Add(Glyph.Read(iff));
        }

        public static Typeface Read(IFFReader iff)
        {
            var typeface = new Typeface();
            iff.AddHandler(Chunks.Name, ChunkType.Default, typeface.ReadNAMEChunk);
            iff.AddHandler(Chunks.Attributes, ChunkType.Default, typeface.ReadATTRChunk);

            iff.AddHandler(Chunks.Image, ChunkType.Form, typeface.ReadIMAGChunk);
            iff.AddHandler(Chunks.Glyph, ChunkType.Default, typeface.ReadGLPHChunk);

            iff.Parse();
            return typeface;
        }

        public void Write(IFFWriter iff)
        {
            iff.Push(Chunks.Name);
            iff.Write(Name);
            iff.Pop();

            iff.Push(Chunks.Attributes);
            iff.Write(Attributes);
            iff.Pop();

            foreach (var image in _images)
            {
                iff.Push(Chunks.Image, ChunkType.Form);
                image.Write(iff);
                iff.Pop();
            }

            foreach (var glyph in _glyphs)
            {
                iff.Push(Chunks.Glyph);
                glyph.Write(iff);
                iff.Pop();
            }
        }
    }
}
