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
    }

    public interface IGeometry
    {
        ShapeType ShapeType { get; set; }
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

    [StructLayout(LayoutKind.Explicit, Size = 8)]
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

        public int RecordNumber { get => Swap(_RecordNumber); set => _RecordNumber = Swap(value); }
        public int ContentLength { get => Swap(_ContentLength); set => _ContentLength = Swap(value); }

        [System.Security.SecuritySafeCritical]
        public unsafe RecordHeader(in byte[] buffer, int offset)
        {
            fixed (byte* ptr = &buffer[offset])
                this = *(RecordHeader*)ptr;
        }

        [System.Security.SecuritySafeCritical]
        public unsafe RecordHeader(in byte[] buffer) : this(buffer, 0) { }
    }

    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct NullShape : IGeometry
    {
        [FieldOffset(0)] private ShapeType _ShapeType;
        public ShapeType ShapeType { get => _ShapeType; set => _ShapeType = value; }

        [System.Security.SecuritySafeCritical]
        public unsafe NullShape(in byte[] buffer, int offset)
        {
            fixed (byte* ptr = &buffer[offset])
                this = *(NullShape*)ptr;
        }

        [System.Security.SecuritySafeCritical]
        public unsafe NullShape(in byte[] buffer) : this(buffer, 0) { }
    }

    [StructLayout(LayoutKind.Explicit, Size = 20)]
    public struct Point : IPoint
    {
        [FieldOffset(0)] private ShapeType _ShapeType;
        [FieldOffset(4)] private double _X;
        [FieldOffset(12)] private double _Y;

        public ShapeType ShapeType { get => _ShapeType; set => _ShapeType = value; }
        public double X { get => _X; set => _X = value; }
        public double Y { get => _Y; set => _Y = value; }

        [System.Security.SecuritySafeCritical]
        public unsafe Point(in byte[] buffer, int offset)
        {
            fixed (byte* ptr = &buffer[offset])
                this = *(Point*)ptr;
        }

        [System.Security.SecuritySafeCritical]
        public unsafe Point(in byte[] buffer) : this(buffer, 0) { }
    }

    [StructLayout(LayoutKind.Explicit, Size = 28)]
    public struct PointM : IPointM
    {
        [FieldOffset(0)] private ShapeType _ShapeType;
        [FieldOffset(4)] private double _X;
        [FieldOffset(12)] private double _Y;
        [FieldOffset(20)] private double _M;

        public ShapeType ShapeType { get => _ShapeType; set => _ShapeType = value; }
        public double X { get => _X; set => _X = value; }
        public double Y { get => _Y; set => _Y = value; }
        public double M { get => _M; set => _M = value; }

        [System.Security.SecuritySafeCritical]
        public unsafe PointM(in byte[] buffer, int offset)
        {
            fixed (byte* ptr = &buffer[offset])
                this = *(PointM*)ptr;
        }

        [System.Security.SecuritySafeCritical]
        public unsafe PointM(in byte[] buffer) : this(buffer, 0) { }
    }

    [StructLayout(LayoutKind.Explicit, Size = 36)]
    public struct PointZ : IPointZ
    {
        [FieldOffset(0)] private ShapeType _ShapeType;
        [FieldOffset(4)] private double _X;
        [FieldOffset(12)] private double _Y;
        [FieldOffset(20)] private double _Z;
        [FieldOffset(28)] private double _M;

        public ShapeType ShapeType { get => _ShapeType; set => _ShapeType = value; }
        public double X { get => _X; set => _X = value; }
        public double Y { get => _Y; set => _Y = value; }
        public double Z { get => _Z; set => _Z = value; }
        public double M { get => _M; set => _M = value; }

        [System.Security.SecuritySafeCritical]
        public unsafe PointZ(in byte[] buffer, int offset)
        {
            fixed (byte* ptr = &buffer[offset])
                this = *(PointZ*)ptr;
        }

        [System.Security.SecuritySafeCritical]
        public unsafe PointZ(in byte[] buffer) : this(buffer, 0) { }
    }

    [StructLayout(LayoutKind.Explicit, Size = 100)]
    [DebuggerDisplay("{_ShapeType} (Xmin: {Xmin}; Ymin: {Ymin}; Xmax: {Xmax}; Ymax: {Ymax})")]
    public struct ShapeFileHeader
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

        [System.Security.SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static double NaNToMin(double value)
        {
            if ((*(ulong*)(&value) & 0x7FFFFFFFFFFFFFFF) <= 9218868437227405312uL)
                return value;
            return double.MinValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double MinToNaN(double value)
        {
            if (value == double.MinValue)
                return double.NaN;
            return value;
        }

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
        [FieldOffset(32)] private ShapeType _ShapeType;
        [FieldOffset(36)] private double _Xmin;
        [FieldOffset(44)] private double _Ymin;
        [FieldOffset(52)] private double _Xmax;
        [FieldOffset(60)] private double _Ymax;
        [FieldOffset(68)] private double _Zmin;
        [FieldOffset(76)] private double _Zmax;
        [FieldOffset(84)] private double _Mmin;
        [FieldOffset(92)] private double _Mmax;

        /* Big endian properties */
        public int FileCode { get => Swap(_FileCode); set => _FileCode = Swap(value); }
        public int Unused0 { get => Swap(_Unused0); set => _Unused0 = Swap(value); }
        public int Unused1 { get => Swap(_Unused1); set => _Unused1 = Swap(value); }
        public int Unused2 { get => Swap(_Unused2); set => _Unused2 = Swap(value); }
        public int Unused3 { get => Swap(_Unused3); set => _Unused3 = Swap(value); }
        public int Unused4 { get => Swap(_Unused4); set => _Unused4 = Swap(value); }
        public int FileLength { get => Swap(_FileLength); set => _FileLength = Swap(value); }

        /* Little endian properties */
        public int Version { get => _Version; set => _Version = value; }
        public ShapeType ShapeType { get => _ShapeType; set => _ShapeType = value; }
        public double Xmin { get => _Xmin; set => _Xmin = value; }
        public double Ymin { get => _Ymin; set => _Ymin = value; }
        public double Xmax { get => _Xmax; set => _Xmax = value; }
        public double Ymax { get => _Ymax; set => _Ymax = value; }
        public double Zmin { get => MinToNaN(_Zmin); set => _Zmin = NaNToMin(value); }
        public double Zmax { get => MinToNaN(_Zmax); set => _Zmax = NaNToMin(value); }
        public double Mmin { get => MinToNaN(_Mmin); set => _Mmin = NaNToMin(value); }
        public double Mmax { get => MinToNaN(_Mmax); set => _Mmax = NaNToMin(value); }

        [System.Security.SecuritySafeCritical]
        public unsafe ShapeFileHeader(in byte[] buffer)
        {
            fixed (byte* ptr = &buffer[0])
                this = *(ShapeFileHeader*)ptr;
        }
    }

    public class ShapeFile
    {
        public ShapeFileHeader Header { get; set; }
        public unsafe ShapeFile(string path)
        {
            using (System.IO.FileStream FS = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                List<IGeometry> geometries = new List<IGeometry>(256);

                byte[] buffer = new byte[100];
                FS.Read(buffer, 0, 100);
                Header = new ShapeFileHeader(buffer);

                byte[] recHeader = new byte[8];
                FS.Read(buffer, 0, 8);
                RecordHeader recordHeader = new RecordHeader(recHeader);

                byte[] pb = new byte[36];
                FS.Read(pb, 0, 36);
                IPointZ pz = new PointZ(pb);


                Debugger.Break();


                //switch (shptype)
                //{
                //    case ShapeType.NullShape:
                //        geometries.Add(new NullShape());
                //        goto Cs;
                //        break;
                //    case ShapeType.Point:
                //        break;
                //    case ShapeType.PolyLine:
                //        break;
                //    case ShapeType.Polygon:
                //        break;
                //    case ShapeType.Multipoint:
                //        break;
                //    case ShapeType.PointZ:
                //        break;
                //    case ShapeType.PolyLineZ:
                //        break;
                //    case ShapeType.PolygonZ:
                //        break;
                //    case ShapeType.MultipointZ:
                //        break;
                //    case ShapeType.PointM:
                //        break;
                //    case ShapeType.PolyLineM:
                //        break;
                //    case ShapeType.PolygonM:
                //        break;
                //    case ShapeType.MultipointM:
                //        break;
                //    case ShapeType.MultiPatch:
                //        break;
                //}

            }
        }
    }
}
