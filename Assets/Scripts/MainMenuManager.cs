using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para gestionar escenas

/// <summary>
/// Este script gestiona la l�gica de los botones del Men� Principal.
/// Se debe a�adir a un objeto vac�o (ej. "MenuManager") en la escena del men�.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // --- FUNCI�N PARA EL BOT�N "JUGAR" ---

    /// <summary>
    /// Carga la escena principal del juego.
    /// El nombre "GameScene" debe coincidir EXACTAMENTE con el nombre de tu escena de juego.
    /// </summary>
    public void Jugar()
    {
        // Aseg�rate de que tu escena de juego se llama "GameScene"

        string nombreDeEscena = "Demo";

        // O cambia este string por el nombre correcto.
        Debug.Log($"Cargando escena '{nombreDeEscena}'...");
        SceneManager.LoadScene(nombreDeEscena);
    }

    // --- FUNCI�N PARA EL BOT�N "INSTRUCCIONES" ---

    /// <summary>
    /// Muestra las instrucciones.
    /// (Por ahora, solo imprime un mensaje. M�s adelante, podr�a activar un panel de UI)
    /// </summary>
    public void MostrarInstrucciones()
    {
        // TODO: Implementar un panel de instrucciones
        Debug.Log("El bot�n 'Instrucciones' ha sido pulsado.");
        SceneManager.LoadScene("Instrucciones");
    }

        public void MostrarCreditos()
    {
        Debug.Log("El bot�n 'Creditos' ha sido pulsado.");
        SceneManager.LoadScene("Creditos");
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");

        // Esta comprobaci�n es importante:
        // Application.Quit() solo funciona en una build (un juego ya compilado).
        // No funciona dentro del Editor de Unity.

#if UNITY_EDITOR
            // Si estamos en el Editor, detenemos el modo "Play"
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si estamos en una build (PC, Mac, etc.), cerramos la aplicaci�n
        Application.Quit();
#endif
    }
}
