using LycheeLabs.InputTree;
using InputTree.Tests;
using NUnit.Framework;

namespace Hierarchy {

    public class LayerLockTests {

        [SetUp]
        public void SetUp() {
            Interface.ResetStateForTests();
        }

        [TearDown]
        public void TearDown() {
            Interface.ResetStateForTests();
        }

        [Test]
        public void LockLayer_SetsThresholdAndIsLocked() {
            Interface.LockLayer(1);

            Assert.AreEqual(1, Interface.ActiveLayerThreshold);
            Assert.IsTrue(Interface.InterfaceIsLocked);
        }

        [Test]
        public void LockLayer_HigherLayerRaisesThreshold_LowerDoesNot() {
            Interface.LockLayer(1);
            Interface.LockLayer(3);
            Interface.LockLayer(2);

            Assert.AreEqual(3, Interface.ActiveLayerThreshold);
        }

        [Test]
        public void UnlockLayer_RefCounted_StaysLockedUntilLastUnlock() {
            Interface.LockLayer(1);
            Interface.LockLayer(1);

            Interface.UnlockLayer(1);
            Assert.AreEqual(1, Interface.ActiveLayerThreshold);
            Assert.IsTrue(Interface.InterfaceIsLocked);

            Interface.UnlockLayer(1);
            Assert.AreEqual(0, Interface.ActiveLayerThreshold);
            Assert.IsFalse(Interface.InterfaceIsLocked);
        }

        [Test]
        public void UnlockLayer_HighestLayer_DropsToNextHighest() {
            Interface.LockLayer(1);
            Interface.LockLayer(3);

            Interface.UnlockLayer(3);

            Assert.AreEqual(1, Interface.ActiveLayerThreshold);
        }

        [Test]
        public void UnlockLayer_LowerLayer_KeepsHighestThreshold() {
            Interface.LockLayer(1);
            Interface.LockLayer(3);

            Interface.UnlockLayer(1);

            Assert.AreEqual(3, Interface.ActiveLayerThreshold);
        }

        [Test]
        public void UnlockLayer_UnlockedLayer_IsNoOp() {
            Interface.LockLayer(2);

            Interface.UnlockLayer(5);

            Assert.AreEqual(2, Interface.ActiveLayerThreshold);
        }
    }

}
