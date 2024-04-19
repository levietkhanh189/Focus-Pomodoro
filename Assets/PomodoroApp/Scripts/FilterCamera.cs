using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FilterCamera : MonoBehaviour
{
    public Color mainColor;
    private static FilterCamera instance;

    public Camera myCamera;
    private bool takeScreenshotOnNextFrame;

    private void Awake()
    {
        instance = this;
    }

    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);
            renderResult.Apply();

            byte[] byteArray = renderResult.EncodeToPNG();
            string filePath = Path.Combine(Application.persistentDataPath, "screenshot.png");
            File.WriteAllBytes(filePath, byteArray);
            Debug.Log($"Saved screenshot to {filePath}");

            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }
    }

    public static void TakeScreenshot(int width, int height)
    {
        instance.myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        instance.takeScreenshotOnNextFrame = true;
    }

    public void TakeAPhoto()
    {
        int screenWidth = Screen.width;
        TakeScreenshot(screenWidth, screenWidth);
    }

    public void FilterOn()
    {
        ColorSystem.Instance.ChangeColor(mainColor);
    }
}
