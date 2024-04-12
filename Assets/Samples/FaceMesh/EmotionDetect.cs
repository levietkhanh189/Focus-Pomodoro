using System;
using System.Buffers;
using UnityEngine;
using DataType = TensorFlowLite.Interpreter.DataType;


namespace TensorFlowLite
{
    public class EmotionDetect : BaseVisionTask
    {
        public Matrix4x4 InputTransformMatrix { get; private set; } = Matrix4x4.identity;
        //private readonly float[] outputs = new float[896];
        private static readonly string[] emotions = { "angry", "disgust", "fear", "happy", "neutral", "sad", "surprise" };
        public const int KEYPOINT_COUNT = 468;
        private readonly float[,] output0 = new float[KEYPOINT_COUNT, 3]; // keypoint
        private readonly float[] output1 = new float[7]; // flag

        public EmotionDetect(string modelPath)
        {
            var interpreterOptions = new InterpreterOptions();
            interpreterOptions.AddGpuDelegate();
            Load(FileUtil.LoadFile(modelPath), interpreterOptions);
            var options = new TextureToNativeTensor.Options
            {
                compute = null, // Sử dụng Compute Shader mặc định nếu không có custom
                kernel = 0, // Kernel ID trong Compute Shader
                width = 48, // Chiều rộng của tensor đầu ra
                height = 48, // Chiều cao của tensor đầu ra
                channels = 1, // Số kênh (1 vì ảnh đơn sắc)
                inputType = DataType.Float32 // Kiểu dữ liệu float32
            };

            // Tạo một instance của TextureToNativeTensor
            textureToTensor = TextureToNativeTensor.Create(options);
        }

        protected override void PreProcess(float[] modelInput)
        {
            //InputTransformMatrix = textureToTensor.GetAspectScaledMatrix(texture, AspectMode);
            //var input = textureToTensor.Transform(texture, InputTransformMatrix);
            interpreter.SetInputTensorData(0, modelInput);
        }


        protected override void PostProcess()
        {

            interpreter.GetOutputTensorData(0, output1);
            //interpreter.GetOutputTensorData(1, output1);
        }

        public string GetResult()
        {
            int maxIndex = 0; // Chỉ số của giá trị lớn nhất trong mảng
            float maxValue = output1[0]; // Khởi tạo giá trị tối đa bằng phần tử đầu tiên của mảng

            // Duyệt qua mảng để tìm giá trị lớn nhất
            for (int i = 1; i < output1.Length; i++)
            {
                if (output1[i] > maxValue)
                {
                    maxValue = output1[i]; // Cập nhật giá trị lớn nhất
                    maxIndex = i; // Lưu lại chỉ số của giá trị lớn nhất
                }
            }

            // Xuất ra cảm xúc với độ tin cậy cao nhất được phát hiện
            Debug.Log($"Detected Emotion: {emotions[maxIndex]} with confidence {maxValue}");
            return emotions[maxIndex];
        }

    }
}
