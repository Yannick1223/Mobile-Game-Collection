using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game1Manager : MonoBehaviour
{
    public AudioClip ButtonClickClip;
    public AudioClip ButtonWrongClickClip;

    public GameObject SettingsPanel;
    public GameObject HelpPanel;
    public GameObject GamePanel;
    public GameObject StartButton;

    private BoardManager Board;

    [HideInInspector]
    public bool InSettings;
    [HideInInspector]
    public bool InHowToPlay;

    [HideInInspector]
    public bool IsPlaying;
    [SerializeField]
    public Toggle DebugToggle;
    [HideInInspector]
    public bool ShowDebug;

    private void Awake()
    {
        Board = GetComponent<BoardManager>();

        Application.targetFrameRate = 60;
        InSettings = false;
        InHowToPlay = false;
        IsPlaying = false;
    }

    public void Back()
    {
        // Work in Progress. Update later to Main Menu Scene.
        //SceneManager.LoadScene(0);
        Application.Quit();
    }

    public void OnDebugChange()
    {
        ShowDebug = DebugToggle.isOn;
    }

    public void Settings()
    {
        InSettings = !InSettings;
        SettingsPanel.SetActive(InSettings);
    }

    public void HowToPlaySettings()
    {
        InHowToPlay = !InHowToPlay;
        HelpPanel.SetActive(InHowToPlay);
    }

    public void StartGame()
    {
        IsPlaying = true;
        StartButton.SetActive(false);
        Board.StartGame();
    }
}