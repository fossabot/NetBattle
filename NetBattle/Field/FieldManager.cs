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
        private readonly Dictionary<long, FieldEntity> _gridEntities = new Dictionary<long, FieldEntity>();

        private readonly Dictionary<long, HashSet<FieldEntity>> _gridWarnCells =
            new Dictionary<long, HashSet<FieldEntity>>();

        public void QueueRegistration(FieldEntity entity) => _regQueue.Enqueue(entity);
        public void QueueDeregistration(FieldEntity entity) => _deregQueue.Enqueue(entity);

        public delegate void SoundPlayHandler(Sound sound);

        public delegate void SoundPauseHandler(Sound sound);

        public delegate void SoundStopHandler(Sound sound);

        public FieldManager(int width, int height) {
            Width = width;
            Height = height;
        }

        public bool EntityHitBoxChange(FieldEntity entity, HitBox previousHitBox, HitBox targetHitBox) {
            var targetPos = (long) targetHitBox.Position;
            if (_gridEntities.TryGetValue(targetPos, out var res))
                return res == entity;
            _gridEntities.Remove((long) previousHitBox.Position);
            _gridEntities[targetPos] = entity;
            return true;
        }

        public void EntityWarnCellChange(FieldEntity entity, ICollection<Cell2> previousWarnCells,
            ICollection<Cell2> targetWarnCells) {
            foreach (var cell in previousWarnCells) {
                if (!_gridWarnCells.TryGetValue((long) cell, out var set)) continue;
                set.Remove(entity);
                if (set.Count == 0)
                    _gridWarnCells.Remove((long) cell);
            }

            foreach (var cell in targetWarnCells) {
                if (!_gridWarnCells.TryGetValue((long) cell, out var set))
                    set = _gridWarnCells[(long) cell] = new HashSet<FieldEntity>();
                set.Add(entity);
            }
        }

        private void AddFieldEntity(FieldEntity entity) {
            _fieldEntities.Add(entity);
            _fieldEntitiesGuid.Add(entity.Id, entity);
            entity.Manager = this;
            var bOwner = entity.Owner.BaseName;
            if (_ownerDict.TryGetValue(bOwner, out var entities))
                entities.Add(entity);
            else
                _ownerDict[bOwner] = new HashSet<FieldEntity> {entity};
        }

        private void RemoveFieldEntity(FieldEntity entity) {
            if (entity.Manager != this) return;
            entity.Manager = null;
            ClearFieldEntityOwner(entity);
            _fieldEntities.Remove(entity);
            _fieldEntitiesGuid.Remove(entity.Id);
            // TODO remove from grid
        }

        public void UpdateFieldEntityOwner(FieldEntity entity, Owner newOwner) {
            if (entity.Manager != this) return;
            if (entity.Owner.BaseName != null)
                ClearFieldEntityOwner(entity);
            entity.Owner = newOwner;
            if (_ownerDict.TryGetValue(newOwner.BaseName, out var entities))
                entities.Add(entity);
            else
                _ownerDict[newOwner.BaseName] = new HashSet<FieldEntity> {entity};
        }

        private void ClearFieldEntityOwner(FieldEntity entity) {
            if (entity.Manager != this) return;
            var empty = new List<string>();
            foreach (var e in _ownerDict) {
                e.Value.Remove(entity);
                if (e.Value.Count == 0)
                    empty.Add(e.Key);
            }

            foreach (var e in empty)
                _ownerDict.Remove(e);
            entity.Owner = Owner.None;
        }

        public void DisavowOwner(Owner owner) => _ownerDict.Remove(owner.BaseName);

        public FieldEntity FindEntity(Guid id) => _fieldEntitiesGuid.TryGetValue(id, out var res) ? res : null;

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

        public void RunSequence(float time) {
            DeltaTime = time - Time;
            Time = time;
            while (_regQueue.Count != 0) {
                var ent = _regQueue.Dequeue();
                AddFieldEntity(ent);
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
                            entity.UpdatePhase();
                }

            while (_deregQueue.Count != 0) {
                var ent = _deregQueue.Dequeue();
                ent.DeregistrationPhase();
                RemoveFieldEntity(ent);
            }
        }

        public IEnumerator<FieldEntity> GetEntityEnumerator()
            => _fieldEntities.GetEnumerator();
    }
}