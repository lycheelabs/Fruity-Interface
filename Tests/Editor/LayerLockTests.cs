using LycheeLabs.FruityInterface;
using FruityInterface.Tests;
using NUnit.Framework;

namespace Hierarchy {

    public class LayerLockTests {

        [SetUp]
        public void SetUp() {
            FruityUI.ResetStateForTests();
        }

        [TearDown]
        public void TearDown() {
            FruityUI.ResetStateForTests();
        }

        [Test]
        public void LockLayer_SetsThresholdAndIsLocked() {
            FruityUI.LockLayer(1);

            Assert.AreEqual(1, FruityUI.ActiveLayerThreshold);
            Assert.IsTrue(FruityUI.InterfaceIsLocked);
        }

        [Test]
        public void LockLayer_HigherLayerRaisesThreshold_LowerDoesNot() {
            FruityUI.LockLayer(1);
            FruityUI.LockLayer(3);
            FruityUI.LockLayer(2);

            Assert.AreEqual(3, FruityUI.ActiveLayerThreshold);
        }

        [Test]
        public void UnlockLayer_RefCounted_StaysLockedUntilLastUnlock() {
            FruityUI.LockLayer(1);
            FruityUI.LockLayer(1);

            FruityUI.UnlockLayer(1);
            Assert.AreEqual(1, FruityUI.ActiveLayerThreshold);
            Assert.IsTrue(FruityUI.InterfaceIsLocked);

            FruityUI.UnlockLayer(1);
            Assert.AreEqual(0, FruityUI.ActiveLayerThreshold);
            Assert.IsFalse(FruityUI.InterfaceIsLocked);
        }

        [Test]
        public void UnlockLayer_HighestLayer_DropsToNextHighest() {
            FruityUI.LockLayer(1);
            FruityUI.LockLayer(3);

            FruityUI.UnlockLayer(3);

            Assert.AreEqual(1, FruityUI.ActiveLayerThreshold);
        }

        [Test]
        public void UnlockLayer_LowerLayer_KeepsHighestThreshold() {
            FruityUI.LockLayer(1);
            FruityUI.LockLayer(3);

            FruityUI.UnlockLayer(1);

            Assert.AreEqual(3, FruityUI.ActiveLayerThreshold);
        }

        [Test]
        public void UnlockLayer_UnlockedLayer_IsNoOp() {
            FruityUI.LockLayer(2);

            FruityUI.UnlockLayer(5);

            Assert.AreEqual(2, FruityUI.ActiveLayerThreshold);
        }
    }

}
