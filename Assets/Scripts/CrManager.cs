using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ¡Muy importante para cambiar de escena!

/// <summary>
/// Gestiona la lógica de los botones de la pantalla de Derrota (Game Over).
/// </summary>
public class CrManager : MonoBehaviour
{

    public void VolverAlMenu()
    {
        // Carga la escena del Menú Principal
        Debug.Log("Volviendo al Menú Principal...");
        SceneManager.LoadScene("MainMenu");
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