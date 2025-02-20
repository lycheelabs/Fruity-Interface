using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public static class FruityUI {

        public static Vector2 MouseScreenPosition => ((Vector2)Input.mousePosition * InterfaceConfig.UIScaling) - InterfaceConfig.LetterboxOffset;
        public static Vector3 MouseWorldPosition => InterfaceConfig.MouseToWorldPoint();
        
        public static bool MouseIsOnscreen =>
            Input.mousePosition.x >= 1  && 
            Input.mousePosition.y >= 1 && 
            Input.mousePosition.x < (Screen.width - 1) && 
            Input.mousePosition.y < (Screen.height - 1);

        public static MouseTarget HighlightedTarget { get; internal set; }
        public static ClickTarget SelectedTarget { get; internal set; }
        public static DragTarget DraggedTarget { get; internal set; }
        public static MouseTarget DraggedOverTarget { get; internal set; }
        
        // -----------------------------------------------------------
        
        public static void TriggerNewClick (ClickTarget target, MouseButton button) {
            FruityUIManager.TriggerNewClick(target, button);
        }
        
    }
    
}