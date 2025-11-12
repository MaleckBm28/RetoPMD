// ¡SCRIPT ACTUALIZADO!
// Ahora este script es el "JEFE" del ataque.
// Controla el input, el cooldown, la animación Y el daño.
using UnityEngine;
using System.Collections; // Necesario para las Corrutinas

/// <summary>
/// Gestiona la lógica de ataque del jugador.
/// Debe ir en el objeto del Jugador.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public Transform attackPoint;    
    public float attackRange = 0.5f;   
    public int attackDamage = 1;       

    [Header("Tiempos de Ataque")]
    [Tooltip("El tiempo total de la animación de ataque (para evitar spam)")]
    public float attackCooldown = 1.0f; 
    [Tooltip("El momento exacto en que el golpe hace daño (ej. 0.5s)")]
    public float attackHitDelay = 0.5f; 

    [Header("Detección")]
    public LayerMask enemyLayer;     

    // --- ¡NUEVAS REFERENCIAS! ---
    private Animator animator; // Referencia al Animator
    private bool isAttacking = false; 

    void Start()
    {
        // ¡NUEVO! Obtenemos el Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("PlayerAttack no pudo encontrar el componente Animator.");
        }
    }

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

        // 1. ¡NUEVO! Activar la animación de ataque
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // ¡Este es el trigger de tu script Bandit/PlayerMovement!
        }
        
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
        // ¡Asegúrate de que 'attackCooldown' sea IGUAL o MAYOR que el 'attackHitDelay'!
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