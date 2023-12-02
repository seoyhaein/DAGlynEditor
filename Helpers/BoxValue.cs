using Avalonia;

namespace DAGlynEditor
{
    public static class BoxValue
    {
        public static readonly object Point = default(Point);
        public static readonly object Size = default(Size);
        public static readonly object Rect = default(Rect);
        public static readonly object False = false;
        public static readonly object True = true;
        public static readonly object DoubleHalf = 0.5d;
        public static readonly object Double0 = 0d;
        public static readonly object Double1 = 1d;
        public static readonly object Double2 = 2d;
        public static readonly object Double45 = 45d;
        public static readonly object Double1000 = 1000d;
        public static readonly object Int0 = 0;
        public static readonly object Int1 = 1;
        public static readonly object UInt1 = 1u;
        // object 를 Size 로 고쳤음. 이후 테스트 해봐야 함.
        public static readonly Size ArrowSize = new Size(7, 6);
    }
}
