namespace ShpRead
{
    public enum ShapeType
    {
        NullShape = 0, Point = 1,
        PolyLine = 3, Polygon = 5,
        Multipoint = 8, PointZ = 11,
        PolyLineZ = 13, PolygonZ = 15,
        MultipointZ = 18, PointM = 21,
        PolyLineM = 23, PolygonM = 25,
        MultipointM = 28, MultiPatch = 31
    }

    public interface IRecordHeader
    {
        int RecordNumber { get; set; }
        int ContentLength { get; set; }
        ShapeType ShapeType { get; set; }
    }

    public interface IGeometry
    {
    }

    public interface IPoint : IGeometry
    {
        double X { get; set; }
        double Y { get; set; }
    }

    public interface IPointM : IPoint
    {
        double M { get; set; }
    }

    public interface IPointZ : IPointM
    {
        double Z { get; set; }
    }

    [DebuggerDisplay("{ShapeType} (Xmin: {Xmin}; Ymin: {Ymin}; Xmax: {Xmax}; Ymax: {Ymax})")]
    [StructLayout(LayoutKind.Sequential)]
    public struct ShapeFileHeader
    {
        [System.Security.SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int Reverse(int value)
        {
            byte* p = (byte*)&value;
            return (*p << 24) | (p[1] << 16) | (p[2] << 8) | p[3];
        }

        private const int DefaultFileCode = 9994;
        private readonly int _FileCode;
        [DefaultValue(DefaultFileCode)] public int FileCode { get => Reverse(_FileCode); }

        private const int DefaultUnusedValue = 0;
        private readonly int _Unused1, _Unused2, _Unused3, _Unused4, _Unused5;
        [DefaultValue(DefaultUnusedValue)] public int Unused1 { get => Reverse(_Unused1); }
        [DefaultValue(DefaultUnusedValue)] public int Unused2 { get => Reverse(_Unused2); }
        [DefaultValue(DefaultUnusedValue)] public int Unused3 { get => Reverse(_Unused3); }
        [DefaultValue(DefaultUnusedValue)] public int Unused4 { get => Reverse(_Unused4); }
        [DefaultValue(DefaultUnusedValue)] public int Unused5 { get => Reverse(_Unused5); }

        private int _FileLength;
        [DefaultValue(0)] public int FileLenght { get => Reverse(_FileLength); set => _FileLength = Reverse(value); }

        private const int DefaultFileVersion = 1000;
        [DefaultValue(DefaultFileVersion)] public int Version { get; }

        [DefaultValue(ShapeType.NullShape)] public ShapeType ShapeType { get; }

        [DefaultValue(0.0)] public double Xmin { get; set; }
        [DefaultValue(0.0)] public double Ymin { get; set; }
        [DefaultValue(0.0)] public double Xmax { get; set; }
        [DefaultValue(0.0)] public double Ymax { get; set; }
        [DefaultValue(0.0)] public double Zmin { get; set; }
        [DefaultValue(0.0)] public double Zmax { get; set; }
        [DefaultValue(0.0)] public double Mmin { get; set; }
        [DefaultValue(0.0)] public double Mmax { get; set; }

        public ShapeFileHeader(int fileLenght, ShapeType shapeType, double xmin, double ymin, double xmax, double ymax, double zmin, double zmax, double mmin, double mmax) : this()
        {
            _FileCode = Reverse(DefaultFileCode);

            _Unused1 = DefaultUnusedValue;
            _Unused2 = DefaultUnusedValue;
            _Unused3 = DefaultUnusedValue;
            _Unused4 = DefaultUnusedValue;
            _Unused5 = DefaultUnusedValue;

            Version = DefaultFileVersion;


            FileLenght = fileLenght;
            ShapeType = shapeType;
            Xmin = xmin;
            Ymin = ymin;
            Xmax = xmax;
            Ymax = ymax;
            Zmin = zmin;
            Zmax = zmax;
            Mmin = mmin;
            Mmax = mmax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Security.SecuritySafeCritical]
        public unsafe ShapeFileHeader(in byte[] buffer)
        {
            fixed (byte* ptr = &buffer[0])
                this = *(ShapeFileHeader*)ptr;
        }
    }

