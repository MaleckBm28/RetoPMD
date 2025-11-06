using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ¡Muy importante para cambiar de escena!

public class VictoryScreenManager : MonoBehaviour
{
    // Esta función la llamará el OnClick del botón "Volver al Menú"
    public void VolverAlMenu()
    {
        // Asegúrate de que tu escena de menú se llama exactamente "MainMenu"
        // (Tal como la pusiste en el Build Profile)
        Debug.Log("Volviendo al Menú Principal...");
        SceneManager.LoadScene("MainMenu");
    }
}