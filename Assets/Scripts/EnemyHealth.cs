// ¡SCRIPT ACTUALIZADO!
// Le hemos añadido la lógica de "Aumentar Velocidad" en Die().
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

    // --- ¡NUEVAS VARIABLES AQUÍ! ---
    [Header("Recompensas (Opcional)")]
    [Tooltip("¿Este enemigo cura al jugador al morir?")]
    public bool healPlayerOnDeath = false;
    [Tooltip("Cuánta vida (corazones) restaura (si usa el sistema simple)")]
    public int healthToRestore = 1;

    [Tooltip("¿Este enemigo aumenta la velocidad del jugador al morir?")]
    public bool increasePlayerSpeedOnDeath = false; // ¡NUEVO!
    [Tooltip("Cuánto aumenta la velocidad (ej. 0.1f)")]
    public float speedToRestore = 0.1f;         // ¡NUEVO!
    // --- Fin de las Nuevas Variables ---

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

        // --- ¡LÓGICA DE RECOMPENSAS ACTUALIZADA! ---
        // Comprobar si debemos curar al jugador
        if (healPlayerOnDeath)
        {
            PlayerHealth.Instance?.Heal(healthToRestore);
        }
        // ¡NUEVO! Comprobar si debemos aumentar la velocidad
        if (increasePlayerSpeedOnDeath)
        {
            PlayerHealth.Instance?.ApplySpeedBoost(speedToRestore);
        }
        // --- Fin de la Lógica de Recompensas ---


        // Desactivar componentes
        if (healthCanvas != null)
            healthCanvas.SetActive(false);
        if (GetComponent<EnemyAI>() != null)
            GetComponent<EnemyAI>().enabled = false;
        col.enabled = false;
        this.enabled = false; 

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