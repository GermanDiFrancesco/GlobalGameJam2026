using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    #region Dependencies
    [Header("Dependencies")]
    [SerializeField] private GameController gameController;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private GameObject musicManager;
    #endregion

    [Header("Sound Clips")]
    [SerializeField] private AudioClip winAudio;
    [SerializeField] private AudioClip loseAudio;

    [Header("Carriage")]
    [SerializeField] private CarriageHandler carriage;
    
    #region UI Elements
    [Header("Buttons")]
    [SerializeField] private Button comenzarButton;

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
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    #endregion

    #region Timer
    [Header("Timer")]
    [SerializeField] private Text _timerText;
    
    public float GameTimeInSeconds => _gameTimeInSeconds; 
    private float _currentTime;
    private bool _isTimerRunning;
    
    public event Action OnMidnightReached;
    #endregion

    #region Unity Callbacks
    private void Update()
    {
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

    #region Screen Management
    public void ShowScreen(string screenName)
    {
        HideAllScreens();
        switch (screenName)
        {
            case "Menu":
                menuScreen?.SetActive(true);
                break;
            case "Win":
                winScreen?.SetActive(true);
                SoundManager.PlaySoundAndDestroy(winAudio);
                Destroy(musicManager);
                break;
            case "Lose":
                loseScreen?.SetActive(true);
                SoundManager.PlaySoundAndDestroy(loseAudio);
                Destroy(musicManager);                    
                break;
            default:
                Debug.LogWarning("Nombre de pantalla no reconocido: " + screenName);
                break;
        }
    }
    public void HideAllScreens()
    {
        menuScreen?.SetActive(false);
        winScreen?.SetActive(false);
        loseScreen?.SetActive(false);
        Time.timeScale = 0f;
    }
    
    public void OnStartGameButtonPressed()
    {
        gameController.Reset();
        HideAllScreens();
        
        Time.timeScale = 1f;
        StartTimer();
        
        // Iniciar la carroza
        if (carriage != null)
        {
            carriage.StartMovement();
        }
        else
        {
            Debug.LogWarning("Carriage no está asignado en UIManager!");
        } 
        
        Debug.Log("Juego iniciado.");        
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