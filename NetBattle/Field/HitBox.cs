using NetBattle.Structure;

namespace NetBattle.Field {
    public abstract class HitBox {
        public bool AntiHit { get; protected set; }
        public Cell2 Position { get; protected set; }
    }
}