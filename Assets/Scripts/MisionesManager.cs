using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class MisionesManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelMisiones;
    public Button botonMisiones;
    public Button botonCerrarMisiones;

    [Header("Mision 1")]
    public TMP_Text textoMision1;
    public Slider barraMision1;
    public TMP_Text textoRecompensa1;

    [Header("Mision 2")]
    public TMP_Text textoMision2;
    public Slider barraMision2;
    public TMP_Text textoRecompensa2;

    [Header("Mision 3")]
    public TMP_Text textoMision3;
    public Slider barraMision3;
    public TMP_Text textoRecompensa3;

    [Header("Animación")]
    public PanelAnimator animadorMisiones;

    private string userId;

    void Start()
    {
        userId = PlayerPrefs.GetString("userId", "anonimo");
        panelMisiones.SetActive(false);
        botonMisiones.onClick.AddListener(AbrirMisiones);
        botonCerrarMisiones.onClick.AddListener(() => animadorMisiones.Ocultar());
    }

    public void AbrirMisiones()
    {
        animadorMisiones.Mostrar();
        StartCoroutine(CargarMisiones());
    }

    IEnumerator CargarMisiones()
    {
        textoMision1.text = "Cargando...";
        textoMision2.text = "";
        textoMision3.text = "";

        UnityWebRequest request = UnityWebRequest.Get($"http://127.0.0.1:5000/get_misiones?userId={userId}");
        request.timeout = 5;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            MisionesResponse respuesta = JsonUtility.FromJson<MisionesResponse>(request.downloadHandler.text);

            if (respuesta.success && respuesta.misiones != null && respuesta.misiones.Length > 0)
            {
                MostrarMision(respuesta.misiones[0], textoMision1, barraMision1, textoRecompensa1);
                if (respuesta.misiones.Length > 1)
                    MostrarMision(respuesta.misiones[1], textoMision2, barraMision2, textoRecompensa2);
                if (respuesta.misiones.Length > 2)
                    MostrarMision(respuesta.misiones[2], textoMision3, barraMision3, textoRecompensa3);
            }
        }
        else
        {
            textoMision1.text = "Error al cargar misiones";
        }
    }

    void MostrarMision(MisionData mision, TMP_Text texto, Slider barra, TMP_Text recompensa)
    {
        string estado = mision.completada ? " ✓" : $" ({mision.progreso}/{mision.meta})";
        texto.text = mision.descripcion + estado;
        texto.color = mision.completada ? new Color(0.306f, 0.804f, 0.769f) : Color.white;

        barra.maxValue = mision.meta;
        barra.value = mision.progreso;

        recompensa.text = "+" + mision.recompensa;
    }

    [System.Serializable]
    public class MisionesResponse
    {
        public bool success;
        public MisionData[] misiones;
    }

    [System.Serializable]
    public class MisionData
    {
        public string id;
        public string descripcion;
        public int meta;
        public int recompensa;
        public int progreso;
        public bool completada;
    }
}