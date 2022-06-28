/* 
*   BlazeFace
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Visualizers {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// </summary>
    [RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
    public sealed class BlazeFaceVisualizer : MonoBehaviour {

        #region --Client API--
        /// <summary>
        /// Render a set of detected faces.
        /// </summary>
        /// <param name="image">Image which detections are made on.</param>
        /// <param name="faces">Faces to render.</param>
        public void Render (Texture image, params Rect[] faces) {
            // Delete current
            foreach (var rect in currentRects)
                GameObject.Destroy(rect.gameObject);
            currentRects.Clear();
            // Display image
            rawImage.texture = image;
            aspectFitter.aspectRatio = (float)image.width / image.height;            
            // Render rects
            foreach (var face in faces) {
                var prefab = Instantiate(faceRect, transform);
                prefab.gameObject.SetActive(true);
                Render(prefab, face);                
                currentRects.Add(prefab);
            }
        }
        #endregion


        #region --Operations--
        [SerializeField] Image faceRect;
        RawImage rawImage;
        AspectRatioFitter aspectFitter;
        List<Image> currentRects = new List<Image>();

        void Awake () {
            rawImage = GetComponent<RawImage>();
            aspectFitter = GetComponent<AspectRatioFitter>();
        }

        void Render (Image prefab, Rect faceRect) {
            var rectTransform = prefab.transform as RectTransform;
            var imageTransform = rawImage.transform as RectTransform;
            rectTransform.anchorMin = 0.5f * Vector2.one;
            rectTransform.anchorMax = 0.5f * Vector2.one;
            rectTransform.pivot = Vector2.zero;
            rectTransform.sizeDelta = Vector2.Scale(imageTransform.rect.size, faceRect.size);
            rectTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, faceRect.position);
        }
        #endregion
    }
}