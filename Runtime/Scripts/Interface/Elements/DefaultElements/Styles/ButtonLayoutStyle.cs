using LycheeLabs.FruityInterface.Elements;
using UnityEngine;

public class ButtonLayoutStyle : MonoBehaviour {

    public float Width = 200;
    public float Height = 50;
    public bool CropWidth = false;
    public Vector2 LayoutPadding = new Vector2(10, 10);
    [Range(0f, 2f)] public float FontHeightScaling = 1;

    public void OnValidate () {
        // Find all buttons that reference this driver
        var allButtons = GetComponentsInChildren<TextButton>(true); // or use Resources.FindObjectsOfTypeAll
        foreach (var button in allButtons) {
            if (button.layoutDriver == this) {
                button.OnValidate();
            }
        }
    }

}
