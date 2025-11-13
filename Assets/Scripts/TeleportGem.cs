// ¡SCRIPT ACTUALIZADO!
// ¡Volvemos a usar tu idea de 'Vector2' (coordenadas)!
// Pero MANTENEMOS la lógica de la música.
using UnityEngine;

/// <summary>
/// Este script se pone en la "Gema".
/// Cuando el jugador lo toca, lo teletransporta a unas coordenadas
/// Y le dice al AudioManager que cambie la música.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TeleportGem : MonoBehaviour
{
    [Header("Configuración de Teletransporte")]
    [Tooltip("Las coordenadas (X, Y) a donde irá el jugador")]
    public Vector2 teleportPosition = new Vector2(0f, 0f); // ¡Usamos tu Vector2!

    [Header("Configuración de Música")]
    [Tooltip("La música que sonará al llegar a la nueva zona")]
    public AudioClip musicForNewZone;

    private Collider2D triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobar si el objeto que ha entrado es el Jugador
        if (other.CompareTag("Player"))
        {
            // --- 1. Validar (¡Ya no necesitamos 'destination'!) ---
            if (musicForNewZone == null)
            {
                Debug.LogError("¡Gema de teletransporte no tiene 'Music' asignada!", this);
                return;
            }
            if (AudioManager.Instance == null)
            {
                Debug.LogError("¡AudioManager.Instance no encontrado! ¿Está en MainMenuScene?", this);
                return;
            }

            // --- 2. Cambiar la música (primero) ---
            AudioManager.Instance.PlayNewTrack(musicForNewZone);

            // --- 3. Teletransportar al jugador (¡usando tu lógica de Rigidbody y Vector2!) ---

            Transform playerTransform = other.transform.root;
            Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // ¡Usamos tu 'teleportPosition' (la coordenada)!
                rb.position = teleportPosition;
                rb.linearVelocity = Vector2.zero; // Resetea la velocidad
            }
            else
            {
                // Si no tiene Rigidbody
                playerTransform.position = teleportPosition;
            }

            Debug.Log("¡Teletransporte y música activados!");
        }
    }
}