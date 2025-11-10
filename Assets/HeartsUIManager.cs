// ¡NUEVO SCRIPT!
// Este script es el "traductor".
// Escucha a PlayerHealth y actualiza las imágenes de los corazones.
using UnityEngine;
using UnityEngine.UI; // Necesario para Images
using System.Collections.Generic; // Necesario para List<T>

public class HeartsUIManager : MonoBehaviour
{
    [Header("Referencias de Sprites")]
    public Sprite heartFull;
    // public Sprite heartHalf; // ¡YA NO SE NECESITA!
    public Sprite heartEmpty;

    [Header("Contenedor de Corazones")]
    [Tooltip("El objeto 'Horizontal Layout Group' que contiene las imágenes")]
    public RectTransform heartsContainer;
    [Tooltip("El Prefab de la imagen del corazón que usaremos")]
    public GameObject heartImagePrefab; // Un prefab de un objeto 'Image' de UI

    private List<Image> heartImages = new List<Image>();

    // --- Suscripción a Eventos ---

    private void OnEnable()
    {
        // Suscribirse al evento de PlayerHealth
        PlayerHealth.OnHealthChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        // Darse de baja del evento
        PlayerHealth.OnHealthChanged -= UpdateHearts;
    }

    /// <summary>
    /// Esta función es llamada por el evento de PlayerHealth
    /// </summary>
    void UpdateHearts(int currentHealth, int maxHealth)
    {
        // 1. ¿Cuántos contenedores de corazón necesitamos?
        // ¡CAMBIO! maxHealth ahora es el número de corazones (ej. 3)
        int numHeartContainers = maxHealth; 

        // 2. Asegurarse de que tenemos suficientes objetos 'Image'
        while (heartImages.Count < numHeartContainers)
        {
            // No tenemos suficientes imágenes, crear una nueva
            GameObject newHeart = Instantiate(heartImagePrefab, heartsContainer);
            heartImages.Add(newHeart.GetComponent<Image>());
        }

        // 3. Actualizar los sprites de cada corazón
        for (int i = 0; i < heartImages.Count; i++)
        {
            // Ocultar corazones extra si la vida máxima baja
            if (i >= numHeartContainers)
            {
                heartImages[i].enabled = false;
                continue;
            }

            heartImages[i].enabled = true;

            // --- ¡LÓGICA SIMPLIFICADA! ---
            // i es el índice del corazón (0, 1, 2...)
            // Si nuestro índice (ej. 0) es MENOR que la vida actual (ej. 3), el corazón está lleno.
            // Si currentHealth es 2:
            // Corazón 0: (0 < 2) = Lleno
            // Corazón 1: (1 < 2) = Lleno
            // Corazón 2: (2 < 2) = Falso = Vacío
            if (i < currentHealth)
            {
                // Lleno
                heartImages[i].sprite = heartFull;
            }
            else
            {
                // Vacío
                heartImages[i].sprite = heartEmpty;
            }
        }
    }
}