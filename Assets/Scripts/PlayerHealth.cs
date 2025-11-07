// ¡SCRIPT ACTUALIZADO!
// He añadido la referencia al Animator y el SetTrigger("TakeHit") en TakeDamage.
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool isDead = false;

    // --- Referencias de Componentes ---
    private SpriteRenderer spriteRenderer;
    private Animator animator; // ¡NUEVO!

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // ¡NUEVO! Asignamos el Animator
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"El jugador recibe {damage} de daño. Vida restante: {currentHealth}");

        // --- ¡LÓGICA DE ANIMACIÓN AÑADIDA! ---
        if (animator != null)
        {
            // Llama al trigger "TakeHit" en el Animator del jugador
            animator.SetTrigger("Hurt");
        }
        // --- Fin de la lógica de animación ---

        // Inicia el parpadeo
        StartCoroutine(BlinkEffect());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("¡El jugador ha muerto!");

        // (Opcional) ¡Activa la animación de muerte del jugador!
        // if (animator != null)
        // {
        //     animator.SetTrigger("Die");
        // }

        // Llama al "Director de Terror" (si lo estás usando)
        // GameFileManager.Instance.HandlePlayerDeath();
        
        // O simplemente carga la escena de derrota (Método Simple)
        
        // --- Cargar Escena de Derrota (Método Simple) ---
        // 1. Pausar el juego (opcional, pero recomendado)
        Time.timeScale = 0f; 
        
        // 2. Cargar la escena de derrota
        // ¡Asegúrate de que "DefeatScene" está en tu Build Profiles!
        SceneManager.LoadScene("Derrota");
        
        // --- Fin del método simple ---


        // (Opcional) Desactiva al jugador o su script de movimiento
        GetComponent<Collider2D>().enabled = false;
        // ... (desactivar script de movimiento, etc.)
        this.enabled = false;
    }

    IEnumerator BlinkEffect()
    {
        // Parpadeo rojo
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}