# BlazeFace
[MediaPipe BlazeFace](https://google.github.io/mediapipe/solutions/face_detection.html) for face detection. This package requires [NatML](https://github.com/natmlx/NatML).

## Detecting Faces in an Image
First, create the BlazeFace predictor:
```csharp
// Fetch the model data from Hub
var modelData = await MLModelData.FromHub("@natsuite/blazeface");
// Deserialize the model
var model = modelData.Deserialize();
// Create the BlazeFace predictor
var predictor = new BlazeFacePredictor(model);
```

Then detect faces in the image:
```csharp
// Create image feature
Texture2D image = ...;
var imageFeature = new MLImageFeature(image); // This also accepts a `Color32[]` or `byte[]`
(imageFeature.mean, imageFeature.std) = modelData.normalization;
imageFeature.aspectMode = modelData.aspectMode;
// Detect faces
Rect[] faces = predictor.Predict(imageFeature);
```
___

## Requirements
- Unity 2020.3+
- [NatML 1.0.11+](https://github.com/natmlx/NatML)

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://discord.gg/y5vwgXkz2f).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/open-beta-natml-machine-learning-runtime.1109339/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!