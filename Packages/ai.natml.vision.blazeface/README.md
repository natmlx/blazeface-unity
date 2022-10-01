# BlazeFace
[MediaPipe BlazeFace](https://google.github.io/mediapipe/solutions/face_detection.html) for realtime face detection in Unity Engine with [NatML](https://github.com/natmlx/NatML).

## Installing BlazeFace
Add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.vision.blazeface": "1.0.5"
  }
}
```

## Detecting Faces in an Image
First, create the BlazeFace predictor:
```csharp
// Fetch the model data from NatML
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
var imageFeature = new MLImageFeature(image);
// Set normalization and aspect mode
(imageFeature.mean, imageFeature.std) = modelData.normalization;
imageFeature.aspectMode = modelData.aspectMode;
// Detect faces
Rect[] faces = predictor.Predict(imageFeature);
```
___

## Requirements
- Unity 2021.2+

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://hub.natml.ai/community).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/open-beta-natml-machine-learning-runtime.1109339/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!