namespace ShpRead
{
    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("Xmin: {Xmin}; Ymin: {Ymin}")]
    public struct ShapeFileHeader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _swap(uint value) =>
#if BIGENDIAN
            return value;
#else
           (int)((value >> 24) & 0xffU | ((value << 8) & 0xff0000U) | ((value >> 8) & 0xff00U) | ((value << 24) & 0xff000000U));
#endif


        /* Big endian fields */
        [FieldOffset(0)] private int _FileCode;
        [FieldOffset(4)] private int _Unused0;
        [FieldOffset(8)] private int _Unused1;
        [FieldOffset(12)] private int _Unused2;
        [FieldOffset(16)] private int _Unused3;
        [FieldOffset(20)] private int _Unused4;
        [FieldOffset(24)] private int _FileLength;
        [FieldOffset(28)] private int _Version;

        /* Little endian fields */
        [FieldOffset(32)] private int _ShapeType;
        [FieldOffset(36)] private double _Xmin;
        [FieldOffset(44)] private double _Ymin;
        [FieldOffset(52)] private double _Xmax;
        [FieldOffset(60)] private double _Ymax;
        [FieldOffset(68)] private double _Zmin;
        [FieldOffset(76)] private double _Zmax;
        [FieldOffset(84)] private double _Mmin;
        [FieldOffset(92)] private double _Mmax;

        /* Big endian properties */
        public int FileCode { get => _swap((uint)_FileCode); set => _FileCode = _swap((uint)value); }
        public int Unused0 { get => _swap((uint)_Unused0); set => _Unused0 = _swap((uint)value); }
        public int Unused1 { get => _swap((uint)_Unused1); set => _Unused1 = _swap((uint)value); }
        public int Unused2 { get => _swap((uint)_Unused2); set => _Unused2 = _swap((uint)value); }
        public int Unused3 { get => _swap((uint)_Unused3); set => _Unused3 = _swap((uint)value); }
        public int Unused4 { get => _swap((uint)_Unused4); set => _Unused4 = _swap((uint)value); }
        public int FileLength { get => _swap((uint)_FileLength); set => _FileLength = _swap((uint)value); }

        /* Little endian properties */
        public int Version { get => _Version; set => _Version = value; }
        public int ShapeType { get => _ShapeType; set => _ShapeType = value; }
        public double Xmin { get => _Xmin; set => _Xmin = value; }
        public double Ymin { get => _Ymin; set => _Ymin = value; }
        public double Xmax { get => _Xmax; set => _Xmax = value; }
        public double Ymax { get => _Ymax; set => _Ymax = value; }
        public double Zmin
        {
            get => _Zmin == double.MinValue ? double.NaN : _Zmin;
            set => _Zmin = double.IsNaN(value) ? double.MinValue : value;
        }
        public double Zmax { get => _Zmax; set => _Zmax = value; }
        public double Mmin { get => _Mmin; set => _Mmin = value; }
        public double Mmax { get => _Mmax; set => _Mmax = value; }

        public unsafe ShapeFileHeader(in byte[] buffer)
        {
            fixed (byte* ptr = &buffer[0])
                this = *(ShapeFileHeader*)ptr;
        }
    }
}
