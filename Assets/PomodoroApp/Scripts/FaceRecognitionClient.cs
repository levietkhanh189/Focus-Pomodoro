using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FaceRecognitionClient : MonoBehaviour
{
    public string url;

    [Sirenix.OdinInspector.Button]
    public void SendImageToServer(Texture2D image)
    {
        StartCoroutine(_SendImageToServer(image));
    }

    IEnumerator _SendImageToServer(Texture2D image)
    {
        byte[] imageData = image.EncodeToPNG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "face.png", "image/png");

        UnityWebRequest www = UnityWebRequest.Post(url+"/recognize", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text);
            // Process the response to show recognized names in Unity
            string jsonResponse = www.downloadHandler.text;
            // You can parse the jsonResponse to get the names
        }
    }

    [Sirenix.OdinInspector.Button]
    public void RegisterFace(Texture2D image, string name)
    {
        StartCoroutine(_RegisterFace(image, name));
    }

    IEnumerator _RegisterFace(Texture2D image, string name)
    {
        byte[] imageData = image.EncodeToPNG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "face.png", "image/png");
        form.AddField("name", name);

        UnityWebRequest www = UnityWebRequest.Post(url + "/register", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Response: " + www.downloadHandler.text);
            // Process the response to show registration status in Unity
        }
    }
}
