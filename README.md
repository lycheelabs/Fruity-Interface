# InputTree

Unity package (`com.lycheelabs.inputtree`) providing UI rendering, input handling, and stage sequencing. Depends on `com.unity.ugui`, `com.unity.inputsystem`, and `TextMeshPro`.

Default controls, tooltips, fullscreen controls, and the settings UI bridge live in LycheeGameKit (`com.lycheelabs.gamekit`). Only the canvas and letterbox prefabs remain here, and they load through `Resources.Load`.

---

## Settings UI Bridge

The bridge that connects controls to settings (`ToggleSettingKey`, `SliderSettingKey`, and their effects) lives in LycheeGameKit under `Runtime/Elements/Controls/Settings/`. The settings data classes (`Setting`, `SettingsRegistry`, `SettingsFile`, etc.) live in `com.lycheelabs.inputtree.standardnodes` under `Runtime/Settings/`, with save/load via FruitySaveLoad.

---

## UI Elements (`Runtime/Elements/`)

### Layout & hierarchy

| Class | Extends | Role |
|---|---|---|
| `InterfaceNode` | MonoBehaviour | Base — parent hierarchy, layer, input enabled. |
| `LayoutNode` | InterfaceNode | RectTransform + `LayoutSizePixels` + `RefreshLayout()`. |
| `ColliderNode` | InterfaceNode | BoxCollider-based raycast target. |
| `ControlNode` | LayoutNode | Adds `LayoutDriver` for parent-driven sizing. |
| `ContainerNode` | LayoutNode | Abstract — `ChildNodes` list, `RebuildChildNodes()`. |

### Button base

| Class | Extends | Role |
|---|---|---|
| `ButtonEffect` | MonoBehaviour | Abstract click behavior — `MouseOver()`, `Activate(button)`, `TryUnclick()`. |
| `ButtonNode` | ControlNode + ClickTarget | Hover animation, delegates click to `ButtonEffect`. Has `TryGetSFX`. |

Concrete buttons, toggles, and sliders (`TextButton`, `IconButton`, `ToggleSwitch`, `SliderNode`, and their effects) live in LycheeGameKit.

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
| `TabbingContainerNode` | ContainerNode | Shows one child at a time by index. Size is the largest child. |
| `ButtonSelectorNode` | ContainerNode | Row of selectable buttons. Clicking one selects it and calls `ButtonSelectorEffect.ApplySelection`. Children need `ButtonSelectorSubEffect` and `SelectableButton` (`TextButton` only for now). |

### Canvas

| Class | Role |
|---|---|
| `CanvasNode` | Root Canvas wrapper. |
| `FullscreenLetterboxNode` | Letterbox bars for aspect ratio. |

### Backgrounds

| Class | Role |
|---|---|
| `BackgroundNode` | Base show/hide lifecycle with tweened progress, instant `jump`, and `MaxEntry`/`TransitionSpeed` properties. Subclasses override `OnInitialise` and `ApplyTween`. Set `WaitForFullTransition` to false to follow the target without waiting for a fade to finish. |

---

## Input (`Runtime/Interface/Input/MouseControls/`)

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

`MouseState` resolves raw input and active press state before deferred callbacks
are delivered. Drag callbacks are emitted as queued start, over, update, and end
events, so target callbacks observe a settled input state for the frame. The
global `FruityUI.DraggedTarget` reference and the active press are cleared during
normal cancellation and when a Unity target is destroyed between frames. A
physical button-down event cannot preempt an active or queued press.

---

## Animation (`Runtime/Animation/`)

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
| `FruityUIPrefabs` | Canvas and letterbox prefab loading through `Resources.Load`. |
| `FruityEditorDrawer` | Editor helpers — `DrawConfigProperties()`, `DrawLayoutProperties()`, `DrawPrefabProperties()`, `DrawAdditionalProperties()`. |
| `Anchor` / `ScreenAnchor` / `WorldAnchor` | RectTransform anchoring helpers. |
| `InterfaceHelpers` | Coordinate conversion between screen, UI, and world space. |
| `InterfaceLayer` | Layer index resolution for input filtering. |
