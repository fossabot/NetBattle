using System;

namespace NetBattle.Field {
    public abstract class FieldEvent {
        public readonly string TargetOwner;
        public readonly Guid Target;
        public readonly Guid Source;
        public readonly bool HasTargetEntity;
        public readonly bool HasSourceEntity;
        
        public FieldEvent(string targetOwner) {
            Source = Guid.Empty;
            TargetOwner = targetOwner;
            Target = Guid.Empty;
            HasTargetEntity = false;
            HasSourceEntity = false;
        }

        public FieldEvent(Guid target) {
            Source = Guid.Empty;
            TargetOwner = null;
            Target = target;
            HasTargetEntity = true;
            HasSourceEntity = false;
        }

        public FieldEvent(string targetOwner, Guid source) {
            Source = source;
            TargetOwner = targetOwner;
            Target = Guid.Empty;
            HasTargetEntity = false;
            HasSourceEntity = true;
        }

        public FieldEvent(Guid target, Guid source) {
            Source = source;
            TargetOwner = null;
            Target = target;
            HasTargetEntity = true;
            HasSourceEntity = true;
        }
    }
}