/* 
*   BlazeFace
*   Copyright (c) 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using NatML.Vision;
    using NatML.Visualizers;
    using VideoKit;

    public sealed class BlazeFaceSample : MonoBehaviour {

        [Header(@"Camera")]
        public VideoKitCameraManager cameraManager;  

        [Header(@"UI")]
        public BlazeFaceVisualizer visualizer;

        private BlazeFacePredictor predictor;

        private async void Start () {
            // Create the RVM predictor
            predictor = await BlazeFacePredictor.Create();
            // Listen for camera frames
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
        }

        private void OnCameraFrame (CameraFrame frame) {
            // Predict
            var faces = predictor.Predict(frame);
            // Visualize
            visualizer.Render(faces);
        }

        private void OnDisable () {
            // Stop listening for camera frames
            cameraManager.OnCameraFrame.RemoveListener(OnCameraFrame);
            // Dispose the predictor
            predictor?.Dispose();
        }
    }
}