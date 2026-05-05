using System.Collections.Generic;
using LycheeLabs.FruityInterface;
using UnityEngine;


public class MouseRaycaster  {
    
    private static readonly float MAX_DISTANCE = float.MaxValue;
    public static readonly int MAX_HITS = 100;
    private static LayerMask everything = -1;

    private RaycastHit[] RaycastBuffer = new RaycastHit[MAX_HITS];
    private RaycastHit2D[] RaycastBuffer2D = new RaycastHit2D[MAX_HITS];

    public void CollideAndResolve (MouseButton button, out MouseTarget target, out InterfaceNode targetNode, out Vector3 targetPoint) {
        target = null;
        targetNode = null;
        targetPoint = Vector3.zero; 

        if (!FruityUI.MouseIsOnscreen) {
            return;
        }

        // Cast a ray, collecting all hits (2d and 3d)
        // SyncTransforms ensures 2D colliders reflect their current transform positions,
        // since Physics2D only syncs automatically during FixedUpdate.
        Physics2D.SyncTransforms();
        Ray ray = InterfaceHelpers.ScreenPointToRay(FruityUI.UICamera, Input.mousePosition);
        int hitCount = Physics.RaycastNonAlloc(ray, RaycastBuffer, MAX_DISTANCE);
        int hitCount2D = Physics2D.GetRayIntersectionNonAlloc(ray, RaycastBuffer2D, MAX_DISTANCE, everything);

        // Extract closest enabled MouseTarget (2d and 3d)
        // Use manually computed sqr distance from ray origin for both systems.
        float bestSqrDistance = float.MaxValue;
        for (int i = 0; i < hitCount; i++) {
            var hit = RaycastBuffer[i];

            float sqrDist = ((Vector3)hit.point - ray.origin).sqrMagnitude;
            if (sqrDist < bestSqrDistance) {
                var node = hit.collider.gameObject.GetComponent<InterfaceNode>();
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
                var node = hit.collider.gameObject.GetComponent<InterfaceNode>();
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

}
