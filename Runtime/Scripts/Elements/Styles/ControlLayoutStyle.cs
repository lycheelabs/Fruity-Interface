using LycheeLabs.FruityInterface.Elements;
using UnityEngine;


public interface ControlLayoutDriver {

    // Dimensions
    float DrivenWidth { get; }
    float DrivenHeight { get; }
    Vector2 DrivenLayoutPadding { get; }

    // Text buttons
    bool DrivenCropWidth { get; }
    float DrivenFontHeightScaling { get; }

    // Icon buttons
    float DrivenIconScale { get; }

    // Compound buttons
    float DrivenInteriorMargins{ get; }

}

public interface DrivenControlNode {
    void RefreshDrivenLayout ();
}

public class ControlLayoutStyle : MonoBehaviour, ControlLayoutDriver {

    public string Nickname = "";
    public MonoBehaviour ParentDriver;

    // Dimensions
    public float width = 200;
    public float height = 50;
    public Vector2 layoutPadding = new Vector2(10, 10);

    // Text buttons
    public bool cropWidth = false;
    [Range(0f, 2f)] public float fontHeightScaling = 1;

    // Icon buttons
    [Range(0f, 2f)] public float iconScaling = 1;

    // Compound buttons
    public float interiorMargins = 10;

    // Driven values
    public float DrivenWidth => width;
    public float DrivenHeight => height;
    public Vector2 DrivenLayoutPadding => layoutPadding;
    public bool DrivenCropWidth => cropWidth;
    public float DrivenFontHeightScaling => fontHeightScaling;
    public float DrivenIconScale => iconScaling;
    public float DrivenInteriorMargins => interiorMargins;

    public void OnValidate () {

        if (ParentDriver != null) {
            var driver = ParentDriver as ControlLayoutDriver;
            if (driver == null) {
                ParentDriver = null;
                Debug.LogWarning("Assigned ParentDriver does not implement ControlLayoutDriver interface. Unassigning.", this);
            } else {
                width = driver.DrivenWidth;
                height = driver.DrivenHeight;
                layoutPadding = driver.DrivenLayoutPadding;
                cropWidth = driver.DrivenCropWidth;
                fontHeightScaling = driver.DrivenFontHeightScaling;
                iconScaling = driver.DrivenIconScale;
                interiorMargins = driver.DrivenInteriorMargins;
            }
        }

        // Find all buttons that reference this driver
        var allButtons = GetComponentsInChildren<ControlNode>(true); // or use Resources.FindObjectsOfTypeAll
        foreach (var button in allButtons) {
            button.OnValidate();
        }

    }

}
