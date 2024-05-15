using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
public class EmotionDetectSample : MonoBehaviour
{
    public static EmotionDetectSample Instance; 
    public string baseUrl = "http://localhost:8000/predict/emotion"; // Change to your actual server URL

    public delegate void ResponseDelegate(string response);
    public event ResponseDelegate ResponseReceived;

    private void Awake()
    {

        Instance = this;
    }

    [Sirenix.OdinInspector.Button]
    public void UploadTexture(Texture2D texture)
    {
        StartCoroutine(SendImageToServer(texture));
    }


    public IEnumerator SendImageToServer(Texture2D image)
    {
        if (image == null)
        {
            Debug.LogError("Error: The image is null.");
            yield break; // Exit the coroutine if the image is null
        }

        byte[] imageData = image.EncodeToJPG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "uploaded_image", "image/jpeg");

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
                ResponseReceived?.Invoke("Error: " + www.error); // Notify listeners about the error
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                ResponseReceived?.Invoke(www.downloadHandler.text); // Trigger the response received event
            }
        }
    }

}
