using NetBattle.Structure;

namespace NetBattle.Field {
    public abstract class InputTarget {
        public abstract void QueueInputEvent(InputEvent e);
    }
}