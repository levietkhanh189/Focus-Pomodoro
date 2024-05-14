using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APICaller : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GetMessageCoroutine("http://127.0.0.1:5000"));
    }

    IEnumerator GetMessageCoroutine(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);
            }
        }
    }
}
