using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface  {

    internal static class InterfaceHelpers {
                
        public static Vector2 WorldPointToScreenPoint (Camera camera,Vector3 mousePosition) {
            if (camera == null) return default;
            return camera.WorldToScreenPoint(mousePosition);
        }

        public static Vector3 ScreenPointToWorldPoint (Camera camera,Vector3 mousePosition, Plane plane) {
            if (camera == null) return default;
            var worldPoint = camera.ScreenToWorldPoint(mousePosition);
            return IntersectWithPlane(camera, worldPoint, plane);
        }

        public static Ray ScreenPointToRay (Camera camera, Vector3 mousePosition) {
            if (camera == null) return default;
            return camera.ScreenPointToRay(mousePosition);
        }
        
        public static Vector3 IntersectWithPlane (Camera camera, Vector3 vector, Plane plane) {
            if (camera == null) return default;
            
            // Already intersecting?
            if (vector.y == 0 || camera == null) {
                return vector;
            }


            //Project the ray forwards
            float collisionDistance = 0;
            Ray ray = new Ray(vector, camera.transform.forward);
            if (plane.Raycast(ray, out collisionDistance)) {
                // get the hit point:
                return ray.GetPoint(collisionDistance);
            }

            //Project the ray backwards
            ray = new Ray(vector, -camera.transform.forward);
            if (plane.Raycast(ray, out collisionDistance)) {
                // get the hit point:
                return ray.GetPoint(collisionDistance);
            }

            // Failure! Get approximate projection to infinity
            return ray.GetPoint(99999);
        }
        
        public static float Value (this AspectRatio aspect) {
            if (aspect == AspectRatio.WIDESCREEN) return 16f / 9f;
            if (aspect == AspectRatio.TALLSCREEN) return 9f / 16f;
            if (aspect == AspectRatio.ULTRAWIDE) return 21f / 9f;
            if (aspect == AspectRatio.ULTRATALL) return 9f / 21f;
            return 4f / 3f;
        }

    }

}