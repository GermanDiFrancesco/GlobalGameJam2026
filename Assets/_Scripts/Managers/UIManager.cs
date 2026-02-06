using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameController gameController;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private CinematicHandler cinematicHandler;

    [Header("Clock Settings")]
    [SerializeField] private float _gameTimeInSeconds = 120f;
    [SerializeField] private int _startHour = 23;
    [SerializeField] private int _startMinute = 58;
    
    [Header("Identikit")]
    [SerializeField] private Image hatIMG;
    [SerializeField] private Image ornamentIMG;
    [SerializeField] private Image eyesIMG;

    [Header("UI Screens")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject scoreScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject hudScreen;
    [SerializeField] private GameObject tutorialScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip winAudio;
    [SerializeField] private AudioClip loseAudio;
    [SerializeField] private AudioClip buttonAudio;
    [SerializeField] private AudioClip buttonReversedAudio;

    [Header("Inputs")]
    [SerializeField] private KeyCode interactKey = KeyCode.Space;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Timer")]
    [SerializeField] private Text _timerText;
    
    public float GameTimeInSeconds => _gameTimeInSeconds; 
    private float _currentTime;
    private bool _isTimerRunning;
    
    public event Action OnMidnightReached;

    #region Unity Callbacks

    private void Start()
    {
        ShowScreen("Menu");
        musicManager.FirstEnterMenu();
    }


    private void Update()
    {
        if (Input.GetKeyDown(pauseKey)) ShowScreen("Menu");

        if (!_isTimerRunning) return;
        
        _currentTime -= Time.deltaTime;
        
        if (_currentTime <= 0)
        {
            _currentTime = 0;
            _isTimerRunning = false;
            OnMidnightReached?.Invoke();
        }
        
        UpdateClockDisplay();
    }
    #endregion

    #region Screen Management
    public void ShowScreen(string screenName)
    {
        Time.timeScale = 0f;
        HideAllScreens();
        switch (screenName)
        {
            case "Menu":
                menuScreen?.SetActive(true);
                musicManager.EnterMenu();
                break;
            case "Score":
                scoreScreen?.SetActive(true);
                Debug.Log("Score Screen");
                break;
            case "Credits":
                creditsScreen?.SetActive(true);
                Debug.Log("Credits Screen");
                break;

            case "Tutorial":
                Time.timeScale = 1f;
                gameController.StartTutorial();
                tutorialScreen?.SetActive(true);
                musicManager.EnterGame();//musicManager.EnterTutorial();
                Debug.Log("Tutorial iniciado");
                break;
            case "Game":
                Time.timeScale = 1f;
                hudScreen?.SetActive(true);
                musicManager.EnterGame();
                StartTimer();
                break;

            case "Win":
                winScreen?.SetActive(true);
                SoundManager.PlaySoundAndDestroy(winAudio);
                //Destroy(musicManager);
                break;
            case "Lose":
                loseScreen?.SetActive(true);
                SoundManager.PlaySoundAndDestroy(loseAudio);
                //Destroy(musicManager);                    
                break;

            default:
                Debug.LogWarning("Nombre de pantalla no reconocido: " + screenName);
                break;
        }
    }
    public void HideAllScreens()
    {
        Time.timeScale = 0f;
        menuScreen?.SetActive(false);
        winScreen?.SetActive(false);
        loseScreen?.SetActive(false);
        creditsScreen?.SetActive(false);
        scoreScreen?.SetActive(false);
        tutorialScreen?.SetActive(false);
        hudScreen?.SetActive(false);
    }

    #endregion

    #region Button Press

    public void ButtonPressed_Play() => StartCoroutine(DelayedButtonPressed_Play());
    private IEnumerator DelayedButtonPressed_Play()
    {
        SoundManager.PlaySoundAndDestroy(buttonAudio);
        yield return new WaitForSecondsRealtime(1f);
        ShowScreen("Tutorial");
    }
    public void ButtonPressed_Score() => StartCoroutine(DelayedButtonPressed_Score());
    private IEnumerator DelayedButtonPressed_Score()
    {
        SoundManager.PlaySoundAndDestroy(buttonAudio);
        yield return new WaitForSecondsRealtime(1f);
        ShowScreen("Score");
    }
    public void ButtonPressed_Credits() => StartCoroutine(DelayedButtonPressed_Credits());
    private IEnumerator DelayedButtonPressed_Credits()
    {
        SoundManager.PlaySoundAndDestroy(buttonAudio);
        yield return new WaitForSecondsRealtime(1f);
        ShowScreen("Credits");
    }
    public void ButtonPressed_Exit() => StartCoroutine(DelayedButtonPressed_Exit());
    private IEnumerator DelayedButtonPressed_Exit()
    {
        SoundManager.PlaySoundAndDestroy(buttonReversedAudio);
        yield return new WaitForSecondsRealtime(1f);
        Application.Quit();
    }
    public void ButtonPressed_Return() => StartCoroutine(DelayedButtonPressed_Return());
    private IEnumerator DelayedButtonPressed_Return()
    {
        SoundManager.PlaySoundAndDestroy(buttonReversedAudio);
        yield return new WaitForSecondsRealtime(1f);
        ShowScreen("Menu");
    }

    #endregion

    #region Timer Methods
    public void StartTimer()
    {
        _currentTime = _gameTimeInSeconds;
        _isTimerRunning = true;
        UpdateClockDisplay();
    }

    public void PauseTimer()
    {
        _isTimerRunning = false;
    }

    public void ResetTimer()
    {
        _currentTime = _gameTimeInSeconds;
        _isTimerRunning = false;
        UpdateClockDisplay();
    }
    private void UpdateClockDisplay()
    {
        // Calcular cuánto tiempo ha pasado desde el inicio
        float elapsedTime = _gameTimeInSeconds - _currentTime;
        int elapsedSeconds = Mathf.FloorToInt(elapsedTime);

        // Calcular la hora actual del "reloj"
        int totalStartTimeInSeconds = (_startHour * 3600) + (_startMinute * 60);
        int currentClockTimeInSeconds = totalStartTimeInSeconds + elapsedSeconds;

        // Si se pasa de medianoche (86400 segundos = 24 horas), hacer wrap
        currentClockTimeInSeconds = currentClockTimeInSeconds % 86400;

        // Convertir de vuelta a horas, minutos y segundos
        int displayHour = (currentClockTimeInSeconds / 3600) % 24;
        int displayMinute = (currentClockTimeInSeconds % 3600) / 60;
        int displaySecond = currentClockTimeInSeconds % 60;

        // Formato 24 horas: "23:58:45"
        _timerText.text = $"{displayHour:00}:{displayMinute:00}:{displaySecond:00}";

        // Cambiar color según se acerca a medianoche
        if (_currentTime <= 10f)
        {
            _timerText.color = Color.red;
        }
        else if (_currentTime <= 30f)
        {
            _timerText.color = Color.yellow;
        }
        else
        {
            _timerText.color = Color.white;
        }

    }
    #endregion

    #region Witness Methods
    public void UpdateIdentikit(
    MaskPartType type,
    Sprite sprite
)
    {
        switch (type)
        {
            case MaskPartType.Hat:
                hatIMG.sprite = sprite;
                hatIMG.gameObject.SetActive(true);
                break;

            case MaskPartType.Ornament:
                ornamentIMG.sprite = sprite;
                ornamentIMG.gameObject.SetActive(true);
                break;

            case MaskPartType.Eyes:
                eyesIMG.sprite = sprite;
                eyesIMG.gameObject.SetActive(true);
                break;
        }
    }

    #endregion


}