// ¡SCRIPT ACTUALIZADO!
// ¡Ahora también reproduce la música del menú!
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para la lógica de escenas

// Esta línea FUERZA a que el objeto tenga un AudioSource.
[RequireComponent(typeof(AudioSource))]
public class AudioMenuManager : MonoBehaviour
{
    public static AudioMenuManager Instance { get; private set; }

    // Clave para guardar el volumen
    private const string VOLUME_KEY = "MasterVolume";
    
    // --- ¡NUEVO! Referencias de Audio ---
    [Header("Clips de Música")]
    public AudioClip menuMusicClip;   // Arrastra aquí la música del menú
    public AudioClip gameMusicClip;   // (Opcional) Arrastra aquí la música del juego

    private AudioSource audioSource;
    // --- Fin de lo Nuevo ---

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return; // Importante: Salir si somos un duplicado
        }

        // Obtener el componente AudioSource que hemos forzado
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Queremos que la música sea un bucle
    }

    void Start()
    {
        // 1. Cargar el volumen guardado
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        SetMasterVolume(savedVolume, false); // No guardar al inicio

        // 2. ¡NUEVO! Reproducir la música del menú
        if (menuMusicClip != null)
        {
            audioSource.clip = menuMusicClip;
            audioSource.Play();
        }

        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Esta es la función que conectamos al 'On Value Changed' del Slider.
    /// </summary>
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

    // --- ¡NUEVA FUNCIÓN! ---
    /// <summary>
    /// Se llama cada vez que se carga una escena nueva.
    /// Lo usamos para cambiar la música si es necesario.
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si cargamos la escena del juego (ej. "Demo")
        if (scene.name == "Demo") // ¡Asegúrate de que el nombre sea correcto!
        {
            // Y si la música del juego es diferente...
            if (gameMusicClip != null && audioSource.clip != gameMusicClip)
            {
                audioSource.clip = gameMusicClip;
                audioSource.Play();
            }
        }
        // Si cargamos el Menú (ej. "MainMenuScene")
        else if (scene.name == "MainMenuScene")
        {
            // Y si la música del menú es diferente...
            if (menuMusicClip != null && audioSource.clip != menuMusicClip)
            {
                audioSource.clip = menuMusicClip;
                audioSource.Play();
            }
        }
        // (Si es "OptionsScene", "DefeatScene", etc., no hace nada y la música sigue sonando)
    }

    // Limpieza al cerrar
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}