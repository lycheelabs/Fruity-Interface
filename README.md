# FruityInterface

Unity package (`com.lycheelabs.fruityinterface`) providing UI rendering, input handling, stage sequencing, save/load, and a generic settings pipeline. Depends on `TextMeshPro`.

---

## Save & Load (`SaveLoad/`)

Namespace `LycheeLabs.FruityInterface.SaveLoad`. JSON-based with AES encryption (optional) and GZip compression. Uses Newtonsoft.Json (embedded DLL in `Runtime/Plugins/`).

### Public API

| Class | Role |
|---|---|
| `SaveFile` | `[Serializable]` abstract base — subclass with your data fields and `Validate()`. |
| `SaveManager` | Static entry — `TrySave<T>()`, `TryLoad<T>()`, `Exists()`, `DeleteFile()`. |
| `SaveFilePath<T>` | Typed path wrapper — `Exists()`, `Load()`, `LoadIfExists()`, `Delete()`. |
| `LoadData<T>` | Result struct — `IsValid`, `Data`. |
| `AtomicSaveManager` | `internal` — handles serialization, atomic writes, backup recovery, encryption. |

### Usage

```csharp
public class MySaveFile : SaveFile {
    public int Version;
    public string PlayerName;

    public override bool Validate() => !string.IsNullOrEmpty(PlayerName);
    public bool Save() => SaveManager.TrySave(this, "myfile.sav", encrypt: false);
}
```

### Save flow

`SaveManager.TrySave()` → `AtomicSaveManager.TrySaveData()` → `JsonConvert.SerializeObject()` → optional compress + encrypt → atomic write with backup.

### Load flow

`SaveManager.TryLoad()` → `AtomicSaveManager.TryLoadData()` → optional decrypt + decompress → `JsonConvert.DeserializeObject<T>()` → `Validate()`.

---

## Settings (`Settings/`)

Namespace `LycheeLabs.FruityInterface.Settings`. Generic typed settings with JSON persistence, category grouping via dotted keys, and an overrideable `Apply()` hook per setting.

### Core types

| Class | Role |
|---|---|
| `Setting` | Abstract base — `Key`, `virtual Apply()`. Serialized via JToken internally. |
| `BoolSetting` | Concrete — `bool Value`, `DefaultValue`, `Reset()`. |
| `IntSetting` | Concrete — `int Value`, `Min`, `Max`, `DefaultValue`, `Reset()`. Auto-clamps. |
| `StringSetting` | Concrete — `string Value`, `DefaultValue`, `Reset()`. |
| `VolumeSetting` | `: IntSetting` — takes `AudioMixer` + exposed parameter. `Apply()` sets mixer dB. |
| `VSyncSetting` | `: BoolSetting` — `Apply()` sets `QualitySettings.vSyncCount`. |
| `SettingsRegistry` | Static `Active` instance. `Register<T>()`, `TryGet<T>(key)`, `ApplyAll()`, `ResetAll()`. |
| `SettingsFile` | Subclass of `SaveFile`. `CaptureFrom(registry)`, `ApplyTo(registry)`, `Save()`, `Load()`. |

### Value setter behavior

Setting `Value` calls `Apply()` immediately. The base `Apply()` is a no-op — override it for settings that need side effects.

During load, `SettingsFile.ApplyTo()` writes all values silently via `FromToken()` (bypasses the property setter), then calls `Apply()` on every parameter once all values are consistent.

### JSON shape

```json
{"Data":{"display":{"fullscreen":true,"vsync":true,"render_scale":100},"audio":{"master_vol":80,"music_vol":65,"sfx_vol":100}}}
```

Dotted keys (`"display.fullscreen"`) become nested objects. Key is the single source of truth for category structure.

### Usage in a game project

```csharp
public static class GameSettings {
    public static void Initialise(AudioMixer mixer, UniversalRenderPipelineAsset urp) {
        var r = new SettingsRegistry();

        Fullscreen  = r.Register(new FullscreenSetting("display.fullscreen", true));
        RenderScale = r.Register(new RenderScaleSetting("display.render_scale", urp));
        Vsync       = r.Register(new VSyncSetting("display.vsync", true));
        MasterVol   = r.Register(new VolumeSetting("audio.master_vol", mixer, "MasterVolume"));
        MusicVol    = r.Register(new VolumeSetting("audio.music_vol",  mixer, "MusicVolume"));
        SfxVol      = r.Register(new VolumeSetting("audio.sfx_vol",    mixer, "SFXVolume"));

        Registry = r;
        SettingsRegistry.Active = r;
        Load();
    }

    public static void Save() {
        var f = new SettingsFile(); f.CaptureFrom(Registry); f.Save();
    }
    public static void Load() {
        var r = SettingsFile.Load();
        if (r.IsValid) r.Data.ApplyTo(Registry);
    }
}
```

