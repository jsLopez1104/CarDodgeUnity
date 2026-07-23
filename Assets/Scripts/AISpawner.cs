using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AISpawner : MonoBehaviour
{
    [Header("Obstáculo")]
    public GameObject prefabObstaculo;

    [Header("Configuración")]
    public float intervaloInicial = 2f;
    public float spawnZ = 20f;

    private float timer = 0f;
    private float intervaloActual = 2f;
    private bool consultando = false;
    private float posicionSiguiente = 0f;
    private bool doble = false;

    [Header("Obstáculos por mapa")]
    public GameObject[] prefabsObstaculo;

    void Start()
    {
        intervaloActual = intervaloInicial;
        StartCoroutine(ConsultarIA());
    }

    void Update()
    {
        if (!GameManager.instancia.juegoActivo) return;

        timer += Time.deltaTime;

        if (timer >= intervaloActual)
        {
            Spawnear();
            timer = 0f;

            if (!consultando)
                StartCoroutine(ConsultarIA());
        }
    }

    void Spawnear()
    {
        SpawnObstaculo(posicionSiguiente);

        if (doble)
        {
            float otraPos = posicionSiguiente == 0f ?
                (Random.value > 0.5f ? -1.5f : 1.5f) :
                (posicionSiguiente > 0 ? -1.5f : 1.5f);
            SpawnObstaculo(otraPos);
        }
    }

    void SpawnObstaculo(float x)
    {
        Vector3 pos = new Vector3(x, 0f, 40f);
        int mapa = PlayerPrefs.GetInt("mapaSeleccionado", 0);
        GameObject prefab = prefabsObstaculo[mapa] != null ? prefabsObstaculo[mapa] : prefabObstaculo;
        GameObject obstaculo = Instantiate(prefab, pos, Quaternion.identity);

        ObstacleMove move = obstaculo.GetComponent<ObstacleMove>();
        if (move == null)
            move = obstaculo.AddComponent<ObstacleMove>();

        move.velocidad = GameManager.instancia.velocidadActual;
        Destroy(obstaculo, 10f);
    }

    IEnumerator ConsultarIA()
    {
        consultando = true;

        string json = JsonUtility.ToJson(new AIRequest
        {
            ladoPreferido = PlayerMetrics.instancia.LadoPreferido(),
            tiempoVivo = PlayerMetrics.instancia.tiempoVivo,
            posicionActual = PlayerMetrics.instancia.posicionNormalizada,
            velocidad = GameManager.instancia.velocidadActual
        });

        UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:5000/ai", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 1;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            AIResponse respuesta = JsonUtility.FromJson<AIResponse>(request.downloadHandler.text);
            posicionSiguiente = respuesta.posicionX;
            intervaloActual = respuesta.intervalo;
            doble = respuesta.doble;
        }

        consultando = false;
    }

    [System.Serializable]
    public class AIRequest
    {
        public float ladoPreferido;
        public float tiempoVivo;
        public float posicionActual;
        public float velocidad;
    }

    [System.Serializable]
    public class AIResponse
    {
        public float posicionX;
        public float intervalo;
        public bool doble;
        public float dificultad;
    }
}