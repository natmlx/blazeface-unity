/* 
*   BlazeFace
*   Copyright (c) 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Visualizers {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Vision;
    using VideoKit.UI;

    /// <summary>
    /// </summary>
    [RequireComponent(typeof(VideoKitCameraView))]
    public sealed class BlazeFaceVisualizer : MonoBehaviour {

        #region --Inspector--
        public Image faceRect;
        public RectTransform keypoint;
        #endregion


        #region --Client API--
        /// <summary>
        /// Render a set of detected faces.
        /// </summary>
        /// <param name="faces">Faces to render.</param>
        public void Render (params BlazeFacePredictor.Face[] faces) {
            // Delete current
            foreach (var rect in currentRects)
                GameObject.Destroy(rect.gameObject);
            foreach (var keypoint in currentKeypoints)
                GameObject.Destroy(keypoint.gameObject);
            currentRects.Clear();
            currentKeypoints.Clear();
            // Render rects
            foreach (var face in faces) {
                var prefab = Instantiate(faceRect, transform);
                prefab.gameObject.SetActive(true);
                VisualizeRect(prefab, face.rect);
                foreach (var point in face) {
                    var keypointUI = Instantiate(keypoint, transform);
                    keypointUI.gameObject.SetActive(true);
                    VisualizeAnchor(point, keypointUI);
                    currentKeypoints.Add(keypointUI);
                }
                currentRects.Add(prefab);
            }
        }
        #endregion


        #region --Operations--
        private readonly List<Image> currentRects = new List<Image>();
        private readonly List<RectTransform> currentKeypoints = new List<RectTransform>();

        private void VisualizeRect (Image prefab, Rect faceRect) {
            var rectTransform = prefab.transform as RectTransform;
            var imageTransform = transform as RectTransform;
            rectTransform.anchorMin = 0.5f * Vector2.one;
            rectTransform.anchorMax = 0.5f * Vector2.one;
            rectTransform.pivot = Vector2.zero;
            rectTransform.sizeDelta = Vector2.Scale(imageTransform.rect.size, faceRect.size);
            rectTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, faceRect.position);
        }

        private void VisualizeAnchor (Vector2 point, RectTransform anchor) {
            var imageTransform = transform as RectTransform;
            anchor.anchorMin = 0.5f * Vector2.one;
            anchor.anchorMax = 0.5f * Vector2.one;
            anchor.pivot = 0.5f * Vector2.one;
            anchor.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, point);
        }
        #endregion
    }
}