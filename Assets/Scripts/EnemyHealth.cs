using UnityEngine;
using System.Collections; // Para la corrutina de parpadeo

/// <summary>
/// Gestiona la salud de un enemigo.
/// Debe ir en el prefab o el objeto del Enemigo.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 3;   // Salud máxima
    private int currentHealth;  // Salud actual

    [Header("Animación de Muerte")]
    [Tooltip("El tiempo que dura la animación de 'morir' antes de destruir el objeto")]
    public float deathAnimationTime = 1.0f;
    
    private Animator animator;  // El componente Animator
    private SpriteRenderer spriteRenderer; // Para el parpadeo
    public bool isDead = false; // Para evitar que muera varias veces

    // Start se llama antes del primer frame
    void Start()
    {
        // Al empezar, el enemigo tiene la salud máxima
        currentHealth = maxHealth;

        // Obtenemos los componentes que usaremos
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Función pública que otros scripts (como PlayerAttack) pueden llamar.
    /// </summary>
    /// <param name="damage">La cantidad de daño recibido</param>
    public void TakeDamage(int damage)
    {
        // Si ya estamos muertos, no hacemos nada más
        if (isDead)
        {
            return; 
        }

        // 1. Restamos el daño a la salud actual
        currentHealth -= damage;

        Debug.Log($"Enemigo '{gameObject.name}' recibe {damage} de daño. Salud restante: {currentHealth}");

        // 2. Comprobar si el enemigo ha muerto
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 3. Si no ha muerto, activar la animación de "recibir golpe"
            if (animator != null)
            {
                // 'TakeHit' debe ser el nombre del Trigger que crees en el Animator
                animator.SetTrigger("TakeHit");
            }

            // ...y el efecto de "parpadeo"
            StartCoroutine(FlashEffect());
        }
    }

    /// <summary>
    /// Se llama cuando la salud del enemigo llega a 0.
    /// </summary>
    void Die()
    {
        isDead = true; // Marcamos que estamos muertos

        Debug.Log($"Enemigo '{gameObject.name}' ha muerto.");

        // 1. Activar animación de muerte (si existe un Animator)
        if (animator != null)
        {
            // 'death' debe ser el nombre del Trigger que crees en el Animator
            animator.SetTrigger("death"); 
        }

        // 2. Desactivar colisiones para que no nos sigan golpeando
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        
        // 3. Desactivar físicas (si las tuviera)
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }

        // 4. Opcional: Desactivar el script de IA para que deje de moverse
        // (Si tienes un script llamado 'EnemyAI', por ejemplo)
        // EnemyAI aiScript = GetComponent<EnemyAI>();
        // if (aiScript != null)
        // {
        //     aiScript.enabled = false;
        // }

        // 5. Destruir el objeto DESPUÉS de que la animación haya terminado
        Destroy(gameObject, deathAnimationTime);
    }

    /// <summary>
    /// Corrutina para el parpadeo al recibir daño
    /// </summary>
    private IEnumerator FlashEffect()
    {
        if (spriteRenderer != null)
        {
            // Cambia a un color rojo semitransparente o el que prefieras
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f, 1f); 
            
            // Espera un momento
            yield return new WaitForSeconds(0.1f); 
            
            // Vuelve al color original (blanco)
            spriteRenderer.color = Color.white; 
        }
    }
}

