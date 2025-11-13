// ¡SCRIPT ACTUALIZADO!
// 1. Añadido "defeatMusicClip".
// 2. "FadeTrack" (fundido) ahora usa "Time.unscaledDeltaTime" para funcionar
//    incluso cuando el juego está pausado (Time.timeScale = 0).
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string VOLUME_KEY = "MasterVolume";

    [Header("Clips de Música")]
    public AudioClip menuMusicClip;
    public AudioClip gameMusicClip;
    public AudioClip defeatMusicClip; // ¡NUEVO HUECO!

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        SetMasterVolume(savedVolume, false);

        if (menuMusicClip != null)
        {
            audioSource.clip = menuMusicClip;
            audioSource.Play();
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetMasterVolume(float volume)
    {
        SetMasterVolume(volume, true);
    }

    public void SetMasterVolume(float volume, bool save)
    {
        AudioListener.volume = volume;

        if (save)
        {
            PlayerPrefs.SetFloat(VOLUME_KEY, volume);
            PlayerPrefs.Save();
        }
    }

    // ¡FUNCIÓN OnSceneLoaded ACTUALIZADA!
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // Lógica de música por escena mejorada
        if (sceneName == "juego")
        {
            PlayNewTrack(gameMusicClip);
        }
        // --- ¡NUEVA LÓGICA DE DERROTA! ---
        else if (sceneName == "Derrota")
        {
            PlayNewTrack(defeatMusicClip);
        }
        // --- Fin de la lógica ---
        else if (sceneName == "MainMenu" || sceneName == "MenuOpciones")
        {
            PlayNewTrack(menuMusicClip);
        }
        // (Si es otra escena (como Victory), la música no cambia)
    }

    public void PlayNewTrack(AudioClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogWarning("PlayNewTrack: Se ha pasado un clip nulo.");
            return;
        }

        if (audioSource.clip == newClip && audioSource.isPlaying)
        {
            return;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // El fundido ahora funcionará aunque el juego esté pausado
        fadeCoroutine = StartCoroutine(FadeTrack(newClip));
    }

    // ¡CORRUTINA ACTUALIZADA!
    IEnumerator FadeTrack(AudioClip newClip)
    {
        float fadeTime = 0.5f;

        // (Calculamos el volumen basado en AudioListener 
        // para que el fade funcione aunque el AudioSource esté en 0)
        float startVolume = AudioListener.volume;
        float currentClipVolume = audioSource.volume; // Volumen actual del clip

        // 1. Bajar volumen (Fade out)
        // Usamos Time.unscaledDeltaTime para ignorar el Time.timeScale = 0
        while (currentClipVolume > 0)
        {
            currentClipVolume -= startVolume * Time.unscaledDeltaTime / fadeTime;
            audioSource.volume = currentClipVolume; // Aplicar el volumen al AudioSource
            yield return null;
        }

        // 2. Cambiar clip y subir volumen (Fade in)
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        while (currentClipVolume < startVolume)
        {
            currentClipVolume += startVolume * Time.unscaledDeltaTime / fadeTime;
            audioSource.volume = currentClipVolume; // Aplicar el volumen al AudioSource
            yield return null;
        }

        audioSource.volume = startVolume;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}