    [DebuggerDisplay("ID: {RecordNumber}; Content length: {ContentLengthInBytes}")]
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct RecordHeader : IRecordHeader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Swap(int value)
        {
            //Original
            //uint v = (uint)value;
            //v = (v >> 16) | (v << 16);
            //return (int)(((v & 0xFF00FF00) >> 8) | ((v & 0x00FF00FF) << 8));

            //Optimized
            uint v = (uint)((int)((uint)value >> 16) | value << 16);
            return (int)((uint)((int)v & -16711936) >> 8 | (v & 0xFF00FF) << 8);
        }

        [FieldOffset(0)] private int _RecordNumber;
        [FieldOffset(4)] private int _ContentLength;
        [FieldOffset(8)] private ShapeType _ShapeType;

        public int RecordNumber { get => Swap(_RecordNumber); set => _RecordNumber = Swap(value); }
        public int ContentLength { get => Swap(_ContentLength)-1; set => _ContentLength = Swap(value)+1; }
        public ShapeType ShapeType { get => _ShapeType; set => _ShapeType = value; }
        public int ContentLengthInBytes { get => 2 * ContentLength; }

        [System.Security.SecuritySafeCritical]
        public unsafe RecordHeader(in byte[] buffer)
        {
            fixed (byte* ptr = &buffer[0])
                this = *(RecordHeader*)ptr;
        }
    }

    [DebuggerDisplay("X: {X}; Y:{Y}")]
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct Point : IPoint
    {
        [FieldOffset(4-4)] private double _X;
        [FieldOffset(12-4)] private double _Y;

        public double X { get => _X; set => _X = value; }
        public double Y { get => _Y; set => _Y = value; }

        [System.Security.SecuritySafeCritical]
        public unsafe Point(in byte[] buffer)
        {
            fixed (byte* ptr = &buffer[0])
                this = *(Point*)ptr;
        }
    }

    [DebuggerDisplay("X: {X}; Y:{Y}; M:{M}")]
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct PointM : IPointM
    {
        [FieldOffset(4-4)] private double _X;
        [FieldOffset(12-4)] private double _Y;
        [FieldOffset(20-4)] private double _M;

        public double X { get => _X; set => _X = value; }
        public double Y { get => _Y; set => _Y = value; }
        public double M { get => _M; set => _M = value; }

        [System.Security.SecuritySafeCritical]
        public unsafe PointM(in byte[] buffer)
        {
            fixed (byte* ptr = &buffer[0])
                this = *(PointM*)ptr;
        }
    }

    [DebuggerDisplay("X: {X}; Y:{Y}; Z:{Z}; M:{M}")]
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct PointZ : IPointZ
    {
        [FieldOffset(4-4)] private double _X;
        [FieldOffset(12-4)] private double _Y;
        [FieldOffset(20-4)] private double _Z;
        [FieldOffset(28-4)] private double _M;

        public double X { get => _X; set => _X = value; }
        public double Y { get => _Y; set => _Y = value; }
        public double Z { get => _Z; set => _Z = value; }
        public double M { get => _M; set => _M = value; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [System.Security.SecuritySafeCritical]
        public unsafe PointZ(in byte[] buffer)
        {
            fixed (byte* ptr = &buffer[0])
                this = *(PointZ*)ptr;
        }
    }


    public class ShapeFile
    {
        public ShapeFileHeader Header { get; set; }
        public unsafe ShapeFile(string path)
        {
            using (FileStream FS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
            {
                byte[] buffer = new byte[100];
                FS.Read(buffer, 0, 100);
                this.Header = new ShapeFileHeader(buffer);

                List<RecordHeader> recordHeaders = new List<RecordHeader>(256);
                List<IGeometry> geometries = new List<IGeometry>(256);

                long Length = FS.Length;

                while (FS.Position < Length)
                {
                    FS.Read(buffer, 0, 12);
                    RecordHeader recordHeader = new RecordHeader(buffer);
                    recordHeaders.Add(recordHeader);
                    switch (recordHeader.ShapeType)
                    {
                        case ShapeType.PointZ:
                            {
                                FS.Read(buffer, 0, 32);
                                geometries.Add(new PointZ(buffer));
                                break;
                            }
                        case ShapeType.NullShape:
                            {
                                geometries.Add(null);
                                break;
                            }
                    }
                }
                Debugger.Break();
            }
        }
    }
}
