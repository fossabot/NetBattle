using System;

namespace NetBattle.Structure {
    public struct HitBox {
        public Cell2 Position { get; }
        public AvoidType Avoid { get; }
        public bool Targetable { get; }
        public bool Primary { get; }
        
        public HitBox(Cell2 position, AvoidType avoidType, bool targetable, bool primary = true) {
            Position = position;
            Avoid = avoidType;
            Targetable = targetable;
            Primary = primary;
        }

        [Flags]
        public enum AvoidType {
            None = 0x0,
            Invis = 0x1,
            RealInvis = 0x1 << 1,
            Protect = 0x1 << 2,
        }
    }
}