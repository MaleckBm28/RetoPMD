// ¡SCRIPT MUY ACTUALIZADO!
// Hemos cambiado la vida de '100' a un sistema de 'corazones'.
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System; // Necesario para 'Action'

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida (Corazones)")]
    public int numberOfHearts = 3; // ¿Cuántos corazones MÁXIMOS tienes?
    private int maxHealth;         // Vida máxima (numberOfHearts * 2)
    public int currentHealth;      // Vida actual

    public bool isDead = false;

    // --- Evento de Salud ---
    // Esto "gritará" a la UI cada vez que la vida cambie.
    public static event Action<int, int> OnHealthChanged; // <current, max>

    // --- Referencias de Componentes ---
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // 1 corazón = 2 puntos de vida (media y entera)
        maxHealth = numberOfHearts * 2;
        currentHealth = maxHealth;

        // Avisa a la UI de que ponga la vida al máximo al empezar
        StartCoroutine(InitialHealthUpdate());
    }

    IEnumerator InitialHealthUpdate()
    {
        // Esperamos un fotograma para asegurarnos de que la UI se ha suscrito
        yield return null;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"El jugador recibe {damage} de daño. Vida restante: {currentHealth}/{maxHealth}");

        // ¡Avisa a la UI de que la vida ha cambiado!
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // --- Lógica de Animación y Parpadeo ---
        if (animator != null)
        {
            // Llama al trigger "TakeHit" en el Animator del jugador
            animator.SetTrigger("Hurt");
        }
        StartCoroutine(BlinkEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // (Opcional) Función para curar
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        isDead = true;
        Debug.Log("¡El jugador ha muerto!");

        // (Opcional) Animación de muerte
        // if (animator != null) { animator.SetTrigger("Die"); }
        
        // --- Cargar Escena de Derrota (Método Simple) ---
        Time.timeScale = 0f; 
        SceneManager.LoadScene("Derrota");

        GetComponent<Collider2D>().enabled = false;
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