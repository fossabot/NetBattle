using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using NetBattle.Structure;

namespace NetBattle.Field.Entity {
    public abstract class ScriptFieldEntity : FieldEntity {
        private static readonly Type[] DefaultTypes = {
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

        public const string ScriptKeyEntity = "entity";
        public const string ScriptKeyManager = "manager";
        public readonly Script Script;

        public ScriptFieldEntity(FieldEntity parent = null) : base(parent) {
            Script = new Script(CoreModules.Preset_HardSandbox);
        }

        public ScriptFieldEntity(Owner owner, FieldEntity parent = null) : base(owner, parent) {
            Script = new Script(CoreModules.Preset_HardSandbox);
        }

        public void CopySources(ScriptFieldEntity other) {
            for (var i = 0; i < other.Script.SourceCodeCount; i++)
                Script.DoString(other.Script.GetSourceCode(i).Code);
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