using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;  // El objeto a seguir (tu personaje)
    public float smoothSpeed = 0.125f; // Velocidad del seguimiento
    public Vector3 offset; // Para ajustar la posición de la cámara (opcional)

    void LateUpdate()
    {
        if (target == null) return; // Evita errores si no hay target

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
