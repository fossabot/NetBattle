using System;

namespace NetBattle.Field {
    public sealed class Health {
        public int OverchargeMaximum { get; set; }
        public int Maximum { get; set; }
        public int Value { get; set; }

        public Health(int overchargeMaximum = 100, int maximum = 100, int value = 100) {
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
            OverchargeMaximum = overchargeMaximum;
            Maximum = maximum;
            Value = value;
        }

        public int Heal(int amount) {
            if (amount < 0)
                throw new ArgumentException($"Cannot heal negative value {amount}");
            Value = Math.Min(Value + amount, OverchargeMaximum);
            return Value;
        }

        public int Decay(int amount) {
            if (amount < 0)
                throw new ArgumentException($"Cannot decay negative value {amount}");
            if (Value < Maximum) return Value;
            Value = Math.Max(Value - amount, Maximum);
            return Value;
        }

        public int Damage(int amount) {
            if (amount < 0)
                throw new ArgumentException($"Cannot decay negative value {amount}");
            return Value = Math.Max(Value - amount, 0);
        }
    }
}