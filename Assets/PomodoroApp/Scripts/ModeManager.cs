using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    public static ModeManager Instance;
    public GameObject pomodoroMode;
    public PomodoroTimer pomodoroTimer;
    public Sprite pomodoroIcon;
    public GameObject cameraMode;
    public FilterCamera filterCamera;
    public Sprite cameraIcon;

    public Image iconSwitch;
    
    public bool isPomodoroMode = true;

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchMode()
    {
        iconSwitch.sprite = isPomodoroMode ? pomodoroIcon : cameraIcon;
        isPomodoroMode = !isPomodoroMode;
        pomodoroMode.SetActive(isPomodoroMode);
        cameraMode.SetActive(!isPomodoroMode);

        if (isPomodoroMode)
        {
            pomodoroTimer.optionManager.ChooseOption(0);
            pomodoroTimer.OptionPomodoro();
        }
        else
        {
            filterCamera.FilterOn();
        }
    }

    public void WarningPomodoroMode()
    {
        pomodoroTimer.WarningPause();
    }
}
