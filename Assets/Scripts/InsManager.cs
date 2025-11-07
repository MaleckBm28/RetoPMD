using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para gestionar escenas

/// <summary>
/// Gestiona la pantalla de Instrucciones.
/// Su única función es volver al menú principal.
/// </summary>
public class InsManager : MonoBehaviour
{
    // Nombre de la escena del menú principal (asegúrate de que coincida)
    public string mainMenuSceneName = "MainMenu"; 

    public void VolverAlMenu()
    {
        // Re-anuda el juego por si venimos de una pausa (buena práctica)
        Time.timeScale = 1f; 

        // Carga la escena del menú principal
        SceneManager.LoadScene(mainMenuSceneName);
    }
}