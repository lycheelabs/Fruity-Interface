using UnityEngine;
using TMPro;
using System;

namespace LycheeLabs.FruityInterface.Elements {

    public class TextWrapper : RectTransformWrapper {

        protected readonly TMP_Text text;
        //private InlineGraphicManager graphicManager;

        public TextWrapper (GameObject gameObject)
            : base(gameObject) {
            text = gameObject.GetComponent<TMP_Text>();
            if (text == null) {
                throw new NullReferenceException("No TMP_Text component found");
            }
        }

        public TextWrapper (Transform transform) : this(transform.gameObject) { }

        public void ForceLayout () {
            text.ForceMeshUpdate();
        }

        public string Text {
            get { return text.text; }
            set { text.text = value; }
        }

        public Color Color {
            get { return text.color; }
            set { text.color = value; }
        }

        public void SetGradient (Color textColorTop, Color textColorBottom) {
            text.colorGradient = new VertexGradient(textColorTop, textColorTop, textColorBottom, textColorBottom);
        }

        public Material DuplicateMaterial () {
            text.fontSharedMaterial = new Material(text.fontSharedMaterial);
            return text.fontSharedMaterial;
        }

        public int TextSize {
            set {
                text.enableAutoSizing = false;
                text.fontSize = value;
            }
        }

        public bool Wrapping {
            set { text.enableWordWrapping = value; }
        }

        public int CharSpacing {
            set { text.characterSpacing = value; }
        }

        public FontStyles Style {
            set { text.fontStyle = value; }
        }

        public void TextAutoSize (float min, float max) {
            text.enableAutoSizing = true;
            text.fontSizeMin = min;
            text.fontSizeMax = max;
        }

        public TextOverflowModes OverflowMode {
            set { text.overflowMode = value; }
        }

        public int MaxVisibleCharacters {
            set { text.maxVisibleCharacters = value; }
        }

        public int NumCharacters {
            get { return text.textInfo.characterCount; }
        }

        public char GetCharacterAt (int index) {
            if (index >= text.textInfo.characterInfo.Length || index < 0) return ' ';
            return text.textInfo.characterInfo[index].character;
        }

        public TMP_FontAsset Font {
            set { text.font = value; }
        }

        public TMP_SpriteAsset Sprites {
            set {
                /*graphicManager = gameObject.GetComponent<InlineGraphicManager>();
                if (graphicManager == null) {
                    graphicManager = gameObject.AddComponent<InlineGraphicManager> ();
                }
                graphicManager.spriteAsset = value; */
            }
        }

        public Material Material {
            get { return text.fontSharedMaterial; }
            set { text.fontSharedMaterial = value; }
        }

        public int PreferredHeight {
            get { return (int)(text.preferredHeight); }
        }

        public int RenderedHeight {
            get { return (int)(text.renderedHeight); }
        }

        public TextAlignmentOptions Alignment {
            set { text.alignment = value; }
        }

        public float LineSpacing {
            set { text.lineSpacing = value; }
        }

        public float PreferredWidth {
            get { return text.preferredWidth; }
        }

        public float RenderedWidth {
            get { return text.renderedWidth; }
        }

    }

}
