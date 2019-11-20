using NetBattle.Field;
using NetBattle.Field.Entity;
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

        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Test1() {
            TestSfeCopy();
        }

        [Test]
        public void TestSfe() {
            var testInstance = new SfeTest();
            testInstance.RegisterScript(ScriptText);
            Assert.AreEqual(3.0, testInstance.RunScript<double, int, int>("add", 1, 2));
            Assert.AreEqual("arhan", testInstance.RunScript<string>("retstr"));
            Assert.AreEqual("yuuki", testInstance.Script.Globals["konno"]);
        }

        [Test]
        public void TestSfeCopy() {
            var testInstance = new SfeTest();
            testInstance.RegisterScript(ScriptText);
            var testInstance2 = new SfeTest();
            testInstance2.CopySources(testInstance);
            Assert.AreEqual(3.0, testInstance2.RunScript<double, int, int>("add", 1, 2));
            Assert.AreEqual("arhan", testInstance2.RunScript<string>("retstr"));
            Assert.AreEqual("yuuki", testInstance2.Script.Globals["konno"]);
        }

        private class SfeTest : ScriptFieldEntity {
            public SfeTest(FieldEntity parent = null) : base(parent) {
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