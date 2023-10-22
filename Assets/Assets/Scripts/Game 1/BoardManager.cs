using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private GameObject GameBoard;
    [HideInInspector]
    public GameObject[] Board;
    [HideInInspector]
    public List<GameObject> NotInState;
    private GameObject MainCamera;
    private Game1Manager GameManager;

    [SerializeField]
    private GameObject ButtonPrefab;
    public Vector2 BoardSize;
    public Vector2 Edge;
    public bool ShowPos;

    [SerializeField]
    private TextMeshProUGUI DebugText;
    [SerializeField]
    private TextMeshProUGUI ErrorText;

    [SerializeField]
    private Toggle[] ActivEnemyTypes;
    [SerializeField]
    private TMP_InputField[] CustomAppearchance;
    [SerializeField]
    private TMP_InputField TimeField;
    [SerializeField]
    private TMP_Dropdown Difficult;
    [SerializeField]
    private GameObject CustomSettings;

    private int[] Appearchance;
    public int[] EasyAppearchance;
    public int[] NormalAppearchance;
    public int[] HardAppearchance;
    public int[] ImpossibleAppearchance;
    private bool Custom;

    

    [SerializeField]
    private int MaxButtons;
    private bool IsGameOver;

    //Menu
    [SerializeField]
    private GameObject Startbildschirm;
    [SerializeField]
    private GameObject Game;
    [SerializeField]
    private GameObject StartCountdown;



    private void Start()
    {
        Debug.Log(Screen.width + " " + Screen.height);
        GameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Game1Manager>();
        MainCamera = Camera.main.gameObject;

        for (int i = 0; i < EasyAppearchance.Length; ++i)
        {
            CustomAppearchance[i].text = EasyAppearchance[i].ToString();
        }
        TimeField.text = 1f.ToString();
    }

    public void OnDifficultChange() 
    {
        switch (Difficult.value)
        {
            case 0://Easy
                Custom = false;
                Appearchance = EasyAppearchance;
                break;
            case 1://Normal
                Custom = false;
                Appearchance = NormalAppearchance;
                break;
            case 2://Hard
                Custom = false;
                Appearchance = HardAppearchance;
                break;
            case 3://Imposible
                Custom = false;
                Appearchance = ImpossibleAppearchance;
                break;
            case 4://Custom
                Custom = true;
                break;
        }

        IsCustom(Custom);
    }

    private void IsCustom(bool _custom)
    {
        if (_custom)
        {
            for (int i = 0; i < CustomAppearchance.Length; ++i)
            {
                CustomAppearchance[i].gameObject.SetActive(true);
            }
            CustomSettings.SetActive(true);
        }
        else
        {
            for (int i = 0; i < CustomAppearchance.Length; ++i)
            {
                CustomAppearchance[i].gameObject.SetActive(false);
            }
            CustomSettings.SetActive(false);
        }
    }

    private void ResetBoard()
    {
        Board = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject board in Board)
        {
            board.GetComponent<StateHandler>().SetButtonState(ButtonState.DontClick);
        }

        NotInState = Board.ToList();
        IsGameOver = false;
    }

    private float GetTime()
    {
        float result = 1f;

        switch (Difficult.value)
        {
            case 0://Easy
                result = 1f;
                break;
            case 1://Normal
                result = 0.80f;
                break;
            case 2://Hard
                result = 0.65f;
                break;
            case 3://Imposible
                result = 0.45f;
                break;
            case 4://Custom
                if (TimeField.text != "" && float.Parse(TimeField.text.Replace(".", ",")) > 0f)
                {
                    result = float.Parse(TimeField.text.Replace(".", ","));
                }
                else
                {
                    result = 1f;
                }
                break;
        }

        return result;
    }

    private int[] GetAppearchance()
    {
        int[] result = new int[EasyAppearchance.Length];
        OnDifficultChange();

        for (int i = 0; i < result.Length; ++i)
        {
            if (Custom)
            {
                Debug.Log(ActivEnemyTypes[i].isOn && CustomAppearchance[i].text != "");
                result[i] = (ActivEnemyTypes[i].isOn && CustomAppearchance[i].text != "") ? int.Parse(CustomAppearchance[i].text) : 0;
            }
            else
            {
                result[i] = ActivEnemyTypes[i].isOn ? Appearchance[i] : 0;
            }
        }

        return result;
    }

    public void StartGame()
    {
        StartCoroutine(nameof(FirstRound));
    }

    public IEnumerator FirstRound()
    {
        GameManager.InSettings = false;
        Startbildschirm.SetActive(false);
        StartCountdown.SetActive(true);
        GameManager.InHowToPlay = false;
        GameManager.HelpPanel.SetActive(false);

        for (int i = 3; i > 0; --i) 
        {
            StartCountdown.GetComponent<TMP_Text>().text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        Game.SetActive(true);
        ResetBoard();

        StartCountdown.GetComponent<TMP_Text>().text = "";
        StartCountdown.SetActive(false);
        Game.SetActive(true);

        NewRound();
    }

    private void NewRound()
    {
        InvokeRepeating(nameof(ChangeButtonState), 0.5f, GetTime());
    }

    private void ChangeButtonState()
    {
        //Board full
        if (NotInState.Count <= 0)
        {
            GameOver();
        }

        if (!IsGameOver)
        {
            int rnd = Random.Range(0, NotInState.Count);
            NotInState.ElementAt(rnd).GetComponent<StateHandler>().SetRandomButtonState(GetAppearchance());
            NotInState.RemoveAt(rnd);
        }
    }

    public void GameOver()
    {
        CancelInvoke(nameof(ChangeButtonState));
        MainCamera.GetComponent<AudioSource>().clip = GameManager.ButtonWrongClickClip;
        MainCamera.GetComponent<AudioSource>().Play();
        ResetBoard();
        GameManager.GetComponent<HighscoreGame1Manager>().SetScore(0);
        DebugText.text = "";
        IsGameOver = true;
        GameManager.StartButton.SetActive(true);
        GameManager.IsPlaying = false;
        //Game.SetActive(false);
        //Startbildschirm.SetActive(true);
    }


    // Not perfect for all Resolutions --> Main Ressolution S23 5G (1080:2408) i change it maybe later
    private void GenerateBoard(Vector2 _boardSize)
    {
        for (int y = 0; y < _boardSize.y; ++y) 
        {
            for(int x = 0; x < _boardSize.x; ++x)
            {
                GameObject newBtn = Instantiate(ButtonPrefab);
                newBtn.name = "Button (" + x + ", " + y + ")";
                newBtn.GetComponentInChildren<TextMeshProUGUI>().text = ShowPos? "(" + x + ", " + y + ")" : "";
                newBtn.transform.SetParent(GameBoard.transform, false);

                RectTransform btnPos = newBtn.GetComponent<RectTransform>();

                btnPos.sizeDelta = new Vector2((1080 -(Edge.x))/_boardSize.x, (1080 - Edge.x) / _boardSize.x);
                btnPos.anchoredPosition = CenterBoardPosition(newBtn, _boardSize, new Vector2(x, y));

                newBtn.GetComponent<Button>().onClick.AddListener(() => DebugInformations(newBtn));
            }
        }
    }

    // Calculate the position of the button in an area and center it.
    private Vector2 CenterBoardPosition(GameObject _btn, Vector2 _boardSize, Vector2 _currentBtn)
    {
        RectTransform btnPos = _btn.GetComponent<RectTransform>();
        RectTransform boardPos = GameBoard.GetComponent<RectTransform>();

        float x = btnPos.sizeDelta.x * _currentBtn.x + btnPos.sizeDelta.x * btnPos.pivot.x + Edge.x / 2;
        float y = -btnPos.sizeDelta.y * _currentBtn.y - btnPos.sizeDelta.y * btnPos.pivot.y - Edge.y / 2;
        
        return new Vector2(x, y);
    }

    public void DebugInformations(GameObject _obj)
    {
        if(GameManager.IsPlaying && GameManager.ShowDebug)
        {
            RectTransform btnPos = _obj.GetComponent<RectTransform>();
            DebugText.text = (btnPos.name + "Size: (" + btnPos.sizeDelta.x + ", " + btnPos.sizeDelta.x + ")\n" + "Width: " + Screen.width + " Height: " + Screen.height + "\nState: " + _obj.GetComponent<StateHandler>().GetCurrentState().ToString());
        }
        else
        {
            DebugText.text = "";
        }
    }
}