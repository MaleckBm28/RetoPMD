// ¡SCRIPT MUY ACTUALIZADO!
// ¡Ahora puede curar al jugador al morir!
using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 3; 
    public int currentHealth;

    [Header("Referencias de UI (Barra de Vida)")]
    public Image healthBarFill; 

    [Header("Configuración de Animación (Opcional)")]
    public bool useTakeHitAnimation = true;
    public bool useDeathAnimation = true;
    public float destroyDelay = 2f; 

    // --- ¡NUEVAS VARIABLES DE CURACIÓN! ---
    [Header("Recompensas (Opcional)")]
    [Tooltip("Marcar si este enemigo debe curar al jugador al morir")]
    public bool healPlayerOnDeath = false;
    [Tooltip("Cuántos puntos de vida (corazones) restaura")]
    public int healthToRestore = 1;

    // --- Componentes Internos ---
    private Animator animator;
    private Collider2D col; 
    private SpriteRenderer spriteRenderer; 
    public bool isDead = false;
    private GameObject healthCanvas; 

    void Start()
    {
        currentHealth = maxHealth;
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            useTakeHitAnimation = false;
            useDeathAnimation = false;
        }

        if (healthBarFill != null)
        {
            healthCanvas = healthBarFill.GetComponentInParent<Canvas>()?.gameObject;
            healthBarFill.fillAmount = 1; 
            if(healthCanvas != null)
                healthCanvas.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        if (healthBarFill != null)
        {
            if(healthCanvas != null)
                healthCanvas.SetActive(true);
            float fillAmount = (float)currentHealth / (float)maxHealth;
            healthBarFill.fillAmount = fillAmount;
        }

        if (currentHealth > 0)
        {
            if (useTakeHitAnimation && animator != null)
            {
                animator.SetTrigger("TakeHit");
            }
            else
            {
                StartCoroutine(BlinkEffect());
            }
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Enemigo ha muerto.");

        if (healthCanvas != null)
            healthCanvas.SetActive(false);
        if (GetComponent<EnemyAI>() != null)
            GetComponent<EnemyAI>().enabled = false;
        col.enabled = false;
        this.enabled = false; 

        // --- ¡NUEVA LÓGICA DE CURACIÓN! ---
        if (healPlayerOnDeath)
        {
            // Busca al jugador (usando el Singleton) y lo cura
            if (PlayerHealth.Instance != null)
            {
                PlayerHealth.Instance.Heal(healthToRestore);
            }
        }
        // --- Fin de la lógica de curación ---

        if (useDeathAnimation && animator != null)
        {
            animator.SetTrigger("death"); 
            Destroy(gameObject, destroyDelay); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    IEnumerator BlinkEffect()
    {
        if (spriteRenderer == null) yield break; 
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}