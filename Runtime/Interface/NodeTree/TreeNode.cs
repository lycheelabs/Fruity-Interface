namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// A node in an input hierarchy. Implemented by InterfaceNode and by plain test nodes.
    /// </summary>
    public interface TreeNode {

        TreeNode Parent { get; }

        bool IsInputDisabled { get; }

        int Layer { get; }
    }

}
