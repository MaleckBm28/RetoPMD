using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para gestionar escenas

/// <summary>
/// Este script gestiona la lógica de los botones del Menú Principal.
/// Se debe añadir a un objeto vacío (ej. "MenuManager") en la escena del menú.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // --- FUNCIÓN PARA EL BOTÓN "JUGAR" ---

    /// <summary>
    /// Carga la escena principal del juego.
    /// El nombre "GameScene" debe coincidir EXACTAMENTE con el nombre de tu escena de juego.
    /// </summary>
    public void Jugar()
    {
        // Asegúrate de que tu escena de juego se llama "GameScene"

        string nombreDeEscena = "Demo";

        // O cambia este string por el nombre correcto.
        Debug.Log($"Cargando escena '{nombreDeEscena}'...");
        SceneManager.LoadScene(nombreDeEscena);
    }

    // --- FUNCIÓN PARA EL BOTÓN "INSTRUCCIONES" ---

    /// <summary>
    /// Muestra las instrucciones.
    /// (Por ahora, solo imprime un mensaje. Más adelante, podría activar un panel de UI)
    /// </summary>
    public void MostrarInstrucciones()
    {
        // TODO: Implementar un panel de instrucciones
        Debug.Log("El botón 'Instrucciones' ha sido pulsado.");
        // Ejemplo de cómo podrías hacerlo en el futuro:
        // panelInstrucciones.SetActive(true);
    }

    // --- FUNCIÓN PARA EL BOTÓN "SALIR" ---

    /// <summary>
    /// Cierra la aplicación.
    /// </summary>
    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");

        // Esta comprobación es importante:
        // Application.Quit() solo funciona en una build (un juego ya compilado).
        // No funciona dentro del Editor de Unity.

#if UNITY_EDITOR
            // Si estamos en el Editor, detenemos el modo "Play"
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si estamos en una build (PC, Mac, etc.), cerramos la aplicación
        Application.Quit();
#endif
    }
}
