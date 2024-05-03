using System;
using System.Buffers;
using UnityEngine;
using DataType = TensorFlowLite.Interpreter.DataType;

namespace TensorFlowLite
{
    // Lớp EmotionDetect kế thừa từ BaseVisionTask để thực hiện nhận dạng cảm xúc
    public class EmotionDetect : BaseVisionTask
    {
        // Mảng chứa các nhãn cảm xúc
        private static readonly string[] emotions = { "angry", "disgust", "fear", "happy", "neutral", "sad", "surprise" };

        // Ma trận biến đổi đầu vào, khởi tạo là ma trận đơn vị
        public Matrix4x4 InputTransformMatrix { get; private set; } = Matrix4x4.identity;

        // Mảng lưu trữ đầu ra cho nhận dạng cảm xúc
        private readonly float[] emotionScores = new float[7];

        public EmotionDetect(string modelPath)
        {
            // Khởi tạo và tải model
            var interpreterOptions = new InterpreterOptions();
            Load(FileUtil.LoadFile(modelPath), interpreterOptions);
            // Cấu hình TextureToNativeTensor
            width = 96; // Chiều rộng của tensor đầu ra
            height = 96; // Chiều cao của tensor đầu ra
            channels = 3; // Số kênh màu RGB

            DebugModel();
        }

        // Phương thức debug để kiểm tra thông tin model
        void DebugModel()
        {
            Debug.Log("Input Details: " + interpreter.GetInputTensorInfo(0));
            Debug.Log("Output Details: " + interpreter.GetOutputTensorInfo(0));
        }

        protected override void PreProcess(Texture texture)
        {
            // Lấy ma trận biến đổi phù hợp với tỉ lệ aspect của texture
            InputTransformMatrix = textureToTensor.GetAspectScaledMatrix(texture, AspectMode.Fit);
            // Biến đổi texture sang tensor
            var inputTensor = textureToTensor.Transform(texture, InputTransformMatrix);
            // Đặt tensor vào model
            interpreter.SetInputTensorData(0, inputTensor);
        }

        protected override void PostProcess()
        {
            // Lấy dữ liệu đầu ra từ model
            interpreter.GetOutputTensorData(0, emotionScores);
        }

        public string GetResult()
        {
            int maxIndex = 0;
            float maxValue = emotionScores[0];

            // Tìm chỉ số và giá trị lớn nhất trong mảng emotionScores
            for (int i = 1; i < emotionScores.Length; i++)
            {
                if (emotionScores[i] > maxValue)
                {
                    maxValue = emotionScores[i];
                    maxIndex = i;
                }
            }

            // Trả về cảm xúc với độ tin cậy cao nhất
            Debug.Log($"Detected Emotion: {emotions[maxIndex]} with confidence {maxValue}");
            return emotions[maxIndex];
        }
    }
}
