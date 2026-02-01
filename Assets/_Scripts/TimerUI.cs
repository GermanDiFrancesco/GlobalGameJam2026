using UnityEngine;
using UnityEngine.UI;
using System;

public class TimerUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Text _timerText;
    
    [Header("Clock Settings")]
    [SerializeField] private float _gameTimeInSeconds = 120f;  // 2 minutos de juego
    [SerializeField] private int _startHour = 23;
    [SerializeField] private int _startMinute = 58;
    
    private float _currentTime;
    private bool _isRunning;
    
    // Evento cuando llega a medianoche (00:00)
    public event Action OnMidnightReached;
    
    public bool IsRunning => _isRunning;
    public float CurrentTime => _currentTime;
    
    private void Start()
    {
        _currentTime = _gameTimeInSeconds;
        UpdateClockDisplay();
    }
    
    private void Update()
    {
        if (!_isRunning) return;
        
        _currentTime -= Time.deltaTime;
        
        if (_currentTime <= 0)
        {
            _currentTime = 0;
            _isRunning = false;
            OnMidnightReached?.Invoke();
        }
        
        UpdateClockDisplay();
    }
    
    public void StartTimer()
    {
        _isRunning = true;
    }
    
    public void PauseTimer()
    {
        _isRunning = false;
    }
    
    public void ResetTimer()
    {
        _currentTime = _gameTimeInSeconds;
        _isRunning = false;
        UpdateClockDisplay();
    }
    
    public void AddTime(float seconds)
    {
        _currentTime += seconds;
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
}