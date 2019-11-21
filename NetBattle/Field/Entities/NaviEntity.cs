using NetBattle.Structure;

namespace NetBattle.Field.Entities {
    public class NaviEntity : ScriptFieldEntity {
        public int Team { get; set; }

        public NaviEntity(Owner owner, FieldEntity parent = null) : base(owner, parent) {
        }

        public void DetonateBomb(FieldEntity entity) {
            // TODO implement bomb detonation logic
        }

        public override void RegistrationPhase() {
            throw new System.NotImplementedException();
        }

        public override void ControlPhase() {
            throw new System.NotImplementedException();
        }

        public override void UpdatePhase() {
            throw new System.NotImplementedException();
        }

        public override void DeregistrationPhase() {
            throw new System.NotImplementedException();
        }
    }
}