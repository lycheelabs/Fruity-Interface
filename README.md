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
| `SettingsRegistry` | Static `Active` instance. `Register<T>()`, `TryGet<T>(key)`, `ApplyAll()`, `ResetAll()`. |
| `SettingsFile` | Subclass of `SaveFile`. `CaptureFrom(registry)`, `ApplyTo(registry)`, `Save()`, `Load()`. |

### Value setter behavior

Setting `Value` calls `Apply()` immediately. The base `Apply()` is a no-op — override it for settings that need side effects (e.g. `Screen.fullScreen = ...`).

During load, `SettingsFile.ApplyTo()` writes all values silently via `FromToken()` (bypasses the property setter), then calls `Apply()` on every parameter once all values are consistent.

### JSON shape

```json
{"Data":{"display":{"fullscreen":true,"resolution":2},"audio":{"master_vol":80}}}
```

Dotted keys (`"display.fullscreen"`) become nested objects. Key is the single source of truth for category structure.

### Usage in a game project

```csharp
public static class GameSettings {
    static GameSettings() {
        var r = new SettingsRegistry();

        Fullscreen = r.Register(new FullscreenSetting("display.fullscreen", true));
        MasterVol  = r.Register(new IntSetting("audio.master_vol", 80, 0, 100));

        Registry = r;
        SettingsRegistry.Active = r;
    }

    public static SettingsRegistry Registry { get; private set; }

    public static FullscreenSetting Fullscreen { get; private set; }
    public static IntSetting MasterVol { get; private set; }

    public static void Save() {
        var f = new SettingsFile(); f.CaptureFrom(Registry); f.Save();
    }
    public static void Load() {
        var r = SettingsFile.Load();
        if (r.IsValid) r.Data.ApplyTo(Registry);
    }
}
```

Boot: call `GameSettings.Load()` before any setting is read (e.g. `[RuntimeInitializeOnLoadMethod]` or in a bootstrap manager).

Complex settings subclass the base type and override `Apply()`:

```csharp
public class FullscreenSetting : BoolSetting {
    public FullscreenSetting(string key, bool def) : base(key, def) { }
    public override void Apply() => Screen.fullScreen = Value;
}
```

---

## Settings UI Bridge (`Elements/Buttons/Settings/`)

Connects UI controls to the settings pipeline. Two-component pattern per control type.

### Toggle bridge

| Component | Goes on | Role |
|---|---|---|
| `ToggleSettingKey` | SettingNode (the field) | Holds `SettingKey` string. `Start()` finds child `ToggleEffect` and calls `SetUpAs()`. `OnToggled()` writes to the `BoolSetting`. |
| `ToggleSettingEffect` | ToggleButton (the button) | Extends `ToggleEffect`. `ApplyToggle()` routes to `GetComponentInParent<ToggleSettingKey>().OnToggled()`. |

### Prefab setup

Bake `ToggleSettingEffect` onto the `ToggleButton` child in a prefab variant. The user drops the variant, adds `ToggleSettingKey` to the root, sets `SettingKey` in the inspector — one field to configure.

### Extending

Future control types follow the same pattern:

| Data | Field Key Component | Button Effect Component |
|---|---|---|
| `BoolSetting` | `ToggleSettingKey` | `ToggleSettingEffect` |
| `IntSetting` | `SliderSettingKey` | `SliderSettingEffect` |
| `StringSetting` | `DropdownSettingKey` | `DropdownSettingEffect` |

---

## UI Elements (`Elements/`)

### Layout & hierarchy

| Class | Extends | Role |
|---|---|---|
| `InterfaceNode` | MonoBehaviour | Base — parent hierarchy, layer, input enabled. |
| `LayoutNode` | InterfaceNode | RectTransform + ILayoutElement + `RefreshLayout()`. |
| `ColliderNode` | InterfaceNode | BoxCollider-based raycast target. |
| `ControlNode` | LayoutNode | Adds `LayoutDriver` for parent-driven sizing. |
| `ContainerNode` | LayoutNode | Abstract — `ChildNodes` list, `RebuildChildNodes()`. |
| `SettingNode` | ControlNode | Field layout — splits into text label + control child. |

### Button types

| Class | Extends | Role |
|---|---|---|
| `ButtonEffect` | MonoBehaviour | Abstract click behavior — `MouseOver()`, `Activate(button)`, `TryUnclick()`. |
| `ButtonNode` | ControlNode + ClickTarget | Hover animation, delegates click to `ButtonEffect`. |
| `TextButton` | ButtonNode | Button with TMP text + optional icon. |
| `IconButton` | ButtonNode | Button with sprite icon. |
| `ToggleButton` | LayoutNode + ClickTarget | Animated boolean toggle switch. Self-animates, delegates `ToggleTo()` to its `ToggleEffect`. |

### Toggle types

| Class | Extends | Role |
|---|---|---|
| `ToggleEffect` | MonoBehaviour | Abstract — `IsToggledOn`, `Toggle()`, `ToggleTo()`, `ApplyToggle(bool)`. |
| `ToggleField` | ControlNode | Compound — IconButton toggle + text label. (Click wiring commented out; use ToggleButton pattern instead.) |

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
| `MouseState` | MonoBehaviour singleton — raycasting, press lifecycle, event queuing. |
| `MouseTarget` | Interface — `UpdateMouseHover`, `EndMouseHover`. |
| `ClickTarget` | Extends MouseTarget — `ApplyMouseClick`, `TryMouseUnclick`. |
| `DragTarget` | Extends MouseTarget — drag modes, multi-place. |
| `DraggedOverTarget` | Extends MouseTarget — `UpdateMouseDraggedOver`. |
| `MouseRaycaster` | Raycasts against colliders on each layer. |
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
| `Anchor` / `ScreenAnchor` / `WorldAnchor` | RectTransform anchoring helpers. |
| `InterfaceHelpers` | Coordinate conversion between screen, UI, and world space. |
| `InterfaceLayer` | Layer index resolution for input filtering. |
