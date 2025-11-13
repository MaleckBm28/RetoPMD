// ¡SCRIPT SIMPLIFICADO!
// ¡Ya no hay "stoppingDistance"!
// Mide la distancia desde el "AttackPoint", no desde el centro.
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // El jugador
    public LayerMask playerLayer; // La capa del jugador
    [Tooltip("El punto 'hijo' desde donde se origina el golpe (delante del enemigo)")]
    public Transform attackPoint;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private EnemyHealth health;

    [Header("Valores IA")]
    public float moveSpeed = 2f;
    public float chaseRange = 10f;
    [Tooltip("Distancia para parar Y radio del golpe (medido desde AttackPoint)")]
    public float attackRange = 1.5f; // ¡UNA SOLA VARIABLE DE RANGO!
    public float attackDamage = 1f;
    public float attackCooldown = 2f;
    public float attackHitDelay = 0.5f;

    private bool isAttacking = false;
    private bool isChasing = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (attackPoint == null)
        {
            Debug.LogWarning("¡AttackPoint no asignado en " + name + "! Usando transform.position por defecto.");
            attackPoint = transform;
        }
    }

    // ¡¡FUNCIÓN UPDATE SIMPLIFICADA!!
    void Update()
    {
        if (health.isDead || target == null || isAttacking)
        {
            if (!isChasing || health.isDead)
            {
                animator.SetBool("IsWalking", false);
            }
            return;
        }

        // ¡LÓGICA SIMPLIFICADA!
        // Medimos la distancia desde el ATTACKPOINT al jugador.
        float distanceToPlayer = Vector2.Distance(attackPoint.position, target.position);
        bool playerInVision = false;

        // Usamos "attackRange" para decidir si paramos
        if (distanceToPlayer <= attackRange)
        {
            // 1. En rango: ¡Parar y Atacar!
            isChasing = false;
            animator.SetBool("IsWalking", false);
            TryAttack();
            playerInVision = true;
        }
        // ¡CAMBIO! Comprobamos el rango de visión (chaseRange) desde el CENTRO
        else if (Vector2.Distance(transform.position, target.position) <= chaseRange)
        {
            // 2. En rango de visión: Perseguir
            isChasing = true;
            MoveTowardsPlayer();
            animator.SetBool("IsWalking", true);
            playerInVision = true;
        }
        else
        {
            // 3. Fuera de rango: Parar
            isChasing = false;
            animator.SetBool("IsWalking", false);
            playerInVision = false;
        }

        if (playerInVision)
        {
            FlipTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void TryAttack()
    {
        if (isAttacking) return;
        StartCoroutine("PerformAttack");
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackHitDelay);

        if (!isAttacking)
        {
            yield break;
        }

        // Usamos "attackRange" para el golpe
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("IA golpeó a " + player.name + " (Ataque Frontal)");
            player.GetComponent<PlayerHealth>()?.TakeDamage(Mathf.RoundToInt(attackDamage));
        }

        yield return new WaitForSeconds(attackCooldown - attackHitDelay);
        isAttacking = false;
    }

    public void CancelAttack()
    {
        if (isAttacking)
        {
            StopCoroutine("PerformAttack");
            isAttacking = false;
            Debug.Log("¡Ataque del enemigo CANCELADO por daño!");
        }
    }

    void FlipTowardsPlayer()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        if (direction.x > 0)
            spriteRenderer.flipX = true;
        else if (direction.x < 0)
            spriteRenderer.flipX = false;
    }

    // ¡GIZMOS SIMPLIFICADOS!
    private void OnDrawGizmosSelected()
    {
        // Rango de visión (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Rango de Ataque (Rojo)
        // (Se dibuja en el AttackPoint)
        Gizmos.color = Color.red;
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}