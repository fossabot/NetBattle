using System;

namespace NetBattle.Field.Events {
    public class CounterAcknowledgeEvent : FieldEvent {
        // TODO add event data
        
        public CounterAcknowledgeEvent(string targetOwner) : base(targetOwner) {
        }

        public CounterAcknowledgeEvent(Guid target) : base(target) {
        }

        public CounterAcknowledgeEvent(string targetOwner, Guid source) : base(targetOwner, source) {
        }

        public CounterAcknowledgeEvent(Guid target, Guid source) : base(target, source) {
        }
    }
}