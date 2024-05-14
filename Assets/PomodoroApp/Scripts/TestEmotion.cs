using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlowLite;
using TextureSource;

public class TestEmotion : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    private string emotionModelFile = "coco_ssd_mobilenet_quant.tflite";
    private Interpreter interpreter;

}
