using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class OptionButton : MonoBehaviour
{
    public Button button;
    public Image image;
    public Color activeColor;

    public void Init(Action action, Action<OptionButton> callback)
    {
        button.onClick.AddListener(() =>
        {
            if(image.color == Color.clear)
            {
                action?.Invoke();
                callback?.Invoke(this);
            }
        });
    }

    public void ChooseOption(bool value)
    {
        image.color = value ? activeColor : Color.clear;
    }
}
