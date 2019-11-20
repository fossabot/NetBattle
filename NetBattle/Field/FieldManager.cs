using System;
using System.Collections.Generic;
using NetBattle.Structure;

namespace NetBattle.Field {
    public sealed class FieldManager {
        public int Width { get; }
        public int Height { get; }
        public IEnumerable<Tuple<Owner, bool>> ControlFilter { get; set; }
        public IEnumerable<Tuple<Owner, bool>> UpdateFilter { get; set; }
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }

        private readonly HashSet<FieldEntity> _fieldEntities = new HashSet<FieldEntity>();
        private readonly Dictionary<Guid, FieldEntity> _fieldEntitiesGuid = new Dictionary<Guid, FieldEntity>();

        private readonly Dictionary<string, HashSet<FieldEntity>> _ownerDict =
            new Dictionary<string, HashSet<FieldEntity>>();

        private readonly Queue<FieldEntity> _regQueue = new Queue<FieldEntity>();
        private readonly Queue<FieldEntity> _deregQueue = new Queue<FieldEntity>();
        private readonly Dictionary<long, FieldEntity> _gridPrimaryEntities = new Dictionary<long, FieldEntity>();

        private readonly Dictionary<long, HashSet<FieldEntity>> _gridEntities =
            new Dictionary<long, HashSet<FieldEntity>>();

        private readonly Dictionary<long, HashSet<FieldEntity>> _gridWarnCells =
            new Dictionary<long, HashSet<FieldEntity>>();

        public void QueueRegistration(FieldEntity entity) => _regQueue.Enqueue(entity);
        public void QueueDeregistration(FieldEntity entity) => _deregQueue.Enqueue(entity);

        public delegate void SoundPlayDelegate(Sound sound);

        public delegate void SoundPauseDelegate(Sound sound);

        public delegate void SoundStopDelegate(Sound sound);

        public SoundPlayDelegate SoundPlayHandler;
        public SoundPauseDelegate SoundPauseHandler;
        public SoundStopDelegate SoundStopHandler;

        public FieldManager(int width, int height) {
            Width = width;
            Height = height;
        }

        internal bool EntityHitBoxChange(FieldEntity entity, HitBox previousHitBox, HitBox targetHitBox) {
            if (entity.Manager != this) return false;
            var targetPos = (long) targetHitBox.Position;
            if (targetHitBox.Primary && _gridPrimaryEntities.TryGetValue(targetPos, out var res))
                return res == entity;
            RemoveEntityFromGrid(previousHitBox, entity);
            AddEntityToGrid(targetHitBox, entity);
            return true;
        }

        internal void EntityWarnCellChange(FieldEntity entity, ICollection<Cell2> previousWarnCells,
            ICollection<Cell2> targetWarnCells) {
            if (entity.Manager != this) return;
            RemoveEntityWarnCells(previousWarnCells, entity);
            AddEntityWarnCells(targetWarnCells, entity);
        }

        internal void EntityOwnerChange(FieldEntity entity, Owner previousOwner, Owner targetOwner) {
            if (entity.Manager != this) return;
            RemoveEntityFromOwnerDict(previousOwner, entity);
            AddEntityToOwnerDict(targetOwner, entity);
        }

        private void AddEntity(FieldEntity entity) {
            if (entity.Manager == this) return;
            entity.Manager = this;
            AddEntityToOwnerDict(entity.Owner, entity);
            AddEntityToGrid(entity.HitBox, entity);
            _fieldEntities.Add(entity);
            _fieldEntitiesGuid.Add(entity.Id, entity);
            AddEntityWarnCells(entity.WarnCells, entity);
            var bOwner = entity.Owner.BaseName;
            if (bOwner == null) return;
            if (_ownerDict.TryGetValue(bOwner, out var entities))
                entities.Add(entity);
            else
                _ownerDict[bOwner] = new HashSet<FieldEntity> {entity};
        }

        private void RemoveEntity(FieldEntity entity) {
            if (entity.Manager != this) return;
            entity.Manager = null;
            RemoveEntityFromOwnerDict(entity.Owner, entity);
            RemoveEntityFromGrid(entity.HitBox, entity);
            _fieldEntities.Remove(entity);
            _fieldEntitiesGuid.Remove(entity.Id);
            RemoveEntityWarnCells(entity.WarnCells, entity);
            if (!entity.HitBox.Primary) return;
            var pos = (long) entity.HitBox.Position;
            if (_gridPrimaryEntities.TryGetValue(pos, out var gridEntity) && gridEntity == entity)
                _gridPrimaryEntities.Remove(pos);
        }

