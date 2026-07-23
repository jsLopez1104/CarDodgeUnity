using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textoPuntaje;
    public TMP_Text textoLista;
    public GameObject panelLeaderboard;

    [Header("Botones")]
    public UnityEngine.UI.Button botonLeaderboard;
    public UnityEngine.UI.Button botonCerrar;
    public UnityEngine.UI.Button botonReiniciar;
    public UnityEngine.UI.Button botonMenuGameOver;

    [Header("Animación")]
    public PanelAnimator animadorLeaderboard;

    void Awake()
    {
        botonLeaderboard.onClick.AddListener(AbrirLeaderboard);
        botonCerrar.onClick.AddListener(CerrarLeaderboard);
        botonReiniciar.onClick.AddListener(
            () => { Time.timeScale = 1f; UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); }
        );
        botonMenuGameOver.onClick.AddListener(() => {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipalScene");
        });
    }

    public TMP_Text textoMonedas;

    public void MostrarMonedas(int monedas)
    {
        if (textoMonedas != null)
            textoMonedas.text = "+" + monedas + " monedas";
    }

    public void MostrarPuntaje(int score)
    {
        if (textoPuntaje != null)
            textoPuntaje.text = "Puntaje: " + score;
    }

    [Header("Elementos a ocultar")]
    public GameObject[] elementosGameOver;

    public void AbrirLeaderboard()
    {
        foreach (var e in elementosGameOver)
            e.SetActive(false);
        animadorLeaderboard.Mostrar();
        StartCoroutine(CargarLeaderboard());
    }

    public void CerrarLeaderboard()
    {
        animadorLeaderboard.Ocultar();
        foreach (var e in elementosGameOver)
            e.SetActive(true);
    }

    IEnumerator CargarLeaderboard()
    {
        textoLista.text = "Cargando...";

        int mapa = PlayerPrefs.GetInt("mapaSeleccionado", 0);
        int dificultad = PlayerPrefs.GetInt("dificultadSeleccionada", 1);

        string url = $"http://127.0.0.1:5000/leaderboard?mapa={mapa}&dificultad={dificultad}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 5;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LeaderboardResponse respuesta = JsonUtility.FromJson<LeaderboardResponse>(request.downloadHandler.text);

            if (respuesta.success && respuesta.leaderboard != null && respuesta.leaderboard.Length > 0)
            {
                string[] nombresMapa = { "Ciudad", "Bosque", "Desierto", "Espacio", "Fantasy" };
                string[] nombresDificultad = { "Fácil", "Normal", "Difícil" };

                string lista = nombresMapa[mapa] + " - " + nombresDificultad[dificultad] + "\n\n";

                for (int i = 0; i < respuesta.leaderboard.Length; i++)
                {
                    lista += (i + 1) + ". " + respuesta.leaderboard[i].nombre +
                             "  -  " + respuesta.leaderboard[i].score + "\n\n";
                }
                textoLista.text = lista;
            }
            else
            {
                textoLista.text = "No hay puntajes aún\npara este mapa y dificultad.";
            }
        }
        else
        {
            textoLista.text = "Error al cargar.";
        }
    }

    [System.Serializable]
    public class LeaderboardResponse
    {
        public bool success;
        public EntradaLeaderboard[] leaderboard;
    }

    [System.Serializable]
    public class EntradaLeaderboard
    {
        public string userId;
        public string nombre;
        public int score;
        public int tiempo;
    }
}