using System.Collections.Generic;
using UnityEngine;
using TensorFlowLite;

public class EmotionDetect : BaseVisionTask
{
    [SerializeField, FilePopup("*.tflite")] private string modelFile = "emotion_model.tflite";
    private float[,] emotionOutputs;
    public Matrix4x4 InputTransformMatrix { get; private set; } = Matrix4x4.identity;

    public EmotionDetect(string modelPath)
    {
        var interpreterOptions = new InterpreterOptions();
        interpreterOptions.AddGpuDelegate();
        Load(FileUtil.LoadFile(modelPath), interpreterOptions);
    }

    public override void Load(byte[] model, InterpreterOptions options)
    {
        base.Load(model, options);
        emotionOutputs = new float[1, 7]; // Giả sử model có một output tensor và trả về xác suất của các cảm xúc
    }

    protected override void PreProcess(Texture texture)
    {
        // Resize texture to the required size of the model (48x48)
        Texture2D resizedTexture = ResizeTexture(texture as Texture2D, 48, 48);
        InputTransformMatrix = textureToTensor.GetAspectScaledMatrix(resizedTexture, AspectMode);
        var input = textureToTensor.Transform(resizedTexture, InputTransformMatrix);
        interpreter.SetInputTensorData(inputTensorIndex, input);
    }

    private Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
    {
        RenderTexture tempRT = RenderTexture.GetTemporary(width, height, 24);
        Graphics.Blit(originalTexture, tempRT);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tempRT;
        Texture2D processedTexture = new Texture2D(width, height);
        processedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        processedTexture.Apply();
        RenderTexture.ReleaseTemporary(tempRT);
        RenderTexture.active = previous; // Restore previous RenderTexture
        return processedTexture;
    }

    protected override void PostProcess()
    {
        interpreter.GetOutputTensorData(0, emotionOutputs);
        ProcessEmotionResults(emotionOutputs);
    }

    private void ProcessEmotionResults(float[,] results)
    {
        int maxIndex = 0;
        float maxScore = results[0, 0];
        for (int i = 1; i < results.GetLength(1); i++)
        {
            if (results[0, i] > maxScore)
            {
                maxIndex = i;
                maxScore = results[0, i];
            }
        }

        Debug.Log($"Detected Emotion: {GetEmotionLabel(maxIndex)} with confidence {maxScore * 100:F2}%");
    }

    private string GetEmotionLabel(int index)
    {
        string[] emotions = { "Angry", "Disgust", "Fear", "Happy", "Sad", "Surprise", "Neutral" };
        return emotions[index];
    }

    public List<string> DetectEmotionsForFaces(List<FaceDetect.Result> faceResults, Texture texture)
    {
        List<string> detectedEmotions = new List<string>();
        foreach (var face in faceResults)
        {
            Texture2D faceTexture = CropFaceTexture(texture, face.rect);
            Run(faceTexture);
            detectedEmotions.Add($"Emotion for face at {face.rect}: {GetEmotionLabelFromLastResults()}");
        }
        return detectedEmotions;
    }

    private string GetEmotionLabelFromLastResults()
    {
        int maxIndex = 0;
        float maxScore = emotionOutputs[0, 0];
        for (int i = 1; i < emotionOutputs.GetLength(1); i++)
        {
            if (emotionOutputs[0, i] > maxScore)
            {
                maxIndex = i;
                maxScore = emotionOutputs[0, i];
            }
        }
        return $"{GetEmotionLabel(maxIndex)} with confidence {maxScore * 100:F2}%";
    }

    private Texture2D CropFaceTexture(Texture texture, Rect rect)
    {
        var texture2D = texture as Texture2D;
        var x = Mathf.FloorToInt(rect.x * texture2D.width);
        var y = Mathf.FloorToInt(rect.y * texture2D.height);
        var width = Mathf.FloorToInt(rect.width * texture2D.width);
        var height = Mathf.FloorToInt(rect.height * texture2D.height);

        Color[] pixels = texture2D.GetPixels(x, y, width, height);
        Texture2D faceTexture = new Texture2D(width, height);
        faceTexture.SetPixels(pixels);
        faceTexture.Apply();
        return faceTexture;
    }
}
