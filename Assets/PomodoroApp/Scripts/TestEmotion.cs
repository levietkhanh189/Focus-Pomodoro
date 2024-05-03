using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlowLite;
using TextureSource;

public class TestEmotion : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    private string emotionModelFile = "coco_ssd_mobilenet_quant.tflite";

    public Texture texture;
    public EmotionDetect emotionDetect;
    public VirtualTextureSource source;

    public void Start()
    {
        emotionDetect = new EmotionDetect(emotionModelFile);
        //source.OnTexture.AddListener(OnTextureUpdate);
        emotionDetect.Run(texture);
        Debug.Log(emotionDetect.GetResult());

    }

    private void OnTextureUpdate(Texture texture)
    {
        emotionDetect.Run(texture);

        Debug.Log(emotionDetect.GetResult());
    }

}
