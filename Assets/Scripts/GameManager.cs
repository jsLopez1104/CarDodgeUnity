using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public LeaderboardManager leaderboardManager;
    public static GameManager instancia;

    [Header("UI")]
    public TextMeshProUGUI textoScore;
    public TextMeshProUGUI textoVelocidad;
    public GameObject panelGameOver;

    [Header("Configuración")]
    public float velocidadInicial = 10f;
    public float incrementoVelocidad = 0.5f;
    public float velocidadMaxima = 30f;

    private float score = 0f;
    public float velocidadActual { get; private set; }
    public float scoreActual { get { return score; } }
    public bool juegoActivo = true;

    [Header("HUD")]
    public TextMeshProUGUI textoMapaHUD;
    public TextMeshProUGUI textoNombreHUD;

    // Estadísticas para misiones
    public int esquivasTotales = 0;
    public float velocidadMaxima2 = 0f;

    [System.Serializable]
    public class MisionRequest
    {
        public string userId;
        public int score;
        public int tiempo;
        public int velocidadMax;
        public int esquivas;
        public int mapa;
    }

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        // Mostrar mapa y dificultad en HUD
        if (textoMapaHUD != null)
        {
            string[] nombresMapa = { "Ciudad", "Bosque", "Desierto", "Espacio", "Fantasy" };
            string[] nombresDificultad = { "Fácil", "Normal", "Difícil" };
            int mapaIndex = PlayerPrefs.GetInt("mapaSeleccionado", 0);
            int dificultadIndex = PlayerPrefs.GetInt("dificultadSeleccionada", 1);
            textoMapaHUD.text = nombresMapa[mapaIndex] + " · " + nombresDificultad[dificultadIndex];
        }

        if (textoNombreHUD != null)
            textoNombreHUD.text = PlayerPrefs.GetString("nombre", "Jugador");

        // Leer dificultad seleccionada
        int dificultad = PlayerPrefs.GetInt("dificultadSeleccionada", 1);

        switch (dificultad)
        {
            case 0:
                velocidadInicial = 8f;
                incrementoVelocidad = 0.2f;
                break;
            case 1:
                velocidadInicial = 12f;
                incrementoVelocidad = 0.5f;
                break;
            case 2:
                velocidadInicial = 18f;
                incrementoVelocidad = 0.8f;
                break;
        }

        velocidadActual = velocidadInicial;
        panelGameOver.SetActive(false);
    }

    void Update()
    {
        if (!juegoActivo) return;

        score += Time.deltaTime * 10f;
        velocidadActual += incrementoVelocidad * Time.deltaTime;
        velocidadActual = Mathf.Min(velocidadActual, velocidadMaxima);

        if (textoScore != null)
            textoScore.text = "Score: " + Mathf.FloorToInt(score).ToString("N0");
        if (textoVelocidad != null)
            textoVelocidad.text = "Velocidad: " + Mathf.FloorToInt(velocidadActual);
        if (velocidadActual > velocidadMaxima2)
            velocidadMaxima2 = velocidadActual;
    }

    public void GameOver()
    {
        if (!juegoActivo) return;
        juegoActivo = false;
        Time.timeScale = 0f;
        if (leaderboardManager != null)
            leaderboardManager.MostrarPuntaje(Mathf.FloorToInt(score));
        StartCoroutine(MostrarGameOver());
        StartCoroutine(GuardarPuntaje());
    }

    IEnumerator MostrarGameOver()
    {
        yield return new WaitForSecondsRealtime(1.2f);
        panelGameOver.SetActive(true);
    }

    IEnumerator GuardarPuntaje()
    {
        string userId = PlayerPrefs.GetString("userId", "anonimo");
        int mapa = PlayerPrefs.GetInt("mapaSeleccionado", 0);
        int dificultad = PlayerPrefs.GetInt("dificultadSeleccionada", 1);
        int scoreFinal = Mathf.FloorToInt(score);
        int monedasGanadas = scoreFinal / 10;

        string json = JsonUtility.ToJson(new PuntajeRequest
        {
            userId = userId,
            score = scoreFinal,
            tiempo = Mathf.FloorToInt(Time.timeSinceLevelLoad),
            mapa = mapa,
            dificultad = dificultad,
            monedasGanadas = monedasGanadas
        });

        UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:5000/guardar_puntaje", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 5;
        yield return request.SendWebRequest();

        // Actualizar misiones
        string jsonMisiones = JsonUtility.ToJson(new MisionRequest
        {
            userId = userId,
            score = scoreFinal,
            tiempo = Mathf.FloorToInt(Time.timeSinceLevelLoad),
            velocidadMax = Mathf.FloorToInt(velocidadMaxima2),
            esquivas = esquivasTotales,
            mapa = mapa
        });

        UnityWebRequest requestMisiones = new UnityWebRequest("http://127.0.0.1:5000/actualizar_misiones", "POST");
        byte[] bodyMisiones = System.Text.Encoding.UTF8.GetBytes(jsonMisiones);
        requestMisiones.uploadHandler = new UploadHandlerRaw(bodyMisiones);
        requestMisiones.downloadHandler = new DownloadHandlerBuffer();
        requestMisiones.SetRequestHeader("Content-Type", "application/json");
        requestMisiones.timeout = 5;
        yield return requestMisiones.SendWebRequest();

        if (leaderboardManager != null)
            leaderboardManager.MostrarMonedas(monedasGanadas);

        // Mostrar popup de misiones completadas (si hubo alguna)
        if (requestMisiones.result == UnityWebRequest.Result.Success)
        {
            MisionResponse respuestaMisiones = JsonUtility.FromJson<MisionResponse>(requestMisiones.downloadHandler.text);
            if (respuestaMisiones != null && respuestaMisiones.success
                && respuestaMisiones.misiones_completadas != null
                && respuestaMisiones.misiones_completadas.Length > 0
                && MisionPopupManager.instancia != null)
            {
                List<MisionCompletada> nuevas = new List<MisionCompletada>();
                foreach (var m in respuestaMisiones.misiones_completadas)
                {
                    bool yaMostrada = MisionTracker.instancia != null && MisionTracker.instancia.YaMostrada(m.id);
                    if (!yaMostrada)
                        nuevas.Add(m);
                }
                if (nuevas.Count > 0)
                    MisionPopupManager.instancia.MostrarMisiones(nuevas.ToArray());
            }
        }
    }

    [System.Serializable]
    public class PuntajeRequest
    {
        public string userId;
        public int score;
        public int tiempo;
        public int mapa;
        public int dificultad;
        public int monedasGanadas;
    }

    [System.Serializable]
    public class MisionCompletada
    {
        public string id;
        public string descripcion;
        public int recompensa;
    }

    [System.Serializable]
    public class MisionResponse
    {
        public bool success;
        public int monedas_ganadas;
        public MisionCompletada[] misiones_completadas;
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}