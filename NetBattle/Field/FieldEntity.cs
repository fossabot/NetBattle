using System;
using System.Collections.Generic;
using NetBattle.Structure;

namespace NetBattle.Field {
    public abstract class FieldEntity {
        public FieldManager Manager { get; internal set; }
        public FieldEntity Parent { get; set; }

        public readonly Guid Id = Guid.NewGuid();

        public Position Position { get; set; }
        public VisualState VisualState { get; set; }
        public InputTarget InputTarget { get; set; }

        public Owner Owner {
            get => _owner;
            set => SetOwner(value);
        }

        public HitBox HitBox {
            get => _hitBox;
            set => SetHitBox(value);
        }

        public ICollection<Cell2> WarnCells {
            get => _warnCells;
            set => SetWarnCells(value);
        }

        public Health Health { get; set; } = new Health();
        private Owner _owner;
        private HitBox _hitBox;
        private ICollection<Cell2> _warnCells;

        public readonly HashSet<IFieldEventHandler> EventHandlers = new HashSet<IFieldEventHandler>();
        public readonly Dictionary<string, FTimer> Timers = new Dictionary<string, FTimer>();

        public FieldEntity ResolveTopEntity() => Parent == null ? this :
            Parent != this ? Parent.ResolveTopEntity() : throw new Exception("Cyclical entity chain");

        protected FieldEntity(Owner owner, FieldEntity parent = null) {
            Owner = owner;
            Parent = parent;
        }

        private void SetOwner(Owner owner) {
            Manager?.EntityOwnerChange(this, _owner, owner);
            _owner = owner;
        }

        private void SetHitBox(HitBox hitBox) {
            if (Manager?.EntityHitBoxChange(this, _hitBox, hitBox) ?? false)
                _hitBox = hitBox;
        }

        private void SetWarnCells(ICollection<Cell2> warnCells) {
            Manager?.EntityWarnCellChange(this, _warnCells, warnCells);
            _warnCells = warnCells;
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

        internal void BaseUpdatePhase() {
            if (Manager == null) return;
            var delta = Manager.DeltaTime;
            foreach (var e in Timers)
                e.Value.Update(delta);
            UpdatePhase();
        }

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