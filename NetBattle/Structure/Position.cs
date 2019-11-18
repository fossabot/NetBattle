using System;

namespace NetBattle.Structure {
    public struct Position {
        private readonly FVector3 _a;
        private readonly FVector3 _b;
        private readonly FVector3 _c;
        public TweenType Tween { get; }
        public float BaseTime { get; }
        public float TargetTime { get; }

        public Position(FVector3 basePosition) {
            _a = basePosition;
            _b = basePosition;
            _c = basePosition;
            Tween = TweenType.None;
            BaseTime = 0.0f;
            TargetTime = 0.0f;
        }

        public Position(FVector3 basePosition, FVector3 targetPosition, float baseTime, float targetTime) {
            if (baseTime > targetTime)
                throw new ArgumentException($"Base time {baseTime} cannot be larger than target time {targetTime}");
            _a = basePosition;
            _b = FVector3.Zero;
            _c = targetPosition;
            Tween = TweenType.Linear;
            BaseTime = baseTime;
            TargetTime = targetTime;
        }

        public Position(FVector3 basePosition, FVector3 intermediatePosition, FVector3 targetPosition, float baseTime,
            float targetTime) {
            if (baseTime > targetTime)
                throw new ArgumentException($"Base time {baseTime} cannot be larger than target time {targetTime}");
            QuadraticReg(0.0f, 0.5f, 0.1f, basePosition.X, intermediatePosition.X, targetPosition.X,
                out var xA, out var xB, out var xC);
            QuadraticReg(0.0f, 0.5f, 0.1f, basePosition.Y, intermediatePosition.Y, targetPosition.Y,
                out var yA, out var yB, out var yC);
            QuadraticReg(0.0f, 0.5f, 0.1f, basePosition.Z, intermediatePosition.Z, targetPosition.Z,
                out var zA, out var zB, out var zC);
            _a = new FVector3(xA, yA, zA);
            _b = new FVector3(xB, yB, zB);
            _c = new FVector3(xC, yC, zC);
            Tween = TweenType.Quadratic;
            BaseTime = baseTime;
            TargetTime = targetTime;
        }

        public FVector3 GetPositionAtTime(float time = 0.0f) {
            switch (Tween) {
                case TweenType.None:
                    if (time < BaseTime)
                        return _a;
                    return time > TargetTime ? _c : _a;
                case TweenType.Linear:
                    if (time < BaseTime)
                        return _a;
                    if (time > TargetTime)
                        return _c;
                    return _a + (_b - _a) * (time - BaseTime) / (TargetTime - BaseTime);
                case TweenType.Quadratic:
                    if (time < BaseTime)
                        time = BaseTime;
                    else if (time > TargetTime)
                        time = TargetTime;
                    var percent = (time - BaseTime) / (TargetTime - BaseTime);
                    return percent * percent * _a + _b * percent + _c;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void QuadraticReg(float x1, float x2, float x3, float y1, float y2, float y3, out float a,
            out float b, out float c) {
            var d = (x1 - x2) * (x1 - x3) * (x2 - x3);
            a = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / d;
            b = (x3 * x3 * (y1 - y2) + x2 * x2 * (y3 - y1) + x1 * x1 * (y2 - y3)) / d;
            c = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / d;
        }

        public enum TweenType {
            None,
            Linear,
            Quadratic
        }
    }
}