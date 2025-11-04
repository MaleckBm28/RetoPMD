using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // El jugador
    public LayerMask playerLayer; // La capa del jugador
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private EnemyHealth health;

    [Header("Valores IA")]
    public float moveSpeed = 2f;
    public float chaseRange = 10f; // Rango para empezar a perseguir
    public float attackRange = 1.5f; // Rango para detenerse y atacar
    public float attackDamage = 10f;
    public float attackCooldown = 2f; // Tiempo entre ataques
    public float attackHitDelay = 0.5f; // Cuándo hace daño la animación

    private bool isAttacking = false;
    private bool isChasing = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        // Busca al jugador por su Tag si no está asignado
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    void Update()
    {
        if (health.isDead || target == null || isAttacking)
        {
            // Si estamos muertos, no tenemos jugador o estamos atacando, no hacemos nada.
            // PERO: Aseguramos que la animación de caminar esté apagada si no estamos persiguiendo o atacando.
            if (!isChasing || health.isDead)
            {
                animator.SetBool("IsWalking", false);
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer <= attackRange)
        {
            // 1. En rango de ataque: Detenerse y atacar
            isChasing = false;
            animator.SetBool("IsWalking", false); // ¡Importante! Dejar de caminar
            TryAttack();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // 2. En rango de persecución: Perseguir
            isChasing = true;
            MoveTowardsPlayer();
            animator.SetBool("IsWalking", true); // ¡Importante! Activar anim de caminar
        }
        else
        {
            // 3. Fuera de rango: No hacer nada
            isChasing = false;
            animator.SetBool("IsWalking", false); // ¡Importante! Dejar de caminar
        }

        // Girar siempre hacia el jugador si está persiguiendo
        if (isChasing)
        {
            FlipTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        // Creamos un punto de destino que solo usa la X del jugador, pero nuestra Y
        Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

        // Mueve al enemigo hacia ese punto
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void TryAttack()
    {
        if (isAttacking) return; // Ya estamos atacando

        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        // 1. Activa la animación de ataque
        animator.SetTrigger("Attack"); // El nombre debe coincidir EXACTO con el trigger en el Animator

        // 2. Espera el momento justo del golpe
        yield return new WaitForSeconds(attackHitDelay);

        // 3. Comprueba si el jugador está en rango para recibir el golpe
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer <= attackRange && !health.isDead) // Re-comprobamos por si se ha movido
        {
            // Dibuja un círculo para detectar al jugador
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);

            foreach (Collider2D player in hitPlayers)
            {
                // Si detectamos a alguien, le quitamos vida
                Debug.Log("IA golpeó a " + player.name);
                player.GetComponent<PlayerHealth>()?.TakeDamage(Mathf.RoundToInt(attackDamage));
            }
        }
        
        // 4. Espera el resto del cooldown
        yield return new WaitForSeconds(attackCooldown - attackHitDelay);
        isAttacking = false;
    }

    void FlipTowardsPlayer()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        // Gira el sprite
        // Asume que el sprite original mira a la DERECHA
        // Si tu sprite mira a la IZQUIERDA, invierte el true/false
        if (direction.x > 0)
            spriteRenderer.flipX = true; // Mirando a la derecha
        else if (direction.x < 0)
            spriteRenderer.flipX = false; // Mirando a la izquierda
    }

    // --- DIBUJAR GIZMOS ---
    // Esto dibuja los rangos en el editor para que puedas verlos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

