using System;

namespace NetBattle.Field {
    public class FTimer {
        public float Max { get; set; }
        public float Left { get; set; }
        public bool Enable { get; set; }
        public bool Finished => Math.Abs(Left) < float.Epsilon;

        public delegate void FinishedDelegate();

        public FinishedDelegate OnFinished;

        public FTimer(float max) {
            Max = max;
            Left = max;
        }

        public FTimer(float max, float left) {
            Max = max;
            Left = left;
        }

        public bool Update(float delta) {
            if (!Enable) return false;
            Left = Math.Max(Left - delta, 0.0f);
            if (Finished)
                OnFinished();
            return Finished;
        }

        public void Reset()
            => Left = Max;
    }
}