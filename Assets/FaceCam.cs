/* 
*   FaceCam
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using NatML;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using NatML.Features;
    using NatML.Vision;
    using NatML.Visualizers;

    public class FaceCam : MonoBehaviour {

        [Header(@"Visualization")]
        public BlazeFaceVisualizer visualizer;

        CameraDevice cameraDevice;
        TextureOutput previewTextureOutput;

        MLModelData modelData;
        MLModel model;
        BlazeFacePredictor predictor;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            previewTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(previewTextureOutput);
            // Display the camera preview
            var previewTexture = await previewTextureOutput;
            visualizer.Render(previewTexture);
            // Fetch the BlazeFace model data
            Debug.Log("Fetching model from NatML...");
            modelData = await MLModelData.FromHub("@natsuite/blazeface");
            // Deserialize the model
            model = modelData.Deserialize();
            // Create the BlazeFace predictor
            predictor = new BlazeFacePredictor(model);
        }

        void Update () {
            // Check that the model has been downloaded
            if (predictor == null)
                return;
            // Create input feature
            var previewTexture = previewTextureOutput.texture;
            var inputFeature = new MLImageFeature(previewTexture.GetRawTextureData<byte>(), previewTexture.width, previewTexture.height);
            (inputFeature.mean, inputFeature.std) = modelData.normalization;
            inputFeature.aspectMode = modelData.aspectMode;
            // Predict
            var faces = predictor.Predict(inputFeature);
            // Visualize
            visualizer.Render(previewTexture, faces);
        }

        void OnDisable () {
            // Dispose the predictor and model
            model?.Dispose();
        }
    }
}