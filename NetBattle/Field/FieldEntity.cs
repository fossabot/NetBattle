using System;
using System.Collections.Generic;
using NetBattle.Structure;

namespace NetBattle.Field {
    public abstract class FieldEntity {
        public FieldManager Manager { get; set; }
        public Owner Owner { get; set; }
        public FieldEntity Parent { get; set; }

        public readonly Guid Id = Guid.NewGuid();

        public Position Position { get; protected set; }
        public VisualState VisualState { get; protected set; }
        public InputTarget InputTarget { get; protected set; }
        public HitBox HitBox { get; protected set; }
        public Health Health { get; protected set; }

        protected readonly HashSet<IFieldEventHandler> EventHandlers = new HashSet<IFieldEventHandler>();

        public FieldEntity ResolveTopEntity() {
            return Parent == null ? this : Parent.ResolveTopEntity();
        }


        protected FieldEntity(Owner owner, FieldEntity parent = null) {
            Owner = owner;
            Parent = parent;
        }

        /// <summary>
        /// Standard callback 1/4 - post-registration
        /// </summary>
        /// <remarks>
        /// Callback is triggered when being
        /// processed in registration queue
        /// </remarks>
        public abstract void RegistrationPhase();

        /// <summary>
        /// Standard callback 2/4 - user/AI control
        /// </summary>
        /// <remarks>
        /// Callback is only triggered if this entity's
        /// owner is contained in update filter (if
        /// applicable)
        /// </remarks>
        public abstract void ControlPhase();

        /// <summary>
        /// Standard callback 3/4 - behaviour update
        /// </summary>
        /// <remarks>
        /// Callback is only triggered if this entity's
        /// owner is contained in update filter (if
        /// applicable)
        /// </remarks>
        public abstract void UpdatePhase();

        /// <summary>
        /// Standard callback 4/4 - 
        /// </summary>
        /// <remarks>
        /// Callback is triggered when being
        /// processed in deregistration queue
        /// </remarks>
        public abstract void DeregistrationPhase();


        /// <summary>
        /// Event callback
        /// </summary>
        /// <param name="evt">Event to process</param>
        /// <remarks>
        /// Callback is always triggered once for
        /// events
        /// </remarks>
        public void OnEvent(FieldEvent evt) {
            foreach (var handler in EventHandlers)
                if (handler.Handle(evt))
                    return;
        }
    }
}