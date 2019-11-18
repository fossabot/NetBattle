namespace NetBattle.Structure {
    public struct InputEvent {
        public float Time { get; }
        public InputKey Key { get; }
        public bool Down { get; }

        public InputEvent(float time, InputKey key, bool down) {
            Time = time;
            Key = key;
            Down = down;
        }

        public enum InputKey {
            Up,
            Down,
            Left,
            Right,
            AtkAChip,
            AtkBBuster,
            DefShield,
            SpecialLock
        }
    }
}