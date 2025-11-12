
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System; 

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Configuración de Vida (Corazones)")]
    public int numberOfHearts = 3; 
    private int maxHealth;         
    public int currentHealth;      

    public bool isDead = false;
    public static event Action<int, int> OnHealthChanged; 

    // --- Referencias de Componentes ---
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerMovement playerMovement; 

    void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>(); 

        maxHealth = numberOfHearts; 
        currentHealth = maxHealth;

        StartCoroutine(InitialHealthUpdate());
    }

    IEnumerator InitialHealthUpdate()
    {
        yield return null;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        StartCoroutine(BlinkEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Jugador curado {amount}. Vida actual: {currentHealth}");
    }

    // --- ¡¡NUEVA FUNCIÓN!! ---
    /// <summary>
    /// Llamado por EnemyHealth para aplicar el buff de velocidad.
    /// </summary>
    public void ApplySpeedBoost(float amount)
    {
        if (playerMovement != null)
        {
            playerMovement.IncreaseSpeed(amount);
        }
        else
        {
            Debug.LogError("¡PlayerHealth no pudo encontrar el script PlayerMovement!");
        }
    }
    // --- Fin de la Nueva Función ---


    void Die()
    {
        isDead = true;
        Time.timeScale = 0f; 
        SceneManager.LoadScene("Derrota");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    IEnumerator BlinkEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}