using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PomodoroTimer : MonoBehaviour
{
    public static Person person;
    public FaceRecognitionClient recognitionClient;
    public Text buttonText;
    public Text timerText; // Gán trong Inspector
    public OptionManager optionManager;
    private float timeRemaining;
    private bool isTimerRunning = false;

    // Thời gian cho mỗi chế độ, đơn vị là giây
    private const float pomodoroTime = 25;//* 60;
    private const float shortBreakTime = 5;//* 60;
    private const float longBreakTime = 15; // * 60;

    private IEnumerator timerCoroutine;
    private float durationTime = 25;// * 60;
    private string mode = "Pomodoro";
    
    public Color[] timerColors;
    private void Awake()
    {
        List<Action> actions = new List<Action>();
        actions.Add(OptionPomodoro);
        actions.Add(OptionShortBreak);
        actions.Add(OptionLongBreak);
        optionManager.Init(actions);
    }

    private void Start()
    {
        OptionPomodoro();
    }

    // Hàm khởi động đếm ngược cho Pomodoro
    public void OptionPomodoro()
    {
        mode = "Pomodoro";
        OptionSet(pomodoroTime);
        ColorSystem.Instance.ChangeColor(timerColors[0]);
    }

    // Hàm khởi động đếm ngược cho Short Break
    public void OptionShortBreak()
    {
        mode = "ShortBreak";
        OptionSet(shortBreakTime);
        ColorSystem.Instance.ChangeColor(timerColors[1]);
    }

    // Hàm khởi động đếm ngược cho Long Break
    public void OptionLongBreak()
    {
        mode = "LongBreak";
        OptionSet(longBreakTime);
        ColorSystem.Instance.ChangeColor(timerColors[2]);
    }

    public void OptionSet(float duration)
    {
        if (isTimerRunning)
        {
            StopCoroutine(timerCoroutine);
            isTimerRunning = false;
        }
        buttonText.text = "START";
        durationTime = duration;
        UpdateTimerDisplay(durationTime);
    }

    public void SendImageToServer()
    {
        Texture2D textureToSave = FilterCamera.instance.TextureToSave();
        Sprite newSprite = Sprite.Create(textureToSave, new Rect(0.0f, 0.0f, textureToSave.width, textureToSave.height), new Vector2(0.5f, 0.5f), 100.0f);
        person.icon = newSprite;
        recognitionClient.SendImageToServer(textureToSave);
        recognitionClient.Response += ImageInfo;
    }

    public void ImageInfo(string json)
    {
        Response response = JsonUtility.FromJson<Response>(json);
        if (response.names != null)
            person.name = response.names[0];
        else
            person.name = "Unknown";
    }

    public void StartTimer()
    {
        if (isTimerRunning)
        {
            person.pause++;
            buttonText.text = "START";
            durationTime = timeRemaining;
            StopCoroutine(timerCoroutine);
            isTimerRunning = false;
        }
        else
        {
            StartTimer(durationTime);
            buttonText.text = "PAUSE";
        }
    }

    private void StartTimer(float duration)
    {
        if (isTimerRunning)
        {
            StopCoroutine(timerCoroutine);
        }
        person = new Person();
        person.focus = 100f;
        person.emotion = "Happy";
        SendImageToServer();
        timerCoroutine = Countdown(duration);
        StartCoroutine(timerCoroutine);
    }

    public void MinusFocus()
    {
        person.focus -= Time.deltaTime;
    }

    public void WarningPause()
    {
        if (isTimerRunning && mode == "Pomodoro")
        {
            person.pause++;
            DTNSoundManagement.instance.Play("Warning");
            buttonText.text = "START";
            durationTime = timeRemaining;
            StopCoroutine(timerCoroutine);
            isTimerRunning = false;
        }
    }

    public void NextTimer()
    {
        if (mode == "Pomodoro")
        {
            optionManager.ChooseOption(1);
            OptionShortBreak();
        }
        else
        {
            optionManager.ChooseOption(0);
            OptionPomodoro();
        }
    }

    private IEnumerator Countdown(float duration)
    {
        isTimerRunning = true;
        timeRemaining = duration;

        while (timeRemaining > 0)
        {
            UpdateTimerDisplay(timeRemaining);

            // Đợi trong 1 giây
            yield return new WaitForSeconds(1);

            timeRemaining--;
        }

        // Hoàn thành đếm ngược
        ReportPopup.Instance.AddPerson(person);
        buttonText.text = "START";
        timerText.text = "Time's up!";
        if(mode == "Pomodoro")
        {
            OptionShortBreak();
        }
        else
        {
            OptionPomodoro();
        }
        isTimerRunning = false;
    }

    // Cập nhật UI hiển thị thời gian
    private void UpdateTimerDisplay(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}