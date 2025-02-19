using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class CanvasWrapper : RectTransformWrapper {

        protected readonly Canvas canvas;

        public CanvasWrapper (GameObject gameObject) : base(gameObject) {
            canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null) {
                throw new NullReferenceException("No Canvas component found");
            }
        }

        public CanvasWrapper (Transform transform) : this(transform.gameObject) { }

        public string SortingLayer {
            set { canvas.sortingLayerName = value; }
        }

        public int SortingLayerID {
            set { canvas.sortingLayerID = value; }
        }

        public int SortingOrder {
            set { canvas.sortingOrder = value; }
        }

        public float PlaneDistance {
            set { canvas.planeDistance = value; }
        }

        public bool Visible {
            set { canvas.enabled = value; }
        }

        public Camera Camera {
            set { canvas.worldCamera = value; }
        }

    }

}

