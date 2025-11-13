// ¡SCRIPT ACTUALIZADO!
// Ya no usa PlayOneShot. Ahora "interrumpe" el audio.
using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;

    [Header("Tiempos de Ataque")]
    public float attackCooldown = 1.0f;
    public float attackHitDelay = 0.5f;

    [Header("Detección")]
    public LayerMask enemyLayer;

    [Header("Audio (SFX)")]
    public AudioSource sfxAudioSource;
    public AudioClip attackSound;

    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("PlayerAttack no pudo encontrar el componente Animator.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        // 1. ¡REPRODUCIR SONIDO (CON INTERRUPCIÓN)!
        if (sfxAudioSource != null && attackSound != null)
        {
            // Detenemos CUALQUIER sonido que estuviera antes
            sfxAudioSource.Stop();
            // Asignamos el clip de ataque
            sfxAudioSource.clip = attackSound;
            // Lo reproducimos
            sfxAudioSource.Play();
        }

        // 2. Activar la animación de ataque
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 3. Esperar el delay
        yield return new WaitForSeconds(attackHitDelay);

        // 4. Detectar enemigos (el código de daño)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies.Length > 0)
        {
            Debug.Log($"¡Golpe ejecutado (con delay)! {hitEnemies.Length} enemigos en rango.");
        }

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

        // 5. Esperar el cooldown
        float remainingTime = attackCooldown - attackHitDelay;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}