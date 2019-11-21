using NetBattle.Field;
using NUnit.Framework;

namespace NetBattle.Test {
    public class Tests {
        private const string ScriptText = @"
konno = ""yuuki""
function add(a,b)
    return a + b
end
function retstr()
    return ""arhan""
end
";

        private const string ManagerTestScriptText = @"

function x_set(id)
    xid = id
end
function x_get()
    return mg_find_entity(xid)
end
function x_damage_other_health(damage)
    return x_get().Health.Damage(damage)
end
function x_heal_other_health(heal)
    return x_get().Health.Heal(heal)
end
function x_set_self_health(value)
    entity.Health.Value = value
    return entity.Health.Value
end
";

        [SetUp]
        public void Setup() {
        }

        [Test]
        public void TestSfeCopy() {
            var entity1 = new TestScriptFieldEntity();
            entity1.RegisterScript(ScriptText);
            var entity2 = new TestScriptFieldEntity();
            entity2.CopySources(entity1);
            Assert.AreEqual(3.0, entity2.RunScript<int, int, double>("add", 1, 2));
            Assert.AreEqual("arhan", entity2.RunScript<string>("retstr"));
            Assert.AreEqual("yuuki", entity2.Script.Globals["konno"]);
        }

        [Test]
        public void TestSfeManager() {
            var manager = new FieldManager(10, 10);
            var entity1 = new TestScriptFieldEntity();
            var guid = entity1.Id;
            entity1.Health.OverchargeMaximum = 100;
            entity1.Health.Maximum = 100;
            entity1.Health.Value = 100;
            manager.QueueRegistration(entity1);
            var entity2 = new TestScriptFieldEntity();
            entity2.RegisterScript(ManagerTestScriptText);
            manager.QueueRegistration(entity2);
            manager.UpdateField(1.0f);
            entity2.RunVoidScript("x_set", guid);
            var res = entity2.RunScript<FieldEntity>("x_get");
            Assert.AreEqual(entity1, res);
            entity2.RunScript<int, double>("x_damage_other_health", 10);
            Assert.AreEqual(90, entity1.Health.Value);
            entity2.RunScript<int, double>("x_heal_other_health", 1000);
            Assert.AreEqual(100, entity1.Health.Value);
            entity2.RunScript<int, double>("x_set_self_health", 50);
            Assert.AreEqual(50, entity2.Health.Value);
        }

        private class TestScriptFieldEntity : ScriptFieldEntity {
            public TestScriptFieldEntity(FieldEntity parent = null) : base(Structure.Owner.None, parent) {
                Health = new Health();
            }

            public override void RegistrationPhase() {
            }

            public override void ControlPhase() {
            }

            public override void UpdatePhase() {
            }

            public override void DeregistrationPhase() {
            }
        }
    }
}