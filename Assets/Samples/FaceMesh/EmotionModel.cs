using UnityEngine;
using TensorFlowLite;

public class EmotionModel : BaseVisionTask
{
    // Khai báo kích thước đầu vào của mô hình
    private const int INPUT_WIDTH = 48;
    private const int INPUT_HEIGHT = 48;
    private const int INPUT_CHANNELS = 1;  // Chỉ sử dụng grayscale

    // Mảng để chứa dữ liệu tensor đầu vào và đầu ra
    private float[,,,] inputs = new float[1, INPUT_HEIGHT, INPUT_WIDTH, INPUT_CHANNELS];
    private float[,] outputs = new float[1, 7]; // 7 cảm xúc

    // Khởi tạo mô hình với đường dẫn file
    public EmotionModel(string modelPath)
    {
        var options = new InterpreterOptions();
        options.AddGpuDelegate(); // Sử dụng GPU nếu có thể
        Load(FileUtil.LoadFile(modelPath), options); // Tải file model
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    protected override void PreProcess(Texture texture)
    {
        // Biến đổi texture đầu vào thành tensor
        TextureToTensor(texture, ref inputs);
        interpreter.SetInputTensorData(0, inputs);
    }

    protected override void PostProcess()
    {
        // Lấy dữ liệu từ tensor đầu ra
        interpreter.GetOutputTensorData(0, outputs);
    }

    // Phương thức để lấy kết quả cảm xúc
    public float[] GetEmotionProbabilities()
    {
        return outputs.Clone() as float[];
    }

    // Phương thức chuyển đổi texture đến tensor
    void TextureToTensor(Texture texture, ref float[,,,] inputs)
    {
        var tempTexture = RenderTexture.GetTemporary(INPUT_WIDTH, INPUT_HEIGHT, 0);
        Graphics.Blit(texture, tempTexture);
        var activeRT = RenderTexture.active;
        RenderTexture.active = tempTexture;

        var tex2D = new Texture2D(INPUT_WIDTH, INPUT_HEIGHT, TextureFormat.R8, false);
        tex2D.ReadPixels(new Rect(0, 0, INPUT_WIDTH, INPUT_HEIGHT), 0, 0);
        tex2D.Apply();

        for (int y = 0; y < tex2D.height; y++)
        {
            for (int x = 0; x < tex2D.width; x++)
            {
                Color color = tex2D.GetPixel(x, y);
                inputs[0, y, x, 0] = color.grayscale / 255.0f;  // Chuẩn hóa dữ liệu
            }
        }

        Object.Destroy(tex2D);
        RenderTexture.ReleaseTemporary(tempTexture);
        RenderTexture.active = activeRT;
    }
}
