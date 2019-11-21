namespace NetBattle.Structure {
    public struct Cell2 {
        public static readonly Cell2 Right = new Cell2(1, 0);
        public static readonly Cell2 DownRight = new Cell2(1, -1);
        public static readonly Cell2 Down = new Cell2(0, -1);
        public static readonly Cell2 DownLeft = new Cell2(-1, -1);
        public static readonly Cell2 Left = new Cell2(-1, 0);
        public static readonly Cell2 UpLeft = new Cell2(-1, 1);
        public static readonly Cell2 Up = new Cell2(0, 1);
        public static readonly Cell2 UpRight = new Cell2(1, 1);
        public static readonly Cell2 Zero = new Cell2(0, 0);

        public int X { get; }
        public int Y { get; }

        public Cell2(int x, int y) {
            X = x;
            Y = y;
        }

        public static explicit operator long(Cell2 c) => ((long) c.X << 32) + c.Y;

        public static Cell2 operator +(Cell2 v) => v;
        public static Cell2 operator -(Cell2 v) => new Cell2(-v.X, -v.Y);
        public static Cell2 operator +(Cell2 v, Cell2 w) => new Cell2(v.X + w.X, v.Y + w.X);
        public static Cell2 operator -(Cell2 v, Cell2 w) => new Cell2(v.X - w.X, v.Y - w.Y);
        public static Cell2 operator *(Cell2 v, int f) => new Cell2(v.X * f, v.Y * f);
        public static Cell2 operator *(int f, Cell2 v) => new Cell2(v.X * f, v.Y * f);
    }
}