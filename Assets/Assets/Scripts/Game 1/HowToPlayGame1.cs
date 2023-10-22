using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HowToPlayGame1 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    private Button Button;
    private ButtonState CurrentState;
    [SerializeField]
    private TMP_Text InformationText;
    [SerializeField]
    private TMP_Dropdown StateDropdown;

    private Color white = new Color(1f, 1f, 1f, 1f);
    private Color black = new Color(0f, 0f, 0f, 1f);
    private Color lightblue = new Color(0f, 176f / 255f, 1f, 1f);
    private Color darkred = new Color(174f / 255f, 2f / 255f, 22f / 255f, 1f);

    private int TimeClickTime;
    private float HoldDuration;
    private int MultipleClickAmount;

    private bool IsHolding;
    private bool InButtonArea;
    private float ButtonPressedTime;



    private void Start()
    {
        HoldDuration = 1f;
        TimeClickTime = 5;
        MultipleClickAmount = 5;
        OnChangeState();
    }

    public void OnChangeState()
    {
        StopCoroutine(nameof(TimeClickTimer));

        Button.GetComponentInChildren<TMP_Text>().text = "";
        ButtonPressedTime = 0f;
        IsHolding = false;
        MultipleClickAmount = 5;
        IsHolding = false;


        switch (StateDropdown.value)
        {
            case 0:
                CurrentState = ButtonState.Click;
                Button.colors = SetNewColorBlock(black);
                InformationText.text = "Tippe den Knopf einmal\nan um Punkte zu bekommen";
                break;
            case 1:
                CurrentState = ButtonState.HoldClick;
                Button.colors = SetNewColorBlock(lightblue);
                InformationText.text = "Halte den Knopf mindestens\n1 Sekunde lang ";
                break;
            case 2:
                CurrentState = ButtonState.TimeClick;
                Button.colors = SetNewColorBlock(white);
                InformationText.text = "Tippe den Knopf\ninnerhalb von 5\nSekunden an";
                StartCoroutine(nameof(TimeClickTimer));
                break;
            case 3:
                CurrentState = ButtonState.MultipleClick;
                Button.colors = SetNewColorBlock(white);
                InformationText.text = "Tippe den Knopf\n5 mal hintereinander\nan";
                Button.GetComponentInChildren<TMP_Text>().text = NumberToRoman(5);
                break;
            case 4:
                CurrentState = ButtonState.TrapClick;
                Button.colors = SetNewColorBlock(darkred);
                InformationText.text = "Diesen Knopd darfst\r\ndu nicht anklicken";
                break;
            default:
                Debug.LogError("Add new State in the Statehandler!!!");
                break;
        }
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case ButtonState.HoldClick:

                if (IsHolding && InButtonArea && ((Time.time - ButtonPressedTime) >= HoldDuration))
                {
                    ButtonPressedTime = 0f;
                    Button.colors = SetNewColorBlock(white);
                    CurrentState = ButtonState.None;
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
        switch (CurrentState)
        {
            case ButtonState.HoldClick:
                InButtonArea = false;
                break;
            case ButtonState.None:
                Button.colors = SetNewColorBlock(white);
                CurrentState = ButtonState.DontClick;
                break;
        }
    }

    private ColorBlock SetNewColorBlock(Color _value)
    {
        ColorBlock result = new ColorBlock();

        result.normalColor = result.selectedColor = result.pressedColor = result.highlightedColor = _value;
        result.colorMultiplier = 1f;
        result.fadeDuration = 0.1f;

        return result;
    }

    private string NumberToRoman(int _num)
    {
        if (_num <= 0 || _num >= 4000)
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

    private IEnumerator TimeClickTimer()
    {
        for (int i = TimeClickTime; i >= 0; --i)
        {
            Button.GetComponentInChildren<TMP_Text>().text = i.ToString();
            yield return new WaitForSeconds(1f);
            if (i <= 0)
            {
                InformationText.text = "Du must den Button schon innerhalb von 5 Sekunden anklicken!!!";
                Button.GetComponentInChildren<TMP_Text>().text = "";
            }
        }
    }

    public void ClickButton()
    {
        switch(CurrentState)
        {
            case ButtonState.DontClick:
                InformationText.text = "Jetzt darfst du den Knopf nicht mehr anklicken!!!";
                break;
            case ButtonState.Click:
                Button.colors = SetNewColorBlock(white);
                CurrentState = ButtonState.DontClick;
                break;
            case ButtonState.HoldClick:
                //OnPointerDown
                break;
            case ButtonState.TimeClick:
                StopCoroutine(nameof(TimeClickTimer));
                Button.GetComponentInChildren<TMP_Text>().text = "";
                CurrentState = ButtonState.DontClick;
                break;
            case ButtonState.MultipleClick:
                --MultipleClickAmount;
                if (MultipleClickAmount > 0)
                {
                    Button.GetComponentInChildren<TMP_Text>().text = NumberToRoman(MultipleClickAmount);
                }
                else
                {
                    Button.GetComponentInChildren<TMP_Text>().text = "";
                    CurrentState = ButtonState.DontClick;
                }
                break;
            case ButtonState.TrapClick:
                InformationText.text = "Du darfst den Knopf nicht mehr anklicken!!!";
                break;
        }
    }

}
