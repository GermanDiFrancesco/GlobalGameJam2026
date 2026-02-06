using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum MusicState
{
    None,
    Menu,
    Tutorial,
    Game
}
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip tutorialMusic;
    [SerializeField] private AudioClip gameMusic;

    private AudioSource audioSource;
    private MusicState currentState = MusicState.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // =========================
    // PUBLIC API
    // =========================

    public void EnterMenu()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        currentState = MusicState.Menu;
    }

    public void EnterTutorial()
    {
        // Si el tutorial ya estaba sonando y estaba pausado → continuar
        if (currentState == MusicState.Menu && audioSource.clip == tutorialMusic)
        {
            audioSource.UnPause();
            currentState = MusicState.Tutorial;
            return;
        }

        PlayNewClip(tutorialMusic, loop: true);
        currentState = MusicState.Tutorial;
    }

    public void EnterGame()
    {
        // Si el juego ya estaba sonando y estaba pausado → continuar
        if (currentState == MusicState.Menu && audioSource.clip == gameMusic)
        {
            audioSource.UnPause();
            currentState = MusicState.Game;
            return;
        }

        PlayNewClip(gameMusic, loop: false);
        currentState = MusicState.Game;
    }

    public void StopMusic()
    {
        audioSource.Stop();
        audioSource.clip = null;
        currentState = MusicState.None;
    }

    // =========================
    // INTERNAL
    // =========================

    private void PlayNewClip(AudioClip clip, bool loop)
    {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.time = 0f;
        audioSource.Play();
    }
}