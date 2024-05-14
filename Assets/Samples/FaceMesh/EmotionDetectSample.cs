using System.Linq;
using TensorFlowLite;
using TextureSource;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class EmotionDetectSample : MonoBehaviour
{
    [Header("Model Settings")]
    [SerializeField, FilePopup("*.tflite")]
    private string emotionModelFile = "coco_ssd_mobilenet_quant.tflite";

    public EmotionDetect emotionDetect;
    public FaceDetectionSample faceDetectionSample;
    public string emotion;
    public Texture texture;
    private void Start()
    {
        emotionDetect = new EmotionDetect(emotionModelFile);
        emotionDetect.Run(texture);
    }


    private void OnDestroy()
    {
        emotionDetect?.Dispose();
    }


    public float[] PrepareKeypointsForModel(FaceDetect.Result result)
    {
        // Tạo một mảng để chứa dữ liệu đầu vào của mô hình, giả sử mỗi keypoint có 2 giá trị (x, y)
        float[] modelInput = new float[result.keypoints.Length * 2];

        for (int i = 0; i < result.keypoints.Length; i++)
        {
            // Chuyển đổi tọa độ pixel sang chuẩn hóa từ 0 đến 1 nếu chưa chuẩn hóa
            modelInput[i * 2] = result.keypoints[i].x;
            modelInput[i * 2 + 1] = result.keypoints[i].y;
            Debug.Log(result.keypoints[i].y);
        }

        return modelInput;
    }

}
