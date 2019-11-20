using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;
using NetBattle.Structure;

namespace NetBattle.Field.Entity {
    public abstract class ScriptFieldEntity : FieldEntity {
        private static readonly Type[] DefaultTypes = {
            typeof(Guid),
            typeof(ScriptFieldEntity),
            typeof(FieldEntity),
            typeof(FieldEvent),
            typeof(FieldManager),
            typeof(Health),
            typeof(IFieldEventHandler),
            typeof(InputTarget),
            typeof(Cell2),
            typeof(FVector3),
            typeof(HitBox),
            typeof(InputEvent),
            typeof(Owner),
            typeof(Position),
            typeof(Sound),
            typeof(VisualState),
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

        public const string ScriptKeyEntity = "entity";
        public const string ScriptKeyManager = "manager";
        public readonly Script Script;

        private const string DefaultScript = @"";
        private readonly HashSet<string> _scripts = new HashSet<string>();

        protected ScriptFieldEntity(FieldEntity parent = null) : this(Owner.None, parent) {
        }

        protected ScriptFieldEntity(Owner owner, FieldEntity parent = null) : base(owner, parent) {
            Script = new Script(CoreModules.Preset_HardSandbox);
            RegisterScript(DefaultScript);
            RegisterAction<FieldEntity>("mg_queue_registration", MgQueueRegistration);
            RegisterAction<FieldEntity>("mg_queue_deregistration", MgQueueDeregistration);
            RegisterFunction<Guid, FieldEntity>("mg_find_entity", MgFindEntity);
            RegisterFunction<Cell2, FieldEntity>("mg_check_primary_entity", MgCheckPrimaryEntity);
            RegisterFunction<Cell2, bool>("mg_check_warn_cell", MgCheckWarnCell);
            RegisterAction<FieldEvent>("mg_send_event", MgSendEvent);
            RegisterFunction<string, bool>("nt_has_timer", NtHasTimer);
            RegisterFunction<string, FTimer>("nt_get_timer", NtGetTimer);
            RegisterFunction<string, float, FTimer>("nt_add_timer", NtAddTimer);
            RegisterAction<string>("nt_remove_timer", NtRemoveTimer);
        }

#pragma region FieldManager proxy functions
        private void MgQueueRegistration(FieldEntity entity) => Manager?.QueueRegistration(entity);
        private void MgQueueDeregistration(FieldEntity entity) => Manager?.QueueDeregistration(entity);
        private FieldEntity MgFindEntity(Guid guid) => Manager?.FindEntity(guid);
        private FieldEntity MgCheckPrimaryEntity(Cell2 cell) => Manager?.CheckPrimaryEntity(cell);
        private bool MgCheckWarnCell(Cell2 cell) => Manager?.CheckWarnCell(cell) ?? true;
        private void MgSendEvent(FieldEvent evt) => Manager?.SendEvent(evt);
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

        public void RunVoidScript(string script) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn);
        }

        public void RunVoidScript<T1>(string script, T1 v1) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1);
        }

        public void RunVoidScript<T1, T2>(string script, T1 v1, T2 v2) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2);
        }

        public void RunVoidScript<T1, T2, T3>(string script, T1 v1, T2 v2, T3 v3) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3);
        }

        public void RunVoidScript<T1, T2, T3, T4>(string script, T1 v1, T2 v2, T3 v3, T4 v4) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return;
            Script.Call(fn, v1, v2, v3, v4);
        }

        public TR RunScript<TR>(string script) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn).ToObject();
        }

        public TR RunScript<T1, TR>(string script, T1 v1) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1).ToObject();
        }

        public TR RunScript<T1, T2, TR>(string script, T1 v1, T2 v2) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2).ToObject();
        }

        public TR RunScript<T1, T2, T3, TR>(string script, T1 v1, T2 v2, T3 v3) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3).ToObject();
        }

        public TR RunScript<T1, T2, T3, T4, TR>(string script, T1 v1, T2 v2, T3 v3, T4 v4) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4).ToObject();
        }
    }
}