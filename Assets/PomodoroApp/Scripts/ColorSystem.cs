using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorSystem : MonoBehaviour
{
    public static ColorSystem Instance;

    public Action<Color> colorItems;

    private void Awake()
    {
        Instance = this;
    }

    public void Register(Action<Color> colorItem)
    {
        colorItems += colorItem;
    }

    public void ChangeColor(Color color)
    {
        colorItems?.Invoke(color);
    }
}
