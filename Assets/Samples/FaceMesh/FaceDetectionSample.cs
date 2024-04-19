using System.Collections.Generic;
using TensorFlowLite;
using TextureSource;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
/// <summary>
/// BlazeFace from MediaPile
/// https://github.com/google/mediapipe
/// https://viz.mediapipe.dev/demo/face_detection
/// </summary>
[RequireComponent(typeof(VirtualTextureSource))]
public class FaceDetectionSample : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    private string faceModelFile = "coco_ssd_mobilenet_quant.tflite";

    [SerializeField]
    private RawImage inputPreview = null;

    private FaceDetect faceDetect;
    public List<FaceDetect.Result> results;
    private PrimitiveDraw draw;
    private readonly Vector3[] rtCorners = new Vector3[4];
    private Material previewMaterial;
    public Text debugText;
    public Image noseImage;
    private Camera camera;

    public Action<FaceDetect.Result> OnResult;
    private void Start()
    {
        camera = Camera.main;
        faceDetect = new FaceDetect(faceModelFile);
        draw = new PrimitiveDraw(camera, gameObject.layer);
        previewMaterial = new Material(Shader.Find("Hidden/TFLite/InputMatrixPreview"));
        inputPreview.material = previewMaterial;

        if (TryGetComponent(out VirtualTextureSource source))
        {
            source.OnTexture.AddListener(OnTextureUpdate);
        }
    }

    private void OnDestroy()
    {
        if (TryGetComponent(out VirtualTextureSource source))
        {
            source.OnTexture.RemoveListener(OnTextureUpdate);
        }
        faceDetect?.Dispose();
        draw?.Dispose();
        Destroy(previewMaterial);
    }

    private void Update()
    {
        DrawResults(results);
    }

    private void OnTextureUpdate(Texture texture)
    {
        faceDetect.Run(texture);

        inputPreview.texture = texture;
        previewMaterial.SetMatrix("_TransformMatrix", faceDetect.InputTransformMatrix);

        inputPreview.rectTransform.GetWorldCorners(rtCorners);
        results = faceDetect.GetResults();
    }
    float timeCounter;
    private void DrawResults(List<FaceDetect.Result> results)
    {
        if (results == null || results.Count == 0)
        {
            return;
        }

        float3 min = rtCorners[0];
        float3 max = rtCorners[2];

        draw.color = Color.clear;

        if(results.Count == 0)
        {
            ModeManager.Instance.WarningPomodoroMode();
            return;
        }

        if(results.Count > 0)
        {
            Rect rect = MathTF.Lerp((Vector3)min, (Vector3)max, results[0].rect.FlipY());
            draw.Rect(rect, 0.05f, -0.1f);
            foreach (Vector2 p in results[0].keypoints)
            {
                draw.Point(math.lerp(min, max, new float3(p.x, 1f - p.y, 0)), -0.1f);
            }
            var noseKeyPoint = results[0].keypoints[(int)FaceDetect.KeyPoint.Nose];
            OnResult?.Invoke(results[0]);
            FaceDetect.LookDirection direction = results[0].GetLookDirection(0.1f);

            if (direction != FaceDetect.LookDirection.Forward)
            {
                timeCounter += 0.02f;
                if (timeCounter > 10f)
                {
                    ModeManager.Instance.WarningPomodoroMode();
                }
                debugText.text = "# You look " + direction.ToString();
            }
            else
            {
                timeCounter = 0f;
                debugText.text = "# You are focus ";
            }

        }

        //foreach (var result in results)
        //{
        //    Rect rect = MathTF.Lerp((Vector3)min, (Vector3)max, result.rect.FlipY());
        //    draw.Rect(rect, 0.05f, -0.1f);
        //    foreach (Vector2 p in result.keypoints)
        //    {
        //        draw.Point(math.lerp(min, max, new float3(p.x, 1f - p.y, 0)), -0.1f);
        //    }
        //    var noseKeyPoint = results[0].keypoints[(int)FaceDetect.KeyPoint.Nose];
        //    OnResult?.Invoke(results[0]);
        //    //   UpdateNosePosition(noseKeyPoint);
        //    debugText.text = "# You look " + result.GetLookDirection(0.085f).ToString();
        //}
        draw.Apply();
    }

    public void UpdateNosePosition(Vector2 noseKeyPoint)
    {
        Vector2 screenPosition = new Vector2(noseKeyPoint.x, noseKeyPoint.y);

        noseImage.rectTransform.position = screenPosition;

    }

}
