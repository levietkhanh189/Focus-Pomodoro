using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorItem : MonoBehaviour
{
    private Text text;
    private Image image;

    void Start()
    {
        text = GetComponent<Text>();
        image = GetComponent<Image>();
        ColorSystem.Instance.Register(ColorChange);
    }

    public void ColorChange(Color color)
    {
        if (text != null)
            text.color = color;
        if (image != null)
            image.color = color;
    }
}
