using NetBattle.Field;
using NetBattle.Field.Entity;
using NetBattle.Structure;
using NUnit.Framework;

namespace NetBattle.Test {
    public class Tests {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void Test1() {
            Assert.Pass();
        }

        [Test]
        public void TestSfe() {
            var testInstance = new SfeTest(Owner.Default);
            testInstance.RegisterScript(@"
function add(a,b)
    return a + b
end
function retstr()
    return ""arhan""
end
");
            Assert.AreEqual(3.0, testInstance.RunScript<double, int, int>("add", 1, 2));
            Assert.AreEqual("arhan", testInstance.RunScript<string>("retstr"));
        }

        private class SfeTest : ScriptFieldEntity {
            public SfeTest(Owner owner, FieldEntity parent = null) : base(owner, parent) {
            }

            public override void RegistrationPhase() {
                throw new System.NotImplementedException();
            }

            public override void ControlPhase() {
                throw new System.NotImplementedException();
            }

            public override void UpdatePhase() {
                throw new System.NotImplementedException();
            }

            public override void DeregistrationPhase() {
                throw new System.NotImplementedException();
            }
        }
    }
}