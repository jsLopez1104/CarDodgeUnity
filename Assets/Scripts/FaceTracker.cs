using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FaceTracker : MonoBehaviour
{
    public static FaceTracker instancia;

    [Header("Configuración")]
    public float intervaloDeteccion = 0.05f;

    public float yawActual { get; private set; }
    public float inputCabeza { get; private set; }

    private float timer = 0f;
    private bool procesando = false;
    private bool servidorListo = false;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        StartCoroutine(EsperarServidor());
    }

    IEnumerator EsperarServidor()
    {
        while (!servidorListo)
        {
            UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:5000/face");
            request.timeout = 1;
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
                servidorListo = true;
            else
                yield return new WaitForSeconds(0.5f);
        }

        // Enviar sensibilidad al servidor
        float sensibilidad = PlayerPrefs.GetFloat("sensibilidad", 5f);
        float umbral = Mathf.Lerp(25f, 1f, (sensibilidad - 1f) / 9f);

        string json = "{\"umbral\": " + umbral.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "}";
        UnityWebRequest reqUmbral = new UnityWebRequest("http://127.0.0.1:5000/set_umbral", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        reqUmbral.uploadHandler = new UploadHandlerRaw(body);
        reqUmbral.downloadHandler = new DownloadHandlerBuffer();
        reqUmbral.SetRequestHeader("Content-Type", "application/json");
        yield return reqUmbral.SendWebRequest();
    }

    void Update()
    {
        if (!servidorListo) return;

        timer += Time.deltaTime;
        if (timer >= intervaloDeteccion && !procesando)
        {
            timer = 0f;
            StartCoroutine(ConsultarServidor());
        }
    }

    IEnumerator ConsultarServidor()
    {
        procesando = true;

        UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:5000/face");
        request.timeout = 1;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            ProcesarRespuesta(request.downloadHandler.text);

        procesando = false;
    }

    void ProcesarRespuesta(string json)
    {
        int yawIndex = json.IndexOf("\"yaw\":");
        if (yawIndex == -1) return;

        string sub = json.Substring(yawIndex + 6);
        int end = sub.IndexOfAny(new char[] { ',', '}' });
        string yawStr = sub.Substring(0, end).Trim();

        int inputIndex = json.IndexOf("\"input\":");
        string inputStr = "0";
        if (inputIndex != -1)
        {
            string sub2 = json.Substring(inputIndex + 8);
            int end2 = sub2.IndexOfAny(new char[] { ',', '}' });
            inputStr = sub2.Substring(0, end2).Trim();
        }

        if (float.TryParse(yawStr, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out float yaw))
            yawActual = yaw;

        if (float.TryParse(inputStr, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out float input))
            inputCabeza = input;
    }
}