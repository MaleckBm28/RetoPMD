using UnityEngine;
using System.Collections; // Necesario para las Corrutinas

/// <summary>
/// Gestiona la lógica de ataque del jugador.
/// Debe ir en el objeto del Jugador.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public Transform attackPoint;    // El objeto vacío que marca dónde se origina el golpe
    public float attackRange = 0.5f;   // El radio del círculo de detección del golpe
    public int attackDamage = 1;       // Cuánto daño hace cada golpe

    [Header("Tiempos de Ataque")]
    [Tooltip("El tiempo total de la animación de ataque (para evitar spam)")]
    public float attackCooldown = 1.0f; // Asumimos que tu anim "lenta" dura 1 seg. ¡Ajústalo!
    [Tooltip("El momento exacto en que el golpe hace daño (ej. 0.5s)")]
    public float attackHitDelay = 0.5f; // El delay que pediste

    [Header("Detección")]
    public LayerMask enemyLayer;     // La capa en la que se encuentran los enemigos (IMPORTANTE)

    private bool isAttacking = false; // Flag para evitar spamear el ataque

    // Update se llama una vez por frame
    void Update()
    {
        // Si el jugador pulsa el botón izquierdo Y no está atacando ya
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            // Inicia la corrutina de ataque
            StartCoroutine(PerformAttack());
        }
    }

    /// <summary>
    /// Realiza el ataque con delays para sincronizar con la animación.
    /// </summary>
    IEnumerator PerformAttack()
    {
        isAttacking = true; // Marcamos que estamos atacando

        // 1. Opcional: Activar la animación de ataque
        // animator.SetTrigger("Attack");

        // 2. Esperar el delay antes de que el golpe conecte
        yield return new WaitForSeconds(attackHitDelay);

        // 3. Detectar enemigos (el código de daño)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies.Length > 0)
        {
            Debug.Log($"¡Golpe ejecutado (con delay)! {hitEnemies.Length} enemigos en rango.");
        }

        foreach(Collider2D enemyCollider in hitEnemies)
        {
            Debug.Log("¡Le diste a " + enemyCollider.name + "!");

            EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

        // 4. Esperar a que la animación termine (el tiempo restante del cooldown)
        float remainingTime = attackCooldown - attackHitDelay;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // 5. Permitir que el jugador vuelva a atacar
        isAttacking = false;
    }

    /// <summary>
    /// Ayuda visual para ver el rango de ataque en el editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        // Dibuja un círculo rojo en el editor (solo visible en la vista 'Scene')
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

