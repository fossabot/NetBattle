using NetBattle.Structure;

namespace NetBattle.Field.Entities {
    public class BombEntity : TemporaryEntity {
        public BombEntity(Owner owner, FieldEntity parent = null) : base(owner, parent) {
        }

        public override void RegistrationPhase() {
        }

        public override void ControlPhase() {
        }

        public override void UpdatePhase() {
            if (Manager.Time > TargetTime) {
                (Parent as NaviEntity)?.DetonateBomb(this);
                Manager.QueueDeregistration(this);
            }
        }

        public override void DeregistrationPhase() {
        }
    }
}