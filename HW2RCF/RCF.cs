using System.Collections.Generic;
using System.IO;
using System.Text;
using Homeworld2.IFF;

namespace Homeworld2.RCF
{
    public class RCF
    {
        private string _name;
        private int _version;
        private byte[] _attributes;
        private int _charCount;
        private string _charset;

        private readonly List<Typeface> _typefaces = new List<Typeface>();

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int Version
        {
            get => _version;
            set => _version = value;
        }

        public int CharsetCount => _charCount;

        public string Charset
        {
            get => _charset;
            set
            {
                _charset = value;
                _charCount = _charset.Length;
            }
        }

        public byte[] Attributes
        {
            get => _attributes;
            set => _attributes = value;
        }

        public List<Typeface> Typefaces => _typefaces;

        private void ReadFONTChunk(IFFReader iff, ChunkAttributes attr)
        {
            iff.AddHandler(Chunks.Name, ChunkType.Default, ReadNAMEChunk);
            iff.AddHandler(Chunks.Version, ChunkType.Default, ReadVERSChunk);
            iff.AddHandler(Chunks.Attributes, ChunkType.Default, ReadATTRChunk);
            iff.AddHandler(Chunks.Charset, ChunkType.Default, ReadCSETChunk);

            iff.AddHandler(Chunks.Typeface, ChunkType.Form, ReadTYPEChunk);

            iff.Parse();
        }

        private void ReadVERSChunk(IFFReader iff, ChunkAttributes attr)
        {
            _version = iff.ReadInt32();
        }

        private void ReadATTRChunk(IFFReader iff, ChunkAttributes attr)
        {
            _attributes = iff.ReadBytes(attr.Size);
        }

        private void ReadNAMEChunk(IFFReader iff, ChunkAttributes attr)
        {
            _name = iff.ReadString();
        }

        private void ReadCSETChunk(IFFReader iff, ChunkAttributes attr)
        {
            _charCount = iff.ReadInt32();
            _charset = Encoding.Unicode.GetString(iff.ReadBytes(2 * _charCount));
        }

        private void ReadTYPEChunk(IFFReader iff, ChunkAttributes attr)
        {
            _typefaces.Add(Typeface.Read(iff));
        }

        public void Read(Stream stream)
        {
            _typefaces.Clear();

            var iff = new IFFReader(stream);
            iff.AddHandler(Chunks.Font, ChunkType.Form, ReadFONTChunk);
            iff.Parse();
        }

        public void Write(Stream stream)
        {
            var iff = new IFFWriter(stream);
            iff.Push(Chunks.Font, ChunkType.Form);

            iff.Push(Chunks.Name);
            iff.Write(_name);
            iff.Pop();

            iff.Push(Chunks.Version);
            iff.WriteInt32(_version);
            iff.Pop();

            iff.Push(Chunks.Attributes);
            iff.Write(_attributes);
            iff.Pop();

            iff.Push(Chunks.Charset);
            iff.WriteInt32(_charCount);
            iff.Write(Encoding.Unicode.GetBytes(_charset));
            iff.Pop();

            foreach (var typeface in _typefaces)
            {
                iff.Push(Chunks.Typeface, ChunkType.Form);
                typeface.Write(iff);
                iff.Pop();
            }

            iff.Pop();
        }
    }
}
