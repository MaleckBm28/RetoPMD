// ¡SCRIPT ACTUALIZADO!
// Ahora "habla" con EnemyAI para cancelar ataques.
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

    [Header("Recompensas (Opcional)")]
    public bool healPlayerOnDeath = false;
    public int healthToRestore = 1;
    public bool increasePlayerSpeedOnDeath = false;
    public float speedToRestore = 0.1f;

    // --- Componentes Internos ---
    private Animator animator;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    public bool isDead = false;
    private GameObject healthCanvas;
    private EnemyAI enemyAI; // ¡NUEVO! Referencia al "cerebro" (IA)

    void Start()
    {
        currentHealth = maxHealth;
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>(); // ¡NUEVO! Asignamos el cerebro

        if (animator == null)
        {
            useTakeHitAnimation = false;
            useDeathAnimation = false;
        }

        if (healthBarFill != null)
        {
            healthCanvas = healthBarFill.GetComponentInParent<Canvas>()?.gameObject;
            healthBarFill.fillAmount = 1;
            if (healthCanvas != null)
                healthCanvas.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // --- ¡¡NUEVA LÓGICA DE CANCELACIÓN!! ---
        // ¡Antes de recibir el daño, dile al cerebro que cancele cualquier ataque!
        if (enemyAI != null)
        {
            enemyAI.CancelAttack();
        }
        // --- Fin de la Lógica de Cancelación ---

        currentHealth -= damage;

        if (healthBarFill != null)
        {
            if (healthCanvas != null)
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

        if (healPlayerOnDeath)
        {
            PlayerHealth.Instance?.Heal(healthToRestore);
        }
        if (increasePlayerSpeedOnDeath)
        {
            PlayerHealth.Instance?.ApplySpeedBoost(speedToRestore);
        }

        if (healthCanvas != null)
            healthCanvas.SetActive(false);
        if (enemyAI != null) // Usamos la referencia que ya tenemos
            enemyAI.enabled = false;
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