using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;
using NetBattle.Field.Entities;
using NetBattle.Field.Events;
using NetBattle.Structure;

namespace NetBattle.Field {
    public abstract class ScriptFieldEntity : FieldEntity {
        private static readonly Type[] DefaultTypes = {
            typeof(Guid),
            typeof(FieldEntity),
            typeof(FieldEvent),
            typeof(FieldManager),
            typeof(Health),
            typeof(IFieldEventHandler),
            typeof(InputTarget),
            typeof(ScriptFieldEntity),
            typeof(Cell2),
            typeof(FVector3),
            typeof(HitBox),
            typeof(InputEvent),
            typeof(Owner),
            typeof(Position),
            typeof(Sound),
            typeof(VisualState),
            typeof(BombEntity),
            typeof(NaviEntity),
            typeof(TemporaryEntity),
            typeof(CounterAcknowledgeEvent),
            typeof(HitAcknowledgeEvent),
        };

        static ScriptFieldEntity() {
            MsRegisterTypes(DefaultTypes);
        }

        public static void MsRegisterTypes(IEnumerable<Type> types) {
            if (types == null)
                throw new ArgumentNullException(nameof(types));
            foreach (var type in types)
                UserData.RegisterType(type);
        }

        [MoonSharpHidden]
        public static void MsHwDump(Stream stream) {
            using (var writer = new StreamWriter(stream)) {
                writer.Write(UserData.GetDescriptionOfRegisteredTypes(true).Serialize());
            }
        }

        public readonly Script Script;

        private const string DefaultScript = @"";
        private readonly HashSet<string> _scripts = new HashSet<string>();

        protected ScriptFieldEntity(Owner owner, FieldEntity parent = null) : base(owner, parent) {
            Script = new Script(CoreModules.Preset_HardSandbox);
            RegisterScript(DefaultScript);

            RegisterAction<FieldEntity>("mg_queue_registration", MgQueueRegistration);
            RegisterAction<FieldEntity>("mg_queue_deregistration", MgQueueDeregistration);
            RegisterFunction<Guid, FieldEntity>("mg_find_entity", MgFindEntity);
            RegisterFunction<Cell2, FieldEntity>("mg_check_primary_entity", MgCheckPrimaryEntity);
            RegisterFunction<Cell2, bool>("mg_check_warn_cell", MgCheckWarnCell);
            RegisterAction<FieldEvent>("mg_send_event", MgSendEvent);
            RegisterFunction<Cell2, Cell2, int, FieldEntity>("mg_scan_primary", MgScanPrimary);
            RegisterFunction<Cell2, Cell2, int, IEnumerable<FieldEntity>>("mg_scan_primary_multi", MgScanPrimaryMulti);
            RegisterAction<Sound>("mg_play_sound", MgPlaySound);
            RegisterAction<Sound>("mg_pause_sound", MgPauseSound);
            RegisterAction<Sound>("mg_stop_sound", MgStopSound);

            RegisterFunction<string, bool>("nt_has_timer", NtHasTimer);
            RegisterFunction<string, FTimer>("nt_get_timer", NtGetTimer);
            RegisterFunction<string, float, FTimer>("nt_add_timer", NtAddTimer);
            RegisterAction<string>("nt_remove_timer", NtRemoveTimer);
            RegisterAction<FieldEntity, Cell2, float>("ntx_configure_position_static", NtxConfigurePositionStatic);
            RegisterAction<FieldEntity, float, float, Cell2, Cell2, float, float>("ntx_configure_position_linear",
                NtxConfigurePositionLinear);
            RegisterAction<FieldEntity, float, float, Cell2, Cell2, float, float, float>("ntx_configure_position_quad",
                NtxConfigurePositionQuad);
            RegisterAction<FieldEntity, string, string, float, float, float, bool, bool>("ntx_configure_visual_state",
                NtxConfigureVisualState);
            RegisterAction<FieldEntity, Cell2, HitBox.AvoidType, bool, bool>("ntx_configure_hit_box",
                NtxConfigureHitBox);
        }

#pragma region FieldManager proxy functions
        private void MgQueueRegistration(FieldEntity entity) => Manager?.QueueRegistration(entity);
        private void MgQueueDeregistration(FieldEntity entity) => Manager?.QueueDeregistration(entity);
        private FieldEntity MgFindEntity(Guid guid) => Manager?.FindEntity(guid);
        private FieldEntity MgCheckPrimaryEntity(Cell2 cell) => Manager?.CheckPrimaryEntity(cell);
        private bool MgCheckWarnCell(Cell2 cell) => Manager?.CheckWarnCell(cell) ?? true;
        private void MgSendEvent(FieldEvent evt) => Manager?.SendEvent(evt);