        private void AddEntityWarnCells(ICollection<Cell2> warnCells, FieldEntity entity) {
            if (warnCells == null) return;
            foreach (var cell in warnCells) {
                if (!_gridWarnCells.TryGetValue((long) cell, out var set))
                    set = _gridWarnCells[(long) cell] = new HashSet<FieldEntity>();
                set.Add(entity);
            }
        }

        private void RemoveEntityWarnCells(ICollection<Cell2> warnCells, FieldEntity entity) {
            if (warnCells == null) return;
            foreach (var cell in warnCells) {
                if (!_gridWarnCells.TryGetValue((long) cell, out var set)) continue;
                set.Remove(entity);
                if (set.Count == 0)
                    _gridWarnCells.Remove((long) cell);
            }
        }

        private void AddEntityToOwnerDict(Owner owner, FieldEntity entity) {
            if (owner.BaseName == null) return;
            if (!_ownerDict.TryGetValue(owner.BaseName, out var entities))
                entities = _ownerDict[owner.BaseName] = new HashSet<FieldEntity>();
            entities.Add(entity);
        }

        private void RemoveEntityFromOwnerDict(Owner owner, FieldEntity entity) {
            if (owner.BaseName == null) return;
            if (!_ownerDict.TryGetValue(owner.BaseName, out var set)) return;
            set.Remove(entity);
            if (set.Count == 0)
                _ownerDict.Remove(owner.BaseName);
        }

        private void AddEntityToGrid(HitBox hitBox, FieldEntity entity) {
            var cell = (long) hitBox.Position;
            if (!_gridEntities.TryGetValue(cell, out var set))
                set = _gridEntities[cell] = new HashSet<FieldEntity>();
            set.Add(entity);
            if (!hitBox.Primary) return;
            _gridPrimaryEntities[cell] = entity;
        }

        private void RemoveEntityFromGrid(HitBox hitBox, FieldEntity entity) {
            var cell = (long) hitBox.Position;
            if (!_gridEntities.TryGetValue(cell, out var set)) return;
            set.Remove(entity);
            if (set.Count == 0)
                _gridEntities.Remove(cell);
            if (hitBox.Primary && _gridPrimaryEntities.TryGetValue(cell, out var res) && res == entity)
                _gridPrimaryEntities.Remove(cell);
        }

        public void DisavowOwner(Owner owner) => _ownerDict.Remove(owner.BaseName);

        public FieldEntity FindEntity(Guid id) => _fieldEntitiesGuid.TryGetValue(id, out var res) ? res : null;

        public FieldEntity CheckPrimaryEntity(Cell2 cell) =>
            _gridPrimaryEntities.TryGetValue((long) cell, out var res) ? res : null;

        public bool CheckWarnCell(Cell2 cell) => _gridWarnCells.ContainsKey((long) cell);

        public void QueueInputEvent(InputEvent evt, string owner) {
            if (!_ownerDict.ContainsKey(owner)) return;
            foreach (var f in _ownerDict[owner])
                f.InputTarget?.QueueInputEvent(evt);
        }

        public void SendEvent(FieldEvent evt) {
            if (evt.HasTargetEntity && _fieldEntitiesGuid.TryGetValue(evt.Target, out var ent)) {
                ent.OnEvent(evt);
                return;
            }

            if (!_ownerDict.TryGetValue(evt.TargetOwner, out var entries)) return;
            foreach (var entry in entries)
                entry.OnEvent(evt);
        }

        public void UpdateField(float time) {
            DeltaTime = time - Time;
            Time = time;
            while (_regQueue.Count != 0) {
                var ent = _regQueue.Dequeue();
                AddEntity(ent);
                ent.RegistrationPhase();
            }

            if (ControlFilter == null)
                foreach (var entity in _fieldEntities)
                    entity.ControlPhase();
            else
                foreach (var (filterOwner, filterAllVariants) in ControlFilter) {
                    if (!_ownerDict.TryGetValue(filterOwner.BaseName, out var oEntry)) continue;
                    foreach (var entity in oEntry)
                        if (filterAllVariants || entity.Owner.Variant == filterOwner.Variant)
                            entity.ControlPhase();
                }

            if (UpdateFilter == null)
                foreach (var entity in _fieldEntities)
                    entity.UpdatePhase();
            else
                foreach (var (filterOwner, filterAllVariants) in UpdateFilter) {
                    if (!_ownerDict.TryGetValue(filterOwner.BaseName, out var oEntry)) continue;
                    foreach (var entity in oEntry)
                        if (filterAllVariants || entity.Owner.Variant == filterOwner.Variant)
                            entity.BaseUpdatePhase();
                }

            while (_deregQueue.Count != 0) {
                var ent = _deregQueue.Dequeue();
                ent.DeregistrationPhase();
                RemoveEntity(ent);
            }
        }

        public IEnumerator<FieldEntity> GetEntityEnumerator()
            => _fieldEntities.GetEnumerator();
    }
}