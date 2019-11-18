namespace NetBattle.Structure {
    public struct Owner {
        public static readonly Owner None = new Owner(null);
        public static readonly Owner Default = new Owner("Default");

        public readonly string BaseName;
        public readonly int Variant;

        public Owner(string baseName, int variant = 0) {
            BaseName = baseName;
            Variant = variant;
        }
    }
}