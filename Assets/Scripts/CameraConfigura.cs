using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraConfigura : MonoBehaviour
{
    [Header("Camera Configuration")]
    public Camera camara;

    [Header("Projection")]
    public Button botonProyeccion;
    public TMP_Text textoProyeccion;

    [Header("Perspective")]
    public GameObject grupoPerspective;
    public Slider sliderFOV;
    public TMP_Text textoFOV;

    [Header("Orthographic")]
    public GameObject grupoOrthographic;
    public Slider sliderSize;
    public TMP_Text textoSize;

    [Header("Clipping Planes")]
    public Slider sliderNear;
    public TMP_Text textoNear;
    public Slider sliderFar;
    public TMP_Text textoFar;

    private float fovInicial;
    private float sizeInicial;
    private float nearInicial;
    private float farInicial;

    void Start()
    {
        if (camara == null)
        {
            camara = GetComponent<Camera>();
        }

        fovInicial = camara.fieldOfView;
        sizeInicial = camara.orthographicSize;
        nearInicial = camara.nearClipPlane;
        farInicial = camara.farClipPlane;

        sliderFOV.minValue = 1;
        sliderFOV.maxValue = 180;
        sliderFOV.value = fovInicial;

        sliderSize.minValue = 0.1f;
        sliderSize.maxValue = 100;
        sliderSize.value = sizeInicial;

        sliderNear.minValue = 0.01f;
        sliderNear.maxValue = 100;
        sliderNear.value = nearInicial;

        sliderFar.minValue = 1;
        sliderFar.maxValue = 1000;
        sliderFar.value = farInicial;

        sliderFOV.onValueChanged.AddListener(CambiarFOV);
        sliderSize.onValueChanged.AddListener(CambiarSize);
        sliderNear.onValueChanged.AddListener(CambiarNear);
        sliderFar.onValueChanged.AddListener(CambiarFar);

        botonProyeccion.onClick.AddListener(CambiarProyeccion);

        ActualizarInterfaz();
    }

    void CambiarProyeccion()
    {
        camara.orthographic = !camara.orthographic;
        ActualizarInterfaz();
    }

    void CambiarFOV(float valor)
    {
        camara.fieldOfView = valor;
        textoFOV.text = "FOV: " + valor.ToString("F0");
    }

    void CambiarSize(float valor)
    {
        camara.orthographicSize = valor;
        textoSize.text = "Size: " + valor.ToString("F1");
    }

    void CambiarNear(float valor)
    {
        camara.nearClipPlane = valor;

        if (camara.farClipPlane <= valor)
        {
            camara.farClipPlane = Mathf.Min(valor + 0.01f, 1000);
            sliderFar.SetValueWithoutNotify(camara.farClipPlane);
            textoFar.text = "Far: " + camara.farClipPlane.ToString("F1");
        }

        textoNear.text = "Near: " + valor.ToString("F2");
    }

    void CambiarFar(float valor)
    {
        if (valor <= camara.nearClipPlane)
        {
            valor = Mathf.Min(camara.nearClipPlane + 0.01f, 1000);
            sliderFar.SetValueWithoutNotify(valor);
        }

        camara.farClipPlane = valor;
        textoFar.text = "Far: " + valor.ToString("F1");
    }

    void ActualizarInterfaz()
    {
        bool perspectiva = !camara.orthographic;

        grupoPerspective.SetActive(perspectiva);
        grupoOrthographic.SetActive(!perspectiva);

        textoProyeccion.text = perspectiva
            ? "Proyección: Perspective"
            : "Proyección: Orthographic";

        textoFOV.text = "FOV: " + camara.fieldOfView.ToString("F0");
        textoSize.text = "Size: " + camara.orthographicSize.ToString("F1");
        textoNear.text = "Near: " + camara.nearClipPlane.ToString("F2");
        textoFar.text = "Far: " + camara.farClipPlane.ToString("F1");
    }

    void OnDrawGizmos()
    {
        Camera cam;

        if (camara != null)
        {
            cam = camara;
        }
        else
        {
            cam = GetComponent<Camera>();
        }

        if (cam == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.matrix = cam.transform.localToWorldMatrix;

        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;
        float aspect = cam.aspect;

        if (cam.orthographic)
        {
            DibujarRectangulo(near, cam.orthographicSize, aspect);
            DibujarRectangulo(far, cam.orthographicSize, aspect);
        }
        else
        {
            float alturaNear = 2f * near *
                Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

            float alturaFar = 2f * far *
                Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

            DibujarRectangulo(near, alturaNear / 2f, aspect);
            DibujarRectangulo(far, alturaFar / 2f, aspect);
        }

        Vector3[] esquinasNear = ObtenerEsquinas(cam, near);
        Vector3[] esquinasFar = ObtenerEsquinas(cam, far);

        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(esquinasNear[i], esquinasFar[i]); 
        }
        // Dibujar la esfera y la línea roja una sola vez
        DibujarGizmoCamara();
    }

    void DibujarRectangulo(float distancia, float mitadAltura, float aspect)
    {
        float mitadAncho = mitadAltura * aspect;

        Vector3 arribaIzquierda = new Vector3(-mitadAncho, mitadAltura, distancia);
        Vector3 arribaDerecha = new Vector3(mitadAncho, mitadAltura, distancia);
        Vector3 abajoIzquierda = new Vector3(-mitadAncho, -mitadAltura, distancia);
        Vector3 abajoDerecha = new Vector3(mitadAncho, -mitadAltura, distancia);

        Gizmos.DrawLine(arribaIzquierda, arribaDerecha);
        Gizmos.DrawLine(arribaDerecha, abajoDerecha);
        Gizmos.DrawLine(abajoDerecha, abajoIzquierda);
        Gizmos.DrawLine(abajoIzquierda, arribaIzquierda);
    }

    Vector3[] ObtenerEsquinas(Camera cam, float distancia)
    {
        float mitadAltura;
        float mitadAncho;

        if (cam.orthographic)
        {
            mitadAltura = cam.orthographicSize;
        }
        else
        {
            mitadAltura = distancia *
                Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        mitadAncho = mitadAltura * cam.aspect;

        return new Vector3[]
        {
            new Vector3(-mitadAncho, mitadAltura, distancia),
            new Vector3(mitadAncho, mitadAltura, distancia),
            new Vector3(mitadAncho, -mitadAltura, distancia),
            new Vector3(-mitadAncho, -mitadAltura, distancia)
        };
    }

    void DibujarEdificio()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawCube(
            transform.position,
            transform.lossyScale
        );
    }

    void DibujarGizmoCamara()
    {
        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = Color.red;

        // Esfera roja de radio 10 en el centro de Main Camera
        Gizmos.DrawSphere(transform.position, 10f);

        // Línea roja hacia adelante
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.forward * 30f
        );
    }

}
