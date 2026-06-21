using UnityEngine;

namespace LycheeLabs.FruityInterface.Settings {

    public class FullscreenSetting : BoolSetting {

        private Vector2Int? _cachedWindowedResolution;

        public FullscreenSetting (string key, bool defaultValue) : base(key, defaultValue) { }

        public override void Apply () {
            if (Value) {
                EnterFullscreen();
            } else {
                ExitFullscreen();
            }
        }

        private void EnterFullscreen () {
            if (Screen.fullScreenMode == FullScreenMode.Windowed) {
                _cachedWindowedResolution = new Vector2Int(Screen.width, Screen.height);
            }
            Screen.SetResolution(
                Display.main.systemWidth,
                Display.main.systemHeight,
                FullScreenMode.FullScreenWindow
            );
        }

        private void ExitFullscreen () {
            if (_cachedWindowedResolution.HasValue) {
                int minW = Mathf.CeilToInt(Display.main.systemWidth * 0.25f);
                int maxW = Mathf.FloorToInt(Display.main.systemWidth * 0.95f);
                int minH = Mathf.CeilToInt(Display.main.systemHeight * 0.25f);
                int maxH = Mathf.FloorToInt(Display.main.systemHeight * 0.95f);
                int w = Mathf.Clamp(_cachedWindowedResolution.Value.x, minW, maxW);
                int h = Mathf.Clamp(_cachedWindowedResolution.Value.y, minH, maxH);
                Screen.SetResolution(w, h, FullScreenMode.Windowed);
            } else {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }

    }

}
