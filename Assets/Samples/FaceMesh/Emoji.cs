using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlowLite;
public abstract class Emoji : MonoBehaviour
{
    RectTransform canvasRectTransform;
    public void Init(RectTransform canvasRectTransform)
    {
        this.canvasRectTransform = canvasRectTransform;
    }
    public RectTransform noseImage;
    public virtual void Input(FaceDetect.Result result)
    {
        //noseImage.anchoredPosition = new Vector2(noseKeyPoint.x * canvasRectTransform.rect.width, (1 - noseKeyPoint.y) * canvasRectTransform.rect.height);
    }
}
