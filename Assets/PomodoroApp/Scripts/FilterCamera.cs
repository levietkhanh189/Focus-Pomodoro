using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEditor;

public class FilterCamera : MonoBehaviour
{
    public Color mainColor;
    public static FilterCamera instance;

    public Camera myCamera;
    public RawImage rawImage;
    private static int count;
    public Text nameText;
    public Text emotionText;
    public EmotionDetectSample emotionDetect;
    public InputField inputField;
    public FaceRecognitionClient recognitionClient;
    private void Awake()
    {
        instance = this;
    }

    public void TakeScreenshot()
    {
        Texture2D textureToSave = ConvertToTexture2D(rawImage.mainTexture);

        if (textureToSave != null)
        {
            // Encode texture into PNG
            byte[] bytes = textureToSave.EncodeToPNG();

#if UNITY_EDITOR
            // Create a file path in the Assets folder
            count++;
            string filePath = Path.Combine("Assets", "SavedTextures", "Photo_"+count+".png");
            // Ensure directory exists
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(filePath, bytes);
            // Refresh the AssetDatabase to show the new file in the Editor
            AssetDatabase.Refresh();
            Debug.Log("Saved Texture to " + filePath);
#else
                Debug.LogError("Texture saving is only supported in the Unity Editor.");
#endif
        }
        else
        {
            Debug.LogError("No texture found to save!");
        }
    }

    public string TakeScreenshotGetString()
    {
        Texture2D textureToSave = ConvertToTexture2D(rawImage.mainTexture);

        if (textureToSave != null)
        {
            // Encode texture into PNG
            byte[] bytes = textureToSave.EncodeToPNG();

#if UNITY_EDITOR
            // Create a file path in the Assets folder
            count++;
            string filePath = Path.Combine("Assets", "SavedTextures", "Photo_" + count + ".png");
            // Ensure directory exists
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(filePath, bytes);
            // Refresh the AssetDatabase to show the new file in the Editor
            AssetDatabase.Refresh();
            Debug.Log("Saved Texture to " + filePath);
#else
                Debug.LogError("Texture saving is only supported in the Unity Editor.");
#endif
            return filePath;
        }
        else
        {
            Debug.LogError("No texture found to save!");
            return null;
        }
    }

    public void TakeAPhoto()
    {
        string path = TakeScreenshotGetString();
        emotionDetect.UploadImage(path);
        emotionDetect.Response += ImageEmotion;
    }

    public void ImageEmotion(string emotion)
    {
        emotionText.text = emotion;
    }

    public void RegisterFace()
    {
        Texture2D textureToSave = ConvertToTexture2D(rawImage.mainTexture);
        recognitionClient.RegisterFace(textureToSave,inputField.text);
        SendImageToServer();
    }

    public void FilterOn()
    {
        ColorSystem.Instance.ChangeColor(mainColor);
        SendImageToServer();
    }

    public Texture2D TextureToSave()
    {
        return ConvertToTexture2D(rawImage.mainTexture);
    }

    public void SendImageToServer()
    {
        Texture2D textureToSave = ConvertToTexture2D(rawImage.mainTexture);
        recognitionClient.SendImageToServer(textureToSave);
        recognitionClient.Response += ImageInfo;
    }

    public void ImageInfo(string json)
    {
        Response response = JsonUtility.FromJson<Response>(json);
        if (response.names != null)
            nameText.text = "Name : " + response.names[0];
        else
            nameText.text = "Name : " + "Unknown";
    }

    public Texture2D ConvertToTexture2D(Texture sourceTexture)
    {
        if (sourceTexture == null)
        {
            Debug.LogError("Source texture is null");
            return null;
        }

        // Create a temporary RenderTexture of the same size as the source texture
        RenderTexture tempRT = RenderTexture.GetTemporary(
                                sourceTexture.width,
                                sourceTexture.height,
                                0,
                                RenderTextureFormat.Default,
                                RenderTextureReadWrite.Linear);

        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(sourceTexture, tempRT);

        // Backup the currently set RenderTexture
        RenderTexture previous = RenderTexture.active;

        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tempRT;

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D newTexture = new Texture2D(sourceTexture.width, sourceTexture.height);
        newTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        newTexture.Apply();

        // Reset the active RenderTexture
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tempRT);

        return newTexture;
    }
}

[System.Serializable]
public class Response
{
    public List<string> names;
}