using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class EmotionDetectSample : MonoBehaviour
{
    public static EmotionDetectSample Instance; 
    public string baseUrl = "http://localhost:8000/predict/emotion"; // Change to your actual server URL

    public Action<string> Response;

    private void Awake()
    {
        Instance = this;
    }

    [Sirenix.OdinInspector.Button]
    public void UploadImage(string imagePath)
    {
        StartCoroutine(UploadImageCoroutine(imagePath));
    }

    IEnumerator UploadImageCoroutine(string imagePath)
    {
        byte[] imageData = File.ReadAllBytes(imagePath);
        UnityWebRequest www = UnityWebRequest.Post(baseUrl, CreateForm(imageData, Path.GetFileName(imagePath)));
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text);
            Emotion emotion = JsonUtility.FromJson<Emotion>(www.downloadHandler.text);
            Response?.Invoke(emotion.emotion);
            Response = null;
        }
    }

    // Helper method to create form for the request
    private WWWForm CreateForm(byte[] imageData, string fileName)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, fileName, "image/png"); // Change "image/png" based on actual image type
        return form;
    }

}
[System.Serializable]
public class Emotion
{
    public string emotion;
    public int h;
    public int w;
    public int x;
    public int y;
}
