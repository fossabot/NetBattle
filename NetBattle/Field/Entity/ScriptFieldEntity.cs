using System;
using MoonSharp.Interpreter;
using NetBattle.Structure;

namespace NetBattle.Field.Entity {
    public abstract class ScriptFieldEntity : FieldEntity {
        static ScriptFieldEntity() {
            UserData.RegisterType(typeof(ScriptFieldEntity));
        }

        public const string ScriptKeyControlPhase = "ControlPhase";
        public const string ScriptKeyUpdatePhase = "UpdatePhase";
        public const string ScriptKeyEntity = "entity";
        public const string ScriptKeyManager = "manager";
        public readonly Script Script;

        public ScriptFieldEntity(Owner owner, FieldEntity parent = null) : base(owner, parent) {
            Script = new Script(CoreModules.Preset_HardSandbox);
        }

        public void RegisterScript(string script) =>
            Script.DoString(script);

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

        public void RegisterFunction<TR, T1>(string name, Func<TR, T1> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<TR, T1, T2>(string name, Func<TR, T1, T2> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<TR, T1, T2, T3>(string name, Func<TR, T1, T2, T3> function) =>
            Script.Globals[name] = function;

        public void RegisterFunction<TR, T1, T2, T3, T4>(string name, Func<TR, T1, T2, T3, T4> function) =>
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

        public TR RunScript<TR, T1>(string script, T1 v1) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1).ToObject();
        }

        public TR RunScript<TR, T1, T2>(string script, T1 v1, T2 v2) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2).ToObject();
        }

        public TR RunScript<TR, T1, T2, T3>(string script, T1 v1, T2 v2, T3 v3) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3).ToObject();
        }

        public TR RunScript<TR, T1, T2, T3, T4>(string script, T1 v1, T2 v2, T3 v3, T4 v4) {
            Script.Globals[ScriptKeyEntity] = this;
            Script.Globals[ScriptKeyManager] = Manager;
            var fn = Script.Globals.Get(script);
            if (Equals(fn, DynValue.Nil)) return default;
            return (TR) Script.Call(fn, v1, v2, v3, v4).ToObject();
        }
    }
}