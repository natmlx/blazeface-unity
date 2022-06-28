/* 
*   BlazeFace
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using System.Threading.Tasks;
    using UnityEngine;
    using NatML.Features;
    using NatML.Vision;
    using NatML.Visualizers;

    public sealed class BlazeFaceSample : MonoBehaviour {        

        [Header(@"UI")]
        public BlazeFaceVisualizer visualizer;

        MLModelData modelData;
        MLModel model;
        BlazeFacePredictor predictor;
        WebCamTexture webCamTexture;
        Color32[] pixelBuffer;

        async void Start () {
            Debug.Log("Fetching model data from NatML...");
            // Fetch the model from NatML
            modelData = await MLModelData.FromHub("@natsuite/blazeface");
            // Deserialize the model
            model = modelData.Deserialize();
            // Create the BlazeFace predictor
            predictor = new BlazeFacePredictor(model);
            // Start webcam
            webCamTexture = new WebCamTexture(1280, 720, 30);
            webCamTexture.Play();
            // Create and display the destination segmentation image
            while (webCamTexture.width == 16 || webCamTexture.height == 16)
                await Task.Yield();
            // Display webcam
            visualizer.Render(webCamTexture);
        }

        void Update () {
            // Check that predictor has downloaded
            if (predictor == null)
                return;
            // Check that the camera frame updated
            if (!webCamTexture.didUpdateThisFrame)
                return;
            // Create input feature
            pixelBuffer = webCamTexture.GetPixels32(pixelBuffer);
            var inputFeature = new MLImageFeature(pixelBuffer, webCamTexture.width, webCamTexture.height);
            (inputFeature.mean, inputFeature.std) = modelData.normalization;
            inputFeature.aspectMode = modelData.aspectMode;
            // Predict
            var faces = predictor.Predict(inputFeature);
            // Visualize
            visualizer.Render(webCamTexture, faces);
        }

        void OnDisable () {
            // Dispose the model
            model?.Dispose();
        }
    }
}