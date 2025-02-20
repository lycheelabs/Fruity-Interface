using System.Collections.Generic;
using LycheeLabs.FruityInterface;
using UnityEngine;


public class MouseRaycaster  {
    
    private static readonly float MAX_DISTANCE = float.MaxValue;
    public static readonly int MAX_HITS = 100;
    private static LayerMask everything = -1;

    private RaycastHit[] RaycastBuffer = new RaycastHit[MAX_HITS];
    private RaycastHit2D[] RaycastBuffer2D = new RaycastHit2D[MAX_HITS];
    private List<MouseTarget> resolutionStack = new List<MouseTarget>();

    public void CollideAndResolve (out MouseTarget target, out Vector3 targetPoint) {
        target = null;
        targetPoint = Vector3.zero; 
        if (!Mouse.MouseIsInBounds) {
            return;
        }

        // Cast a ray, collecting all hits (2d and 3d)
        Ray ray = InterfaceConfig.ScreenPointToRay(Input.mousePosition);
        int hitCount = Physics.RaycastNonAlloc(ray, RaycastBuffer, MAX_DISTANCE);
        int hitCount2D = Physics2D.GetRayIntersectionNonAlloc(ray, RaycastBuffer2D, MAX_DISTANCE, everything);

        // Extract closest enabled MouseTarget (2d and 3d)
        float bestDistance = MAX_DISTANCE;
        for (int i = 0; i < hitCount; i++) {
            var hit = RaycastBuffer[i];
            if (hit.distance < bestDistance) {
                var node = hit.collider.gameObject.GetComponent<InterfaceNode>();
                var nodeTarget = Resolve(node as MouseTarget, hit.point);

                if (node != null && nodeTarget != null && node.InputEnabledInHierarchy) {
                    target = nodeTarget;
                    targetPoint = hit.point;
                    bestDistance = hit.distance;
                }
            }
        }
        for (int i = 0; i < hitCount2D; i++) {
            var hit = RaycastBuffer2D[i];
            if (hit.distance < bestDistance) {
                var node = hit.collider.gameObject.GetComponent<InterfaceNode>();
                var nodeTarget = Resolve(node as MouseTarget, hit.point);

                if (node != null && nodeTarget != null && node.InputEnabledInHierarchy) {
                    target = nodeTarget;
                    targetPoint = hit.point;
                    bestDistance = hit.distance;
                }
            }
        }
    }

    private MouseTarget Resolve (MouseTarget target, Vector3 point) {
        var nextTarget = target;
        resolutionStack.Clear();

        while (nextTarget != null && !resolutionStack.Contains(nextTarget)) {
            resolutionStack.Add(nextTarget);
            target = nextTarget;
            nextTarget = target.ResolveTarget(point);
        }
        return target;
    }

}
