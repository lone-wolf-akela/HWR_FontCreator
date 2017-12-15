using System;
using Homeworld2.IFF;
using System.Windows;

namespace Homeworld2.RCF
{
    public class Image
    {
        public string Name { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int DataSize { get; private set; }
        public byte[] Data { get; private set; }
        public int Version { get; set; }

        private void ReadNAMEChunk(IFFReader iff, ChunkAttributes attr)
        {
            Name = iff.ReadString();
        }

        private void ReadATTRChunk(IFFReader iff, ChunkAttributes attr)
        {
            Version = iff.ReadInt32();
            Width = iff.ReadInt32();
            Height = iff.ReadInt32();
            DataSize = iff.ReadInt32();
        }

        private void ReadDATAChunk(IFFReader iff, ChunkAttributes attr)
        {
            Data = iff.ReadBytes(attr.Size);
        }

        public static Image Read(IFFReader iff)
        {
            var image = new Image();
            iff.AddHandler(Chunks.Name, ChunkType.Default, image.ReadNAMEChunk);
            iff.AddHandler(Chunks.Attributes, ChunkType.Default, image.ReadATTRChunk);
            iff.AddHandler(Chunks.Data, ChunkType.Default, image.ReadDATAChunk);
            iff.Parse();
            return image;
        }

        public void Write(IFFWriter iff)
        {
            iff.Push(Chunks.Name);
            iff.Write(Name);
            iff.Pop();

            iff.Push(Chunks.Attributes);
            iff.WriteInt32(Version);
            iff.WriteInt32(Width);
            iff.WriteInt32(Height);
            iff.WriteInt32(DataSize);
            iff.Pop();

            iff.Push(Chunks.Data);
            iff.Write(Data);
            iff.Pop();
        }

        public void ModifyBitmapData(int width, int height, byte[] data)
        {
            if (data.Length != width * height)
                throw new ArgumentException("Data length must be equal to width * height.");

            Width = width;
            Height = height;
            DataSize = data.Length;
            Data = data;
        }
    }
}
