using Xamarin.Forms;

// ReSharper disable InconsistentNaming

namespace UISleuth.Tests.Reactions.SetProperties
{
    public class PageWithPrimitives : ContentPage
    {
        public char Char { get; set; }
        public float Float { get; set; }
        public byte Byte { get; set; }
        public short Short { get; set; }
        public ushort UShort { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }
        public uint UInt { get; set; }
        public ulong ULong { get; set; }

        public char? NChar { get; set; } = 'C';
        public float? NFloat { get; set; } = 1.0f;
        public byte? NByte { get; set; } = 23;
        public short? NShort { get; set; } = 125;
        public ushort? NUShort { get; set; } = 22;
        public int? NInt { get; set; } = 8;
        public long? NLong { get; set; } = 9;
        public uint? NUInt { get; set; } = 10;
        public ulong? NULong { get; set; } = 11;
    }
}
