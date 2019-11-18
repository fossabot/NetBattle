using System;

namespace NetBattle.Field {
    public sealed class Health {
        private readonly int _overchargeMaximum;
        private readonly int _maximum;
        public int Value { get; set; }
        private readonly Action<int, int, FieldEntity> _onZero;

        public Health(int overchargeMaximum, int maximum, int value, Action<int, int, FieldEntity> onZero) {
            if (overchargeMaximum < 1)
                throw new ArgumentException($"Overcharge maximum {overchargeMaximum} must be positive");
            if (maximum < 1)
                throw new ArgumentException($"Maximum {maximum} must be positive");
            if (value < 0)
                throw new ArgumentException($"Value {value} cannot be negative");
            if (maximum > overchargeMaximum)
                throw new ArgumentException(
                    $"Maximum {maximum} cannot be greater than overcharge maximum {overchargeMaximum}");
            if (value > maximum)
                throw new ArgumentException($"Value {value} cannot be greater than maximum {maximum}");
            _overchargeMaximum = overchargeMaximum;
            _maximum = maximum;
            Value = value;
            _onZero = onZero;
        }

        public int Heal(int amount) {
            if (amount < 0)
                throw new ArgumentException($"Cannot heal negative value {amount}");
            Value = Math.Min(Value + amount, _overchargeMaximum);
            return Value;
        }

        public int Decay(int amount) {
            if (amount < 0)
                throw new ArgumentException($"Cannot decay negative value {amount}");
            if (Value < _maximum) return Value;
            Value = Math.Max(Value - amount, _maximum);
            return Value;
        }

        public int Damage(int amount, FieldEntity entity) {
            if (amount < 0)
                throw new ArgumentException($"Cannot decay negative value {amount}");
            Value -= amount;
            if (Value > 0) return Value;
            _onZero.Invoke(-Value, amount, entity?.ResolveTopEntity());
            return Value = 0;
        }
    }
}