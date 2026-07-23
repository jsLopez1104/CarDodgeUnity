using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadLateral = 5f;
    public float suavizado = 8f;
    public float limiteX = 2f;

    [Header("UI")]
    public TMPro.TextMeshProUGUI textoConexion;

    private float inputObjetivo = 0f;
    private float inputActual = 0f;
    private float tiempoSinRespuesta = 0f;
    private float umbralDesconexion = 2f;

    void Update()
    {
        if (!GameManager.instancia.juegoActivo) return;

        // Input de teclado
        float inputTeclado = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            inputTeclado = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            inputTeclado = 1f;

        // Input de cabeza
        float inputCabeza = 0f;
        if (FaceTracker.instancia != null)
            inputCabeza = FaceTracker.instancia.inputCabeza;

        // Detectar si el servidor sigue respondiendo
        if (FaceTracker.instancia != null && FaceTracker.instancia.yawActual == 0f)
            tiempoSinRespuesta += Time.deltaTime;
        else
            tiempoSinRespuesta = 0f;

        // Mostrar aviso si se perdió conexión
        if (textoConexion != null)
        {
            if (tiempoSinRespuesta > umbralDesconexion)
                textoConexion.text = "Sin conexión con cámara";
            else
                textoConexion.text = "";
        }

        float inputCabezaProporcional = 0f;
        if (FaceTracker.instancia != null)
        {
            float yaw = FaceTracker.instancia.yawActual;
            float umbral = 5f;
            float sensibilidad = PlayerPrefs.GetFloat("sensibilidad", 5f);
            float maxYaw = Mathf.Lerp(35f, 10f, (sensibilidad - 1f) / 9f);

            if (Mathf.Abs(yaw) > umbral)
            {
                float factor = Mathf.Clamp((Mathf.Abs(yaw) - umbral) / (maxYaw - umbral), 0f, 1f);
                inputCabezaProporcional = -Mathf.Sign(yaw) * factor;
            }
        }

        inputObjetivo = inputTeclado != 0f ? inputTeclado : inputCabezaProporcional;
        inputActual = Mathf.Lerp(inputActual, inputObjetivo, Time.deltaTime * suavizado);

        Vector3 movimiento = new Vector3(inputActual * velocidadLateral * Time.deltaTime, 0, 0);
        transform.Translate(movimiento);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -limiteX, limiteX);
        transform.position = pos;

        // Inclinar el carro al girar
        Transform modeloCarro = transform.GetChild(0);
        if (modeloCarro != null)
        {
            float inclinacionObjetivo = -inputActual * 15f;
            Vector3 rotacion = modeloCarro.localEulerAngles;
            rotacion.z = Mathf.LerpAngle(rotacion.z, inclinacionObjetivo, Time.deltaTime * 5f);
            modeloCarro.localEulerAngles = rotacion;
        }
    }
}