        private FieldEntity MgScanPrimary(Cell2 startCell, Cell2 axis, int count) {
            for (var i = 0; i < count; i++) {
                var res = Manager?.CheckPrimaryEntity(startCell + axis * i);
                if (res != null)
                    return res;
            }

            return null;
        }

        private IEnumerable<FieldEntity> MgScanPrimaryMulti(Cell2 startCell, Cell2 axis,
            int count) {
            for (var i = 0; i < count; i++) {
                var res = Manager?.CheckPrimaryEntity(startCell + axis * i);
                if (res != null)
                    yield return res;
            }
        }

        private void MgPlaySound(Sound sound) => Manager?.SoundPlayHandler(sound);
        private void MgPauseSound(Sound sound) => Manager?.SoundPauseHandler(sound);
        private void MgStopSound(Sound sound) => Manager?.SoundStopHandler(sound);
#pragma endregion

#pragma region FieldEntity proxy funcitons
        private bool NtHasTimer(string name) => Timers.ContainsKey(name);
        private FTimer NtGetTimer(string name) => Timers.TryGetValue(name, out var res) ? res : null;

        private FTimer NtAddTimer(string name, float time) {
            var timer = new FTimer(time);
            Timers.Add(name, timer);
            return timer;
        }

        private void NtRemoveTimer(string name) => Timers.Remove(name);

        private static void NtxConfigurePositionStatic(FieldEntity entity, Cell2 cell, float height) =>
            entity.Position = new Position(new FVector3(cell.X, cell.Y, height));

        private static void NtxConfigurePositionLinear(FieldEntity entity, float baseTime, float targetTime,
            Cell2 srcCell,
            Cell2 targetCell, float height1, float height2) =>
            entity.Position = new Position(new FVector3(srcCell.X, srcCell.Y, height1),
                new FVector3(targetCell.X, targetCell.Y, height2), baseTime, targetTime);

        private static void NtxConfigurePositionQuad(FieldEntity entity, float baseTime, float targetTime,
            Cell2 srcCell,
            Cell2 targetCell, float height1, float height2, float arcHeight) {
            var a = new FVector3(srcCell.X, srcCell.Y, height1);
            var c = new FVector3(targetCell.X, targetCell.Y, height2);
            var b = FVector3.Midpoint(a, c);
            b = new FVector3(b.X, b.Y, arcHeight);
            entity.Position = new Position(a, b, c, baseTime, targetTime);
        }

        private static void NtxConfigureVisualState(FieldEntity entity, string asset, string animation, float baseTime,
            float speed, float warp, bool visible, bool enemyVisible) =>
            entity.VisualState = new VisualState(asset, animation, baseTime, speed, warp, visible, enemyVisible);

        private static void NtxConfigureHitBox(FieldEntity entity, Cell2 cell, HitBox.AvoidType avoidType,
            bool targetable, bool primary) => entity.HitBox = new HitBox(cell, avoidType, targetable, primary);
#pragma endregion

        public void CopySources(ScriptFieldEntity other) {
            foreach (var script in other._scripts)
                RegisterScript(script);
        }

        public void RegisterScript(string script) {
            if (_scripts.Add(script))
                Script.DoString(script);
        }

