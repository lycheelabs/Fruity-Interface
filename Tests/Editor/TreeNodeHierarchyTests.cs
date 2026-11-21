using NUnit.Framework;

namespace LycheeLabs.FruityInterface.Tests {

    public class TreeNodeHierarchyTests {

        [SetUp]
        public void SetUp() {
            FruityUI.ResetStateForTests();
        }

        [TearDown]
        public void TearDown() {
            FruityUI.ResetStateForTests();
        }

        private static TestNode Child(TreeNode parent, bool disabled = false, int layer = 0) {
            return new TestNode { Parent = parent, IsInputDisabled = disabled, Layer = layer };
        }

        [Test]
        public void HasAncestor_WalksAllAncestors() {
            var root = new TestNode();
            var mid = Child(root);
            var leaf = Child(mid);

            Assert.IsTrue(TreeNodeHierarchy.HasAncestor(leaf, mid));
            Assert.IsTrue(TreeNodeHierarchy.HasAncestor(leaf, root));
            Assert.IsFalse(TreeNodeHierarchy.HasAncestor(root, leaf));
            Assert.IsFalse(TreeNodeHierarchy.HasAncestor(leaf, null));
        }

        [Test]
        public void HasAncestor_NodeIsNotItsOwnAncestor() {
            var node = new TestNode();

            Assert.IsFalse(TreeNodeHierarchy.HasAncestor(node, node));
        }

        [Test]
        public void WouldCreateLoop_SelfParent_IsLoop() {
            var node = new TestNode();

            Assert.IsTrue(TreeNodeHierarchy.WouldCreateLoop(node, node));
        }

        [Test]
        public void WouldCreateLoop_ParentIsDescendant_IsLoop() {
            var parent = new TestNode();
            var child = Child(parent);

            Assert.IsTrue(TreeNodeHierarchy.WouldCreateLoop(parent, child));
        }

        [Test]
        public void WouldCreateLoop_UnrelatedOrNull_IsNotLoop() {
            var a = new TestNode();
            var b = new TestNode();

            Assert.IsFalse(TreeNodeHierarchy.WouldCreateLoop(a, b));
            Assert.IsFalse(TreeNodeHierarchy.WouldCreateLoop(a, null));
        }

        [Test]
        public void IsInputEnabled_TrueByDefault() {
            var root = new TestNode();
            var child = Child(root);

            Assert.IsTrue(TreeNodeHierarchy.IsInputEnabled(child));
        }

        [Test]
        public void IsInputEnabled_DisabledNode_IsFalse() {
            var node = Child(null, disabled: true);

            Assert.IsFalse(TreeNodeHierarchy.IsInputEnabled(node));
        }

        [Test]
        public void IsInputEnabled_DisabledAncestor_DisablesDescendant() {
            var root = Child(null, disabled: true);
            var mid = Child(root);
            var leaf = Child(mid);

            Assert.IsFalse(TreeNodeHierarchy.IsInputEnabled(leaf));
        }

        [Test]
        public void IsInputEnabled_DisabledSibling_DoesNotAffectOthers() {
            var root = new TestNode();
            var a = Child(root, disabled: true);
            var b = Child(root);

            Assert.IsFalse(TreeNodeHierarchy.IsInputEnabled(a));
            Assert.IsTrue(TreeNodeHierarchy.IsInputEnabled(b));
        }

        [Test]
        public void IsInputEnabled_LayerBelowLockedThreshold_IsFalse() {
            FruityUI.LockLayer(2);

            Assert.IsFalse(TreeNodeHierarchy.IsInputEnabled(Child(null, layer: 0)));
            Assert.IsTrue(TreeNodeHierarchy.IsInputEnabled(Child(null, layer: 2)));
        }

        [Test]
        public void IsInputEnabled_DisableInputWithoutLock_IsFalse() {
            FruityUI.DisableInput = true;

            Assert.IsFalse(TreeNodeHierarchy.IsInputEnabled(Child(null)));
        }

        [Test]
        public void IsInputEnabled_DisableInputWithLock_IgnoredForLockedLayer() {
            FruityUI.LockLayer(1);
            FruityUI.DisableInput = true;

            Assert.IsTrue(TreeNodeHierarchy.IsInputEnabled(Child(null, layer: 1)));
        }
    }

}
