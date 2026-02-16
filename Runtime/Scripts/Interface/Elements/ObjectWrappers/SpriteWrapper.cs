using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class SpriteWrapper : TransformWrapper {

        protected readonly SpriteRenderer spriteRenderer;

        public SpriteWrapper (GameObject gameObject) : base(gameObject) {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) {
                throw new NullReferenceException("No SpriteRenderer component found");
            }
        }

        public SpriteWrapper (Transform transform) : this(transform.gameObject) { }

        public SpriteRenderer SpriteRenderer {
            get { return spriteRenderer; }
        }

        public Sprite Sprite {
            get { return spriteRenderer.sprite; }
            set { spriteRenderer.sprite = value; }
        }

        public string SortingLayer {
            set { spriteRenderer.sortingLayerName = value; }
        }

        public int SortingOrder {
            set { spriteRenderer.sortingOrder = value; }
        }

        public Color Color {
            get { return spriteRenderer.color; }
            set { spriteRenderer.color = value; }
        }

        public bool Visible {
            set { spriteRenderer.enabled = value; }
            get { return spriteRenderer.enabled; }
        }

        public Material Material {
            set { spriteRenderer.sharedMaterial = value; }
        }

        public void SetTiled (float w, float h) {
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = new Vector2(w, h);
        }

        public void SetSliced (float w, float h) {
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.size = new Vector2(w, h);
        }

        public void SetRendererFloat (string property, float value) {
            var block = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(block);
            block.SetFloat(property, value);
            spriteRenderer.SetPropertyBlock(block);
        }

        public void SetRendererColor (string property, Color value) {
            var block = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(block);
            block.SetColor(property, value);
            spriteRenderer.SetPropertyBlock(block);
        }

    }

}