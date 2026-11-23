using LycheeLabs.InputTree;
using InputTree.Tests;
using NUnit.Framework;
using UnityEngine;

namespace Layout {

    public class ScreenBoundsTests {

        private const float Tol = 0.01f;

        [Test]
        public void Constructor_SetsSafeDefaults() {
            var bounds = new ScreenBounds();

            Assert.AreEqual(1f, bounds.WindowAspectRatio, Tol);
            Assert.AreEqual(1f, bounds.CameraScaling, Tol);
            Assert.AreEqual(1f, bounds.UIScaling, Tol);
        }

        [Test]
        public void ApplySize_AspectWithinRange_NoLetterbox() {
            var bounds = new ScreenBounds();

            bounds.ApplySize(1920, 1080, 1f, 2f);

            Assert.AreEqual(1920f / 1080f, bounds.WindowAspectRatio, Tol);
            Assert.AreEqual(1920f, bounds.BoxedCanvasSize.x, Tol);
            Assert.AreEqual(1080f, bounds.BoxedCanvasSize.y, Tol);
            Assert.AreEqual(0f, bounds.LetterboxWidth, Tol);
            Assert.AreEqual(0f, bounds.LetterboxHeight, Tol);
            Assert.AreEqual(1f, bounds.UIScaling, Tol);
            Assert.AreEqual(1f, bounds.CameraScaling, Tol);
        }

        [Test]
        public void ApplySize_TooWide_LetterboxesHorizontally() {
            var bounds = new ScreenBounds();

            bounds.ApplySize(3000, 1000, 1f, 2f);

            Assert.AreEqual(2f, bounds.BoxedAspectRatio, Tol);
            Assert.AreEqual(2160f, bounds.BoxedCanvasSize.x, Tol);
            Assert.AreEqual(1080f, bounds.LetterboxWidth, Tol);
            Assert.AreEqual(0f, bounds.LetterboxHeight, Tol);
            Assert.AreEqual(3240f, bounds.WindowCanvasSize.x, Tol);
            Assert.AreEqual(1.08f, bounds.UIScaling, Tol);
            Assert.AreEqual(1f, bounds.CameraScaling, Tol);
            Assert.AreEqual(540f, bounds.LetterboxOffset.x, Tol);
        }

        [Test]
        public void ApplySize_TooNarrow_ScalesCameraAndLetterboxesVertically() {
            var bounds = new ScreenBounds();

            bounds.ApplySize(500, 1000, 1f, 2f);

            Assert.AreEqual(1f, bounds.BoxedAspectRatio, Tol);
            Assert.AreEqual(0f, bounds.LetterboxWidth, Tol);
            Assert.AreEqual(1080f, bounds.LetterboxHeight, Tol);
            Assert.AreEqual(1080f, bounds.WindowCanvasSize.x, Tol);
            Assert.AreEqual(2160f, bounds.WindowCanvasSize.y, Tol);
            Assert.AreEqual(2.16f, bounds.UIScaling, Tol);
            Assert.AreEqual(2f, bounds.CameraScaling, Tol);
        }

        [Test]
        public void ApplySize_ZeroSize_ClampsToOnePixel() {
            var bounds = new ScreenBounds();

            bounds.ApplySize(0, 0, 1f, 2f);

            Assert.AreEqual(1f, bounds.WindowAspectRatio, Tol);
            Assert.AreEqual(1080f, bounds.WindowCanvasSize.x, Tol);
            Assert.AreEqual(1080f, bounds.WindowCanvasSize.y, Tol);
        }
    }

}
