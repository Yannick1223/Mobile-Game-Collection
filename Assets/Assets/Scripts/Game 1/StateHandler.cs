using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//if you Change the State from HoldClick to DontClick --> Game Over because after Hold its a Click => wait that the player dont hold it anymore (None) and then change back to DontClick
public enum ButtonState
{
    DontClick, Click, TimeClick, HoldClick, MultipleClick, TrapClick, None,
}

public class StateHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private Game1Manager GameManager;
    private GameObject MainCamera;
    private Button Btn;
    private TextMeshProUGUI BtnText;
    private ButtonState CurrentState;

    private int TimeClickTime; // Time how long you have until you have to press the button
    private int MaxMultipleAmount; // Number of times you have to press the button
    private int TrapDuration; // Time how long the trap stays
    private float HoldDuration; //Time how long to press the button

    private int MultipleClickAmount;
    private bool IsHolding;
    private bool InButtonArea;
    private float ButtonPressedTime;

    

    private void Awake()
    {
        Btn = GetComponent<Button>();
        GameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Game1Manager>();
        MainCamera = Camera.main.gameObject;
        BtnText = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        Btn.onClick.AddListener(() => ButtonClick());

        //Default Settings
        TimeClickTime = 5;
        MaxMultipleAmount = 5;
        TrapDuration = 1;
        HoldDuration = 1f;
    }

    public void ChangeSettings(int _TimeClickTime, int _MaxMultipleAmount, int _TrapDuration, float _HoldDuration)
    {
        TimeClickTime = _TimeClickTime;
        MaxMultipleAmount = _MaxMultipleAmount;
        TrapDuration = _TrapDuration;
        HoldDuration = _HoldDuration;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case ButtonState.HoldClick:

                if (IsHolding && InButtonArea && ((Time.time - ButtonPressedTime) >= HoldDuration))
                {
                    ButtonPressedTime = 0f;
                    GameManager.GetComponent<HighscoreGame1Manager>().AddScore(1);
                    MainCamera.GetComponent<AudioSource>().clip = GameManager.ButtonClickClip;
                    MainCamera.GetComponent<AudioSource>().Play();
                    SetButtonState(ButtonState.None);
                }
                break;
        }
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        switch (CurrentState)
        {
            case ButtonState.HoldClick:
                ButtonPressedTime = Time.time;
                InButtonArea = true;
                IsHolding = true;
                break;
        }
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        switch (CurrentState)
        {
            case ButtonState.HoldClick:
                IsHolding = false;
                break;
        }
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        switch(CurrentState)
        {
            case ButtonState.HoldClick:
                InButtonArea = false;
                break;
            case ButtonState.None:
                SetButtonState(ButtonState.DontClick);
                break;
        }
    }

    public ButtonState GetCurrentState()
    {
        return CurrentState;
    }

    public void ButtonClick()
    {
        if (GameManager.IsPlaying)
        {
            switch (CurrentState)
            {
                case ButtonState.DontClick:
                    MainCamera.GetComponent<AudioSource>().clip = null;
                    GameManager.GetComponent<BoardManager>().GameOver();
                    break;
                case ButtonState.None:
                    MainCamera.GetComponent<AudioSource>().clip = null;
                    SetButtonState(ButtonState.DontClick);
                    break;
                case ButtonState.Click:
                    MainCamera.GetComponent<AudioSource>().clip = GameManager.ButtonClickClip;
                    GameManager.GetComponent<HighscoreGame1Manager>().AddScore(1);
                    SetButtonState(ButtonState.DontClick);
                    break;
                case ButtonState.HoldClick:
                    //See OnPointerUp
                    MainCamera.GetComponent<AudioSource>().clip = GameManager.ButtonClickClip;
                    break;
                case ButtonState.TimeClick:
                    StopCoroutine(nameof(TimeClickTimer));
                    MainCamera.GetComponent<AudioSource>().clip = GameManager.ButtonClickClip;
                    GameManager.GetComponent<HighscoreGame1Manager>().AddScore(1);
                    SetButtonState(ButtonState.DontClick);
                    break;
                case ButtonState.MultipleClick:
                    MainCamera.GetComponent<AudioSource>().clip = GameManager.ButtonClickClip;
                    --MultipleClickAmount;
                    if (MultipleClickAmount > 0)
                    {
                        BtnText.text = NumberToRoman(MultipleClickAmount);
                    }
                    else
                    {
                        GameManager.GetComponent<HighscoreGame1Manager>().AddScore(1);
                        SetButtonState(ButtonState.DontClick);
                    }
                    break;
                case ButtonState.TrapClick:
                    MainCamera.GetComponent<AudioSource>().clip = null;
                    GameManager.GetComponent<BoardManager>().GameOver();
                    SetButtonState(ButtonState.DontClick);
                    break;
            }
            MainCamera.GetComponent<AudioSource>().Play();
        }
    }

    public void SetButtonState(ButtonState _value)
    {
        CurrentState = _value;

        ColorBlock newColor = Btn.colors;

        Color white = new Color(1f, 1f, 1f, 1f);
        Color black = new Color(0f, 0f, 0f, 1f);
        Color lightblue = new Color(0f, 176f/255f, 1f, 1f); 
        Color darkred = new Color(174f/255f, 2f/255f, 22f/255f, 1f);

        BtnText.text = "";
        ButtonPressedTime = 0f;
        IsHolding = false;
        MultipleClickAmount = MaxMultipleAmount;
        IsHolding = false;

        StopCoroutine(nameof(TimeClickTimer));
        StopCoroutine(nameof(StartTrapTimer));

        switch (CurrentState)
        {
            case ButtonState.None:
                newColor = SetNewColorBlock(white);
                break;
            case ButtonState.DontClick:
                newColor = SetNewColorBlock(white);

                GameManager.GetComponent<BoardManager>().NotInState.Add(gameObject);
                break;
            case ButtonState.Click:
                newColor = SetNewColorBlock(black);
                break;
            case ButtonState.HoldClick:
                newColor = SetNewColorBlock(lightblue);
                break;
            case ButtonState.TimeClick:
                newColor = SetNewColorBlock(white);

                StartCoroutine(nameof(TimeClickTimer));
                break;
            case ButtonState.MultipleClick:
                newColor = SetNewColorBlock(white);

                BtnText.text = NumberToRoman(MultipleClickAmount);
                break;
            case ButtonState.TrapClick:
                newColor = SetNewColorBlock(darkred);

                StartCoroutine(nameof(StartTrapTimer), TrapDuration);
                break;
        }
        Btn.colors = newColor;
    }

    private ColorBlock SetNewColorBlock(Color value)
    {
        ColorBlock result = new ColorBlock();

        result.normalColor = result.selectedColor = result.pressedColor = result.highlightedColor = value;
        result.colorMultiplier = 1f;
        result.fadeDuration = 0.1f;

        return result;
    }

    public void SetRandomButtonState(int[] _appearchance)
    {
        int state = GenerateRandomState(_appearchance);
        ButtonState result;

        //follow the same order as in the Inspector (ActivEnemyTypes)
        switch (state)
        {
            case 0:
                result = ButtonState.Click;
                break;
            case 1:
                result = ButtonState.TimeClick;
                break;
            case 2:
                result = ButtonState.HoldClick;
                break;
            case 3:
                result = ButtonState.MultipleClick;
                break;
            case 4:
                result = ButtonState.TrapClick; 
                break;
            default:
                result = ButtonState.DontClick;
                Debug.LogError("Add new State in the Statehandler!!!");
                break;
        }
        SetButtonState(result);
    }

    private IEnumerator TimeClickTimer() 
    { 
        for (int i = TimeClickTime; i >= 0; --i)
        {
            BtnText.text = i.ToString();
            yield return new WaitForSeconds(1f);
            if(i <= 0)
            {
                SetButtonState(ButtonState.DontClick);
                GameManager.GetComponent<BoardManager>().GameOver();
            }
        }
    }

    private IEnumerator StartTrapTimer(int _time)
    {
        yield return new WaitForSeconds(_time);
        SetButtonState(ButtonState.DontClick);
        GameManager.GetComponent<HighscoreGame1Manager>().AddScore(1);
    }

    //Generates a number with different chances
    //Example: appearchance {20, 30, 60, 120}
    // 0--> 20/230% 1--> 30/230% ...
    private int GenerateRandomState(int[] _appearchance)
    {
        int rnd = UnityEngine.Random.Range(0, _appearchance.Sum());
        int lastNum = 0;
        int nextNum = _appearchance[0];
        int result = 0;

        for (int i = 0; i < _appearchance.Length; ++i)
        {
            if (rnd >= lastNum && rnd < nextNum)
            {
                result = i;
                break;
            }
            else
            {
                nextNum += _appearchance[i + 1];
                lastNum += _appearchance[i];
            }
        }
        return result;
    }

    private string NumberToRoman(int _num)
    {
        if(_num <= 0 || _num >= 4000)
        {
            throw new System.ArgumentOutOfRangeException("The number must be between 1 and 3999!" + _num);
        }

        int[] values = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        string[] roman = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

        StringBuilder result = new StringBuilder();

        for (int i = 0; i < values.Length; ++i)
        {
            while (_num >= values[i])
            {
                result.Append(roman[i]);
                _num -= values[i];
            }
        }

        return result.ToString();
    }
}
