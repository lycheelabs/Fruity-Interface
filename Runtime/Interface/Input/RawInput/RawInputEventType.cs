namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Describes an input transition before it is resolved into a Fruity UI target.
    /// </summary>
    internal enum RawInputEventType {
        Move,
        ButtonDown,
        ButtonUp,
        Scroll,
        Cancel
    }

}
