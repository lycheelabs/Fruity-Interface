using LycheeLabs.InputTree;

namespace InputTree.Tests {

    public sealed class TestNode : TreeNode {

        public TreeNode Parent { get; set; }

        public bool IsInputDisabled { get; set; }

        public int Layer { get; set; }
    }

}
