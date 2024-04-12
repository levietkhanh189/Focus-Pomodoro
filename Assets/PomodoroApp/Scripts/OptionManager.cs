using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OptionManager : MonoBehaviour
{
    public List<OptionButton> optionButtons = new List<OptionButton>();

    public void Init(List<Action> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            optionButtons[i].Init(actions[i], ButtonCallback);
        }
        ButtonCallback(optionButtons[0]);
    }

    public void ButtonCallback(OptionButton option)
    {
        foreach (var item in optionButtons)
            item.ChooseOption(item == option);
    }
}
