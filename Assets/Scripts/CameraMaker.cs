using UnityEngine;

public class CameraMaker : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Cubo verde de 10 unidades
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, Vector3.one * 10f);

        // Línea roja hacia adelante
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.forward * 15f
        );
    }
}
