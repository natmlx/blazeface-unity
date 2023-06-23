# BlazeFace
[MediaPipe BlazeFace](https://google.github.io/mediapipe/solutions/face_detection.html) for realtime face detection in Unity Engine with [NatML](https://github.com/natmlx/natml-unity).

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
    "ai.natml.vision.blazeface": "1.0.6"
  }
}
```

## Detecting Faces in an Image
First, create the BlazeFace predictor:
```csharp
// Create the BlazeFace predictor
var predictor = await BlazeFacePredictor.Create();
```

Then detect faces in the image:
```csharp
// Create image feature
Texture2D image = ...;
// Detect faces
Rect[] faces = predictor.Predict(image);
```
___

## Requirements
- Unity 2021.2+

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://natml.ai/community).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!