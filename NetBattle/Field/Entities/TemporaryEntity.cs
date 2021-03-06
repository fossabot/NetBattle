using NetBattle.Structure;

namespace NetBattle.Field.Entities {
    public class TemporaryEntity : FieldEntity {
        public float TargetTime { get; set; }

        public TemporaryEntity(Owner owner, FieldEntity parent = null) : base(owner, parent) {
        }

        public override void RegistrationPhase() {
        }

        public override void ControlPhase() {
        }

        public override void UpdatePhase() {
            if (Manager.Time > TargetTime)
                Manager.QueueDeregistration(this);
        }

        public override void DeregistrationPhase() {
        }
    }
}