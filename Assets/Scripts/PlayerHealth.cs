using UnityEngine;
using UnityEngine.SceneManagement; // Para reiniciar la escena
using System.Collections;

/// <summary>
/// Gestiona la salud del Jugador.
/// Debe ir en el objeto del Jugador.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 10;
    private int currentHealth;

    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false; // Para invencibilidad temporal tras recibir un golpe

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Asume que el sprite es un hijo
        
        // O usa esto si el SpriteRenderer está en el mismo objeto
        // spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    public void TakeDamage(int damage)
    {
        // Si somos invencibles (acabamos de recibir un golpe), no hacemos nada
        if (isInvincible)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log($"Jugador recibe {damage} de daño. Salud restante: {currentHealth}");

        // Activa la invencibilidad temporal
        StartCoroutine(InvincibilityFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Opcional: Activar un trigger de "PlayerTakeHit" en el Animator del jugador
            // GetComponent<Animator>().SetTrigger("TakeHit");
        }
    }

    private void Die()
    {
        Debug.Log("¡El jugador ha muerto!");
        
        // Opcional: Activar animación de muerte del jugador
        // GetComponent<Animator>().SetTrigger("Die");
        
        // Pausa el juego y reinicia la escena actual tras 2 segundos
        Time.timeScale = 0.5f; // Ralentiza el juego
        Invoke("RestartScene", 2f);
    }

    private void RestartScene()
    {
        Time.timeScale = 1f; // Restaura el tiempo normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        
        // Bucle de parpadeo durante 1 segundo
        float duration = 1.0f;
        float flashTime = 0.1f;
        float timer = 0f;

        while (timer < duration)
        {
            // Alterna la visibilidad del sprite
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashTime);
            timer += flashTime;
        }

        spriteRenderer.enabled = true; // Asegura que termine visible
        isInvincible = false;
    }
}
