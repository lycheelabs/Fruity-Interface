using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace LycheeLabs.FruityInterface.Elements {

	public class SortingGroupWrapper : TransformWrapper {

		private readonly SortingGroup sortGroup;

		public SortingGroupWrapper (GameObject gameObject)
				: base(gameObject) {
			sortGroup = gameObject.GetComponent<SortingGroup>();
            if (sortGroup == null) {
                throw new NullReferenceException("No SortingGroup component found");
            }
        }

		public SortingGroupWrapper (Transform transform) : this(transform.gameObject) { }

		public string SortingLayer {
			set { sortGroup.sortingLayerName = value; }
		}

		public int SortingLayerID {
			set { sortGroup.sortingLayerID = value; }
		}

		public int SortingOrder {
			set { sortGroup.sortingOrder = value; }
		}

	}

}