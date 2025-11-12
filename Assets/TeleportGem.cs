using UnityEngine;

public class TeleportGem : MonoBehaviour
{
    public Vector2 teleportPosition = new Vector2(189.5f, 11.54f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = teleportPosition;
                rb.linearVelocity = Vector2.zero; // reinicia velocidad para evitar bugs
            }
            else
            {
                other.transform.position = teleportPosition;
            }

            Debug.Log("�Teletransporte activado!");
        }
    }
}
