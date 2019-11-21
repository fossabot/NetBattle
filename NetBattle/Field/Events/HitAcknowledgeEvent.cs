using System;

namespace NetBattle.Field.Events {
    public class HitAcknowledgeEvent : FieldEvent {
        // TODO add event data
        
        public HitAcknowledgeEvent(string targetOwner) : base(targetOwner) {
        }

        public HitAcknowledgeEvent(Guid target) : base(target) {
        }

        public HitAcknowledgeEvent(string targetOwner, Guid source) : base(targetOwner, source) {
        }

        public HitAcknowledgeEvent(Guid target, Guid source) : base(target, source) {
        }
    }
}