        public void RegisterAction(string name, Action action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1>(string name, Action<T1> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2>(string name, Action<T1, T2> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2, T3>(string name, Action<T1, T2, T3> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2, T3, T4, T5, T6>(string name, Action<T1, T2, T3, T4, T5, T6> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2, T3, T4, T5, T6, T7>(string name,
            Action<T1, T2, T3, T4, T5, T6, T7> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2, T3, T4, T5, T6, T7, T8>(string name,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string name,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action) =>
            Script.Globals[name] = action;

        public void RegisterAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string name,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action) =>
            Script.Globals[name] = action;

        public void RegisterFunction<TR>(string name, Func<TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, TR>(string name, Func<T1, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, TR>(string name, Func<T1, T2, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, T3, TR>(string name, Func<T1, T2, T3, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, T3, T4, TR>(string name, Func<T1, T2, T3, T4, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, T3, T4, T5, TR>(string name, Func<T1, T2, T3, T4, T5, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, T3, T4, T5, T6, TR>(string name,
            Func<T1, T2, T3, T4, T5, T6, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, T3, T4, T5, T6, T7, TR>(string name,
            Func<T1, T2, T3, T4, T5, T6, T7, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, T3, T4, T5, T6, T7, T8, TR>(string name,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>(string name,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR>(string name,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR> function) =>
            Script.Globals[name] = function;

        public void RunVoidScript(string script) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn);
        }

        public void RunVoidScript<T1>(string script, T1 v1) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1);
        }

        public void RunVoidScript<T1, T2>(string script, T1 v1, T2 v2) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2);
        }

        public void RunVoidScript<T1, T2, T3>(string script, T1 v1, T2 v2, T3 v3) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3);
        }

        public void RunVoidScript<T1, T2, T3, T4>(string script, T1 v1, T2 v2, T3 v3, T4 v4) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3, v4);
        }

        public void RunVoidScript<T1, T2, T3, T4, T5>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3, v4, v5);
        }

        public void RunVoidScript<T1, T2, T3, T4, T5, T6>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3, v4, v5, v6);
        }

        public void RunVoidScript<T1, T2, T3, T4, T5, T6, T7>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6,
            T7 v7) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3, v4, v5, v6, v7);
        }

        public void RunVoidScript<T1, T2, T3, T4, T5, T6, T7, T8>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5,
            T6 v6, T7 v7, T8 v8) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3, v4, v5, v6, v7, v8);
        }

        public void RunVoidScript<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5,
            T6 v6, T7 v7, T8 v8, T9 v9) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3, v4, v5, v6, v7, v8, v9);
        }

        public void RunVoidScript<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string script, T1 v1, T2 v2, T3 v3, T4 v4,
            T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10);
        }

        public TR RunScript<TR>(string script) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn).ToObject();
        }

        public TR RunScript<T1, TR>(string script, T1 v1) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1).ToObject();
        }

        public TR RunScript<T1, T2, TR>(string script, T1 v1, T2 v2) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2).ToObject();
        }

        public TR RunScript<T1, T2, T3, TR>(string script, T1 v1, T2 v2, T3 v3) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3).ToObject();
        }

        public TR RunScript<T1, T2, T3, T4, TR>(string script, T1 v1, T2 v2, T3 v3, T4 v4) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4).ToObject();
        }

        public TR RunScript<T1, T2, T3, T4, T5, TR>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4, v5).ToObject();
        }

        public TR RunScript<T1, T2, T3, T4, T5, T6, TR>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4, v5, v6).ToObject();
        }

        public TR RunScript<T1, T2, T3, T4, T5, T6, T7, TR>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6,
            T7 v7) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4, v5, v6, v7).ToObject();
        }

        public TR RunScript<T1, T2, T3, T4, T5, T6, T7, T8, TR>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6,
            T7 v7, T8 v8) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4, v5, v6, v7, v8).ToObject();
        }

        public TR RunScript<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>(string script, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5,
            T6 v6, T7 v7, T8 v8, T9 v9) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4, v5, v6, v7, v8, v9).ToObject();
        }

        public TR RunScript<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR>(string script, T1 v1, T2 v2, T3 v3, T4 v4,
            T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10) {
            Script.Globals["entity"] = this;
            Script.Globals["manager"] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10).ToObject();
        }
    }
}