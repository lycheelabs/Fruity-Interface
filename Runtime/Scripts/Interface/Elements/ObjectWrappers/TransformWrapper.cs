using UnityEngine;
using System;

namespace LycheeLabs.FruityInterface.Elements {

	// Allows for animation of regular transforms
	public class TransformWrapper {

		protected readonly GameObject gameObject;
		protected Vector3 stretching;
		protected float scaling;

		public TransformWrapper (GameObject gameObject) {
			if (gameObject == null) {
				throw new NullReferenceException("No GameObject provided");
			}
			this.gameObject = gameObject;
			stretching = gameObject.transform.localScale;
			scaling = 1f;
		}

		public TransformWrapper (Transform transform) : this(transform.gameObject) { }

		public GameObject GameObject {
			get { return gameObject; }
		}

		public Transform Transform {
			get { return gameObject.transform; }
		}

		public string Name {
			set { gameObject.name = value; }
			get { return gameObject.name; }
		}

		public Transform Parent {
			set { gameObject.transform.SetParent(value, false); }
			get { return gameObject.transform.parent; }
		}

		public void SetParent (Transform newParent, bool worldPositionStays) {
			gameObject.transform.SetParent(newParent, worldPositionStays);
		}

		public Transform GetChild (int index) {
			return gameObject.transform.GetChild(index);
		}

		public bool Active {
			get { return gameObject.activeInHierarchy; }
			set { gameObject.SetActive(value); }
		}

		public string Layer {
			set { gameObject.layer = LayerMask.NameToLayer(value); }
		}

		public int IntLayer {
			set { gameObject.layer = value; }
			get { return gameObject.layer; }
		}

		public bool IsDestroyed => gameObject == null;

		public void Destroy () {
			UnityEngine.Object.Destroy(gameObject);
		}

		// ------------------ Transformation -----------------------

		public virtual Vector3 LocalPosition {
			get { return gameObject.transform.localPosition; }
			set { gameObject.transform.localPosition = value; }
		}

		public Vector3 WorldPosition {
			get { return gameObject.transform.position; }
			set { gameObject.transform.position = value; }
		}

		/*public virtual UIPosition UIPosition {
			get { return new UIPosition3D(gameObject.transform.position); }
		}*/

		public float Scale {
			set {
				scaling = value;
				gameObject.transform.localScale = stretching * scaling;
			}
			get { return scaling; }
		}

		public Vector3 Stretch {
			set {
				stretching = value;
				gameObject.transform.localScale = stretching * scaling;
			}
			get { return stretching; }
		}

		public Vector3 CombinedScale {
			get { return gameObject.transform.localScale; }
		}

		public float ZRotation {
			get { return gameObject.transform.localEulerAngles.z; }
			set { gameObject.transform.localEulerAngles = new Vector3(0, 0, value); }
		}

		public Vector3 Angles {
			get { return gameObject.transform.localEulerAngles; }
			set { gameObject.transform.localEulerAngles = value; }
		}

	}

}