/* 
*   BlazeFace
*   Copyright (c) 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// MediaPipe BlazeFace face predictor.
    /// This predictor accepts an image feature and produces a list of face rectangles.
    /// Face rectangles are always specified in normalized coordinates.
    /// </summary>
    public sealed class BlazeFacePredictor : IMLPredictor<Rect[]> {

        #region --Client API--
        /// <summary>
        /// Predictor tag.
        /// </summary>
        public const string Tag = "@natsuite/blazeface";

        /// <summary>
        /// Detect faces in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected faces.</returns>
        public Rect[] Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"BlazeFace predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            var imageType = MLImageType.FromType(input.type);
            if (!imageType)
                throw new ArgumentException(@"BlazeFace predictor expects an an array or image feature", nameof(inputs));
            // Preprocess
            var imageFeature = input as MLImageFeature;
            if (imageFeature != null) {
                (imageFeature.mean, imageFeature.std) = model.normalization;
                imageFeature.aspectMode = model.aspectMode;
            }
            // Predict
            using var inputFeature = (input as IMLEdgeFeature).Create(inputType);
            using var outputFeatures = model.Predict(inputFeature);
            // Decode
            var (widthInv, heightInv) = (1f / inputType.width, 1f / inputType.height);
            var scoresFeature0 = new MLArrayFeature<float>(outputFeatures[0]);      // (1,512,1)
            var scoresFeature1 = new MLArrayFeature<float>(outputFeatures[1]);      // (1,384,1)
            var regressionFeature0 = new MLArrayFeature<float>(outputFeatures[2]);  // (1,512,16)
            var regressionFeature1 = new MLArrayFeature<float>(outputFeatures[3]);  // (1.384,16)
            var anchorOffset = 0;
            candidateBoxes.Clear();
            candidateScores.Clear();
            foreach (var (scores, regression) in new [] {
                (scoresFeature0, regressionFeature0),
                (scoresFeature1, regressionFeature1)
            }) {
                for (int i = 0, len = scores.shape[1]; i < len; ++i) {
                    // Check score
                    var score = scores[i];
                    if (score < minScore)
                        continue;
                    // Extract
                    var anchor = anchors[i + anchorOffset];
                    var cx = regression[0,i,0] + anchor.x;
                    var cy = regression[0,i,1] + anchor.y;
                    var w = regression[0,i,2];
                    var h = regression[0,i,3];
                    var rawBox = new Rect((cx - w / 2) * widthInv, 1f - (cy + h / 2) * heightInv, w * widthInv, h * heightInv);
                    var box = imageFeature?.TransformRect(rawBox, inputType) ?? rawBox;
                    // Add
                    candidateBoxes.Add(box);
                    candidateScores.Add(score);
                }
                anchorOffset += scores.shape[1];
            }
            // NMS
            var keepIdx = MLImageFeature.NonMaxSuppression(candidateBoxes, candidateScores, maxIoU);
            var result = keepIdx.Select(i => candidateBoxes[i]).ToArray();
            // Return
            return result;
        }

        /// <summary>
        /// Dispose the predictor and release resources.
        /// </summary>
        public void Dispose () => model.Dispose();

        /// <summary>
        /// Create the BlazeFace predictor.
        /// </summary>
        /// <param name="minScore">Minimum candidate score.</param>
        /// <param name="maxIoU">Maximum intersection-over-union score for overlap removal.</param>
        /// <param name="configuration">Model configuration.</param>
        /// <param name="accessKey">NatML access key.</param>
        public static async Task<BlazeFacePredictor> Create (
            float minScore = 0.5f,
            float maxIoU = 0.5f,
            MLEdgeModel.Configuration configuration = null,
            string accessKey = null
        ) {
            var model = await MLEdgeModel.Create(Tag, configuration, accessKey);
            var predictor = new BlazeFacePredictor(model, minScore, maxIoU);
            return predictor;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private readonly float minScore;
        private readonly float maxIoU;
        private readonly MLImageType inputType;
        private readonly Vector2[] anchors;
        private readonly List<Rect> candidateBoxes;
        private readonly List<float> candidateScores;
        private static readonly (int, int)[] AnchorGridSizes = new [] { (16, 16), (8, 8) };
        private static readonly int[] AnchorCounts = new [] { 2, 6 };

        private BlazeFacePredictor (MLEdgeModel model, float minScore, float maxIoU) {
            this.model = model;
            this.minScore = Logit(minScore);
            this.maxIoU = maxIoU;
            this.inputType = model.inputs[0] as MLImageType;
            this.anchors = GenerateAnchors(inputType.width, inputType.height);
            this.candidateBoxes = new List<Rect>(anchors.Length);
            this.candidateScores = new List<float>(anchors.Length);
        }

        private static Vector2[] GenerateAnchors (int width, int height) {
            var result = new List<Vector2>();
            for (var i = 0; i < AnchorGridSizes.Length; ++i) {
                var (cols, rows) = AnchorGridSizes[i];
                var xStride = (float)width / cols;
                var yStride = (float)height / rows;
                var count = AnchorCounts[i];
                for (var y = 0; y < rows; ++y) {
                    var anchorY = yStride * (y + 0.5f);
                    for (var x = 0; x < cols; ++x) {
                        var anchorX = xStride * (x + 0.5f);
                        for (int n = 0; n < count; ++n)
                            result.Add(new Vector2(anchorX, anchorY));
                    }
                }
            }
            return result.ToArray();
        }

        private static float Logit (float x) => x > 0 ? x < 1 ? Mathf.Log(x / (1f - x)) : int.MaxValue : int.MinValue;
        #endregion
    }
}