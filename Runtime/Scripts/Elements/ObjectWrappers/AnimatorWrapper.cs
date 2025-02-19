using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

	public class AnimatorWrapper : TransformWrapper {

		private readonly Animator animator;

		public AnimatorWrapper (GameObject gameObject, bool resetOnDisable = false)
			: base(gameObject) {
            animator = gameObject.GetComponent<Animator>();
            if (animator == null) {
                throw new NullReferenceException("No Animator component found");
            }
			animator.keepAnimatorStateOnDisable = !resetOnDisable;
		}

		public AnimatorWrapper (Transform transform) : this(transform.gameObject) { }

		public void Play (string stateName, int layer = 0) {
			animator.Play(stateName, layer, 0);
		}

		public bool IsPlaying (string stateName, int layer = 0) {
			return animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
		}

		public float NormalisedTime (int layer = 0) {
			return animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
		}

		public bool Enabled {
			set { animator.enabled = value; }
		}

		public float Speed {
			set { animator.speed = value; }
		}

	}

}