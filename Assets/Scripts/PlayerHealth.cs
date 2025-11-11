// ¡SCRIPT ACTUALIZADO!
// Le hemos añadido un 'Singleton' (Instance)
// para que el Enemigo pueda encontrarlo y curarlo.
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System; 

public class PlayerHealth : MonoBehaviour
{
    // --- ¡NUEVO SINGLETON! ---
    public static PlayerHealth Instance { get; private set; }
    // --- Fin del Singleton ---

    [Header("Configuración de Vida (Corazones)")]
    public int numberOfHearts = 3; 
    private int maxHealth;         
    public int currentHealth;      
    public bool isDead = false;

    public static event Action<int, int> OnHealthChanged; 
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // --- ¡NUEVA FUNCIÓN Awake! ---
    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            // Opcional: DontDestroyOnLoad(gameObject); 
            // (No lo pongas si no quieres que el jugador pase entre escenas)
        }
        else
        {
            Destroy(gameObject); // Si ya existe un jugador, destruye este
        }
    }
    // --- Fin de Awake ---

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // 1 corazón = 1 punto de vida
        maxHealth = numberOfHearts; // Sistema 1:1
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
        Debug.Log($"¡Jugador curado! Vida actual: {currentHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

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