Complex settings subclass the base type and override `Apply()` — see `FullscreenSetting`, `VSyncSetting`, `VolumeSetting`, `RenderScaleSetting`.

---

## Settings UI Bridge (`Elements/Buttons/Settings/`)

Connects UI controls to the settings pipeline. Two-component pattern per control type.

### Toggle bridge

| Component | Goes on | Role |
|---|---|---|
| `ToggleSettingKey` | SettingNode (the field) | Holds `SettingKey` string. `Start()` reads the `BoolSetting` and calls `SetUpAs()`. `OnToggled()` writes back. |
| `ToggleSettingEffect` | ToggleSwitch (the child) | Extends `ToggleEffect`. `ApplyToggle()` routes to `GetComponentInParent<ToggleSettingKey>().OnToggled()`. |

### Slider bridge

| Component | Goes on | Role |
|---|---|---|
| `SliderSettingKey` | SettingNode (the field) | Holds `SettingKey` string. `Start()` calls `slider.Configure(min, max, value)`. `OnSlid(int)` writes to `IntSetting`. |
| `SliderSettingEffect` | SliderNode (the child) | Extends `SliderEffect`. `OnValueChanged(int)` routes to `GetComponentInParent<SliderSettingKey>().OnSlid()`. |

### Extending

Future control types follow the same pattern:

| Data | Field Key Component | Effect Component |
|---|---|---|
| `BoolSetting` | `ToggleSettingKey` | `ToggleSettingEffect` |
| `IntSetting` | `SliderSettingKey` | `SliderSettingEffect` |
| `StringSetting` | `DropdownSettingKey` | `DropdownSettingEffect` |

---

## UI Elements

### Layout & hierarchy

| Class | Extends | Role |
|---|---|---|
| `InterfaceNode` | MonoBehaviour | Base — parent hierarchy, layer, input enabled. |
| `LayoutNode` | InterfaceNode | RectTransform + `LayoutSizePixels` + `RefreshLayout()`. |
| `ColliderNode` | InterfaceNode | BoxCollider-based raycast target. |
| `ControlNode` | LayoutNode | Adds `LayoutDriver` for parent-driven sizing. |
| `ContainerNode` | LayoutNode | Abstract — `ChildNodes` list, `RebuildChildNodes()`. |
| `SettingNode` | ControlNode | Field layout — text label + control child. |

### Button types

| Class | Extends | Role |
|---|---|---|
| `ButtonEffect` | MonoBehaviour | Abstract click behavior — `MouseOver()`, `Activate(button)`, `TryUnclick()`. |
| `ButtonNode` | ControlNode + ClickTarget | Hover animation, delegates click to `ButtonEffect`. Has `TryGetSFX`. |
| `TextButton` | ButtonNode | Button with TMP text + optional icon. |
| `IconButton` | ButtonNode | Button with sprite icon. |
| `ToggleSwitch` | LayoutNode + ClickTarget | Animated boolean switch. Has `TryGetSFX` — calls `OnTurnOn()` / `OnTurnOff()`. |

### Toggle types

| Class | Extends | Role |
|---|---|---|
| `ToggleEffect` | MonoBehaviour | Abstract — `IsToggledOn`, `Toggle()`, `ToggleTo()`, `ApplyToggle(bool)`. |

### Slider types

| Class | Extends | Role |
|---|---|---|
| `SliderNode` | LayoutNode + DragTarget | Draggable int slider. Configurable `width`, `showValue`, `valueIsPercentage`. Has `TryGetSFX` — calls `OnClick()` on drag start, `OnSliderModified()` on value change (throttled). |
| `SliderEffect` | MonoBehaviour | Abstract — `OnValueChanged(int value)`. |

### SFX

| Class | Extends | Role |
|---|---|---|
| `NodeSFX` | MonoBehaviour | Abstract — `OnFirstHover()`, `OnClick()`, `OnTurnOn()`, `OnTurnOff()`, `OnSliderModified(float)`. |

Attach a concrete `NodeSFX` subclass to any `ButtonNode`, `ToggleSwitch`, or `SliderNode`. The node calls the appropriate method automatically via `TryGetSFX`.

### Selector types

