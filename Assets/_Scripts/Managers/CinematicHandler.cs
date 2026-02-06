using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[System.Serializable]
public class CinematicFrame
{
    public Sprite image;
    public AudioClip sound;
    public float duration = 2f; // tiempo en pantalla
}
public class CinematicHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image cinematicImage;

    [Header("Cinematic Frames")]
    [SerializeField] private List<CinematicFrame> frames = new();

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private bool autoStart = false;

    private int currentIndex;
    private Coroutine cinematicRoutine;

    // Referencia al GameController (inyectada o buscada)
    private GameController gameController;

    private void Awake()
    {
        cinematicImage.enabled = false;
        if (autoStart) Play();
    }

    public void Play()
    {
        if (frames.Count == 0)
        {
            Debug.LogWarning("CinematicHandler: No hay frames asignados");
            EndCinematic();
            return;
        }

        cinematicImage.enabled = true;
        currentIndex = 0;

        if (cinematicRoutine != null)
            StopCoroutine(cinematicRoutine);

        cinematicRoutine = StartCoroutine(PlayRoutine());
    }
    public void Skip()
    {
        if (cinematicRoutine != null)
            StopCoroutine(cinematicRoutine);

        EndCinematic();
    }

    private IEnumerator PlayRoutine()
    {
        while (currentIndex < frames.Count)
        {
            ShowFrame(frames[currentIndex]);
            yield return new WaitForSecondsRealtime(frames[currentIndex].duration);
            currentIndex++;
        }

        EndCinematic();
    }

    private void ShowFrame(CinematicFrame frame)
    {
        cinematicImage.sprite = frame.image;

        if (frame.sound != null && audioSource != null)
        {
            audioSource.PlayOneShot(frame.sound);
        }
    }

    private void EndCinematic()
    {
        cinematicImage.enabled = false;

        //gameController?.OnCinematicFinished();

        cinematicRoutine = null;
    }
}

