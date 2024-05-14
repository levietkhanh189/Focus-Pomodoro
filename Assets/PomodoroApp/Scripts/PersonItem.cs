using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonItem : MonoBehaviour
{
    public Image image;
    public Text nameText;
    public Text focusText;
    public Text pauseText;
    public Text emotionText;

    public void Init(Person person)
    {
        image.sprite = person.icon;
        nameText.text = person.name;
        focusText.text = "Focus : " + (int)person.focus + "%";
        pauseText.text = "Pause : " + (int)person.pause;
        emotionText.text = "Emotion : " + person.emotion;
    }
}