| Class | Extends | Role |
|---|---|---|
| `TabbingSelectorEffect` | MonoBehaviour | Abstract multi-option — `ListAllOptions()`, `TabLeft/Right`, `SelectedOption`. |
| `TabbingSelector` | — | UI node for tabbing selector. |
| `TabbingSelectorButtonEffect` | ButtonEffect | Routes click to parent `TabbingSelector`. |

### Containers

| Class | Extends | Role |
|---|---|---|
| `ContainerNode` | LayoutNode | Abstract — holds and rebuilds child nodes. |
| `SimpleContainerNode` | ContainerNode | Vertical list. |
| `GridContainerNode` | ContainerNode | Grid with horizontal/vertical wrap. |
| `ListContainerNode` | ContainerNode | Scrollable vertical list. |

### Prompts

| Class | Extends | Role |
|---|---|---|
| `PromptNode` | InterfaceNode | Abstract — modal lifecycle: `Open()`, `Close()`, `Pause()`, `ProceedTo()`, `GoBack()`. |

### Canvas

| Class | Role |
|---|---|
| `CanvasNode` | Root Canvas wrapper. |
| `FullscreenButtonNode` | Fullscreen toggle button. |
| `FullscreenLetterboxNode` | Letterbox bars for aspect ratio. |
| `FullscreenShadowNode` | Dimming shadow under modal prompts. |

---

## Input (`Library/Controls/MouseControls/`)

| Class | Role |
|---|---|
| `MouseState` | Raycasting, press lifecycle, event queuing. Tracks plain `MouseTarget` presses (not just click/drag). |
| `MouseTarget` | Interface — `UpdateMouseHover`, `EndMouseHover`. |
| `ClickTarget` | Extends MouseTarget — `ApplyMouseClick`, `TryMouseUnclick`. |
| `DragTarget` | Extends MouseTarget — drag modes (`DragOnly`, `PickUpOnly`, `DragOrPickUp`). |
| `DraggedOverTarget` | Extends MouseTarget — `UpdateMouseDraggedOver`. |
| `MouseRaycaster` | Supports `InputForwarder` for decoupling collider from `InterfaceNode`. |
| `InputForwarder` | MonoBehaviour — `Target` field. Lives on collider GameObject, routes input to a different `InterfaceNode`. |
| `MouseButton` | Enum `Left`, `Right`, `Middle`, `None`. |

Events go through queues: `HoverHierarchyEvent` → `ClickEvent` → `StartDragEvent` → ... → `EndDragEvent`.

---

## Game Flow (`Library/GameFlow/`)

### Sequencing

| Class | Role |
|---|---|
| `EventSequencer` | Abstract — layered event pipeline. |
| `TwoTierMenuSequencer` | GAMEPLAY + OVERLAY prompt layers. |
| `PromptSequenceLayer` | Navigation stack — GoBack, ProceedTo, ActivePrompt. |
| `TransitionSequenceLayer` | Screen/stage transitions. |

### Staging & transitions

| Class | Role |
|---|---|
| `GameStage` | Base for game states (title, level, expedition). |
| `StageTransition` | State machine — `StageTransitionChart`. |
| `ScreenTransition` | Animated screen wipe/iris. |
| `TransitionEvent` | Queued transition command. |

### Blocking events

`BlockingEvent`, `OneShotBlockingEvent`, `TimedBlockingEvent` — game logic that holds the sequencer until resolved.

---

## Animation (`Library/Animation/`)

| Class | Role |
|---|---|
| `JuicyAnimator` | Per-GameObject animation state — scale, position, rotation tweens. |
| `JuicyGroup` | Synchronised multi-object animator. |
| `JuicyAnimation` / `JuicyAnimations` | Predefined animations: squash, wiggle, bulge, nudge, flatten. |
| `TransformData` | Snapshot of `localPosition`, `localScale`, `localRotation`. |

---

## Utilities

| Class | Role |
|---|---|
| `FruityUI` | Static access — layer locking, camera/world plane, mouse-to-world projection, `TriggerNewClick()`. |
| `FruityUIManager` | MonoBehaviour singleton — mouse input, aspect ratio, click buffering. |
| `FruityUIPrefabs` | Prefab loading — Canvas, Letterbox, Shadow, FullscreenButton. |
| `FruityEditorDrawer` | Editor helpers — `DrawConfigProperties()`, `DrawLayoutProperties()`, `DrawPrefabProperties()`, `DrawAdditionalProperties()`. |
| `Anchor` / `ScreenAnchor` / `WorldAnchor` | RectTransform anchoring helpers. |
| `InterfaceHelpers` | Coordinate conversion between screen, UI, and world space. |
| `InterfaceLayer` | Layer index resolution for input filtering. |
