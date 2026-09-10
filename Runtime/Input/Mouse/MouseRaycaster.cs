using System.Collections.Generic;
using LycheeLabs.FruityInterface;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MouseRaycaster  {
    
    private static readonly float MAX_DISTANCE = float.MaxValue;
    public static readonly int MAX_HITS = 100;
    private static LayerMask everything = -1;

    private RaycastHit[] RaycastBuffer = new RaycastHit[MAX_HITS];
    private RaycastHit2D[] RaycastBuffer2D = new RaycastHit2D[MAX_HITS];
    private readonly List<RaycastResult> graphicResults = new List<RaycastResult>();
    private GraphicRaycaster[] graphicRaycasters;
    private int graphicRaycasterRefreshFrame = -1;

    public void CollideAndResolve (MouseButton button, out MouseTarget target, out InterfaceNode targetNode, out Vector3 targetPoint) {
        target = null;
        targetNode = null;
        targetPoint = Vector3.zero; 

        if (!FruityUI.MouseIsOnscreen) {
            return;
        }

        if (TryGraphicRaycast(button, out target, out targetNode, out targetPoint)) {
            return;
        }

        // Cast a ray, collecting all hits (2d and 3d)
        // SyncTransforms ensures 2D colliders reflect their current transform positions,
        // since Physics2D only syncs automatically during FixedUpdate.
        Physics2D.SyncTransforms();
        Ray ray = InterfaceHelpers.ScreenPointToRay(FruityUI.UICamera, FruityUI.RawMouseScreenPosition);
        int hitCount = Physics.RaycastNonAlloc(ray, RaycastBuffer, MAX_DISTANCE);
        int hitCount2D = Physics2D.GetRayIntersectionNonAlloc(ray, RaycastBuffer2D, MAX_DISTANCE, everything);

        // Extract closest enabled MouseTarget (2d and 3d)
        // Use manually computed sqr distance from ray origin for both systems.
        float bestSqrDistance = float.MaxValue;
        for (int i = 0; i < hitCount; i++) {
            var hit = RaycastBuffer[i];

            float sqrDist = ((Vector3)hit.point - ray.origin).sqrMagnitude;
            if (sqrDist < bestSqrDistance) {
                var node = ResolveNode(hit.collider);
                if (node != null && node.InputEnabledInHierarchy) {
                    var candidate = node.GetMouseTarget(hit.point, button);
                    if (candidate != null) {
                        target = candidate;
                        targetNode = node;
                        targetPoint = hit.point;
                        bestSqrDistance = sqrDist;
                    }
                }
            }
        }
        for (int i = 0; i < hitCount2D; i++) {
            var hit = RaycastBuffer2D[i];

            // The 2D hit only confirms a collision occurred - its point and distance
            // are unreliable in a 3D context. Discard them. Instead, construct a
            // plane at the collider's Z depth with normal (0,0,1) and project the
            // original ray onto it to get a consistent, comparable 3D hit point.
            Plane plane = new Plane(Vector3.forward, hit.collider.transform.position);
            if (!plane.Raycast(ray, out float enter)) continue;
            Vector3 hitPoint3D = ray.GetPoint(enter);

            float sqrDist = (hitPoint3D - ray.origin).sqrMagnitude;
            if (sqrDist < bestSqrDistance) {
                var node = ResolveNode(hit.collider);
                if (node != null && node.InputEnabledInHierarchy) {
                    var candidate = node.GetMouseTarget(hit.point, button);
                    if (candidate != null) {
                        target = candidate;
                        targetNode = node;
                        targetPoint = hitPoint3D;
                        bestSqrDistance = sqrDist;
                    }
                }
            }
        }
    }

    private static InterfaceNode ResolveNode (Collider col) {
        return col.GetComponent<InputForwarder>()?.Target
            ?? col.GetComponent<InterfaceNode>();
    }

    private static InterfaceNode ResolveNode (Collider2D col) {
        return col.GetComponent<InputForwarder>()?.Target
            ?? col.GetComponent<InterfaceNode>();
    }

    private bool TryGraphicRaycast(
        MouseButton button,
        out MouseTarget target,
        out InterfaceNode targetNode,
        out Vector3 targetPoint) {
        target = null;
        targetNode = null;
        targetPoint = Vector3.zero;

        var eventSystem = EventSystem.current;
        if (eventSystem == null) return false;

        var pointerData = new PointerEventData(eventSystem) {
            position = FruityUI.RawMouseScreenPosition,
            button = ToPointerEventButton(button)
        };

        if (graphicRaycasters == null || Time.frameCount - graphicRaycasterRefreshFrame >= 30) {
            graphicRaycasters = Object.FindObjectsByType<GraphicRaycaster>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);
            graphicRaycasterRefreshFrame = Time.frameCount;
        }

        for (var i = 0; i < graphicRaycasters.Length; i++) {
            var graphicRaycaster = graphicRaycasters[i];
            if (graphicRaycaster == null || !graphicRaycaster.isActiveAndEnabled) continue;

            graphicResults.Clear();
            graphicRaycaster.Raycast(pointerData, graphicResults);
            for (var resultIndex = 0; resultIndex < graphicResults.Count; resultIndex++) {
                var result = graphicResults[resultIndex];
                var node = ResolveNode(result.gameObject);
                if (node == null || !node.InputEnabledInHierarchy) continue;

                var candidate = node.GetMouseTarget(result.worldPosition, button);
                if (candidate == null) continue;

                target = candidate;
                targetNode = node;
                targetPoint = result.worldPosition;
                return true;
            }
        }

        return false;
    }

    private static InterfaceNode ResolveNode(GameObject gameObject) {
        return gameObject.GetComponentInParent<InputForwarder>()?.Target
            ?? gameObject.GetComponentInParent<InterfaceNode>();
    }

    private static PointerEventData.InputButton ToPointerEventButton(MouseButton button) {
        switch (button) {
            case MouseButton.Right:
                return PointerEventData.InputButton.Right;
            case MouseButton.Middle:
                return PointerEventData.InputButton.Middle;
            default:
                return PointerEventData.InputButton.Left;
        }
    }

}
