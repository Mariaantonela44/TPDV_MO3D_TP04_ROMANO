using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [Header("Cámaras")]
    public Camera camaraPrincipal;
    public Camera camaraSecundaria;

    [Header("Marcadores")]
    public Transform[] marcadores;

    private int marcadorActual = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (camaraPrincipal != null)
        {
            camaraPrincipal.enabled = true;
        }

        if (camaraSecundaria != null)
        {
            camaraSecundaria.enabled = false;
        }

        // El marcador Sur debe estar en la posición 0
        if (marcadores.Length > 0 && camaraSecundaria != null)
        {
            camaraSecundaria.transform.position = marcadores[0].position;
            camaraSecundaria.transform.rotation = marcadores[0].rotation;
        }

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 180, 40), "Cambiar cámara"))
        {
            bool usarSecundaria = !camaraSecundaria.enabled;

            camaraPrincipal.enabled = !usarSecundaria;
            camaraSecundaria.enabled = usarSecundaria;
        }

        if (camaraSecundaria.enabled)
        {
            string[] nombres = { "SUR", "ESTE", "NORTE", "OESTE" };

            GUI.Label(
                new Rect(10, 60, 250, 40),
                "Cámara: " + nombres[marcadorActual],
                new GUIStyle(GUI.skin.label)
                {
                    fontSize = 24,
                    normal = { textColor = Color.white }
                }
            );

            if (GUI.Button(new Rect(10, 110, 180, 40), "Siguiente posición"))
            {
                CambiarPosicion();
            }
        }
    }

    void CambiarPosicion()
    {
        if (marcadores.Length == 0)
        {
            return;
        }

        marcadorActual = (marcadorActual + 1) % marcadores.Length;

        camaraSecundaria.transform.position = marcadores[marcadorActual].position;
        camaraSecundaria.transform.rotation = marcadores[marcadorActual].rotation;
    }
}
