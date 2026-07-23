using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class StatsPanelManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelEstadisticas;
    public CanvasGroup fondoCanvasGroup;   // CanvasGroup de PanelEstadisticas
    public RectTransform cardTransform;    // RectTransform de Card
    public CanvasGroup cardCanvasGroup;    // CanvasGroup de Card
    public Button botonEstadisticas;
    public Button botonCerrarEstadisticas;

    public TextMeshProUGUI textoMejorPuntaje;
    public TextMeshProUGUI textoPartidas;
    public TextMeshProUGUI textoMonedasActuales;
    public TextMeshProUGUI textoMonedasTotales;

    [Header("Animación")]
    public float duracionAnimacion = 0.3f;

    private string userId;
    private Vector3 escalaOriginal;
    private bool animando = false;

    [System.Serializable]
    private class EstadisticasResponse
    {
        public bool success;
        public int mejor_puntaje_global;
        public int partidas_jugadas;
        public int monedas;
        public int monedas_totales_ganadas;
    }

    void Start()
    {
        userId = PlayerPrefs.GetString("userId", "anonimo");
        escalaOriginal = cardTransform.localScale;

        panelEstadisticas.SetActive(false);
        botonEstadisticas.onClick.AddListener(AbrirEstadisticas);
        botonCerrarEstadisticas.onClick.AddListener(CerrarEstadisticas);
    }

    public void AbrirEstadisticas()
    {
        if (animando) return;
        panelEstadisticas.SetActive(true);
        StartCoroutine(CargarEstadisticas());
        StartCoroutine(AnimarEntrada());
    }

    public void CerrarEstadisticas()
    {
        if (animando) return;
        StartCoroutine(AnimarSalida());
    }

    IEnumerator AnimarEntrada()
    {
        animando = true;

        fondoCanvasGroup.alpha = 0f;
        cardCanvasGroup.alpha = 0f;
        cardCanvasGroup.interactable = false;
        cardCanvasGroup.blocksRaycasts = false;
        cardTransform.localScale = escalaOriginal * 0.8f;

        float t = 0f;
        while (t < duracionAnimacion)
        {
            t += Time.unscaledDeltaTime;
            float progreso = t / duracionAnimacion;

            fondoCanvasGroup.alpha = Mathf.Clamp01(progreso * 1.5f); // fondo aparece un poco más rápido
            cardCanvasGroup.alpha = Mathf.Clamp01(progreso);
            cardTransform.localScale = Vector3.LerpUnclamped(
                escalaOriginal * 0.8f, escalaOriginal, EaseOutBack(progreso)
            );

            yield return null;
        }

        fondoCanvasGroup.alpha = 1f;
        cardCanvasGroup.alpha = 1f;
        cardTransform.localScale = escalaOriginal;
        cardCanvasGroup.interactable = true;
        cardCanvasGroup.blocksRaycasts = true;

        animando = false;
    }

    IEnumerator AnimarSalida()
    {
        animando = true;
        cardCanvasGroup.interactable = false;
        cardCanvasGroup.blocksRaycasts = false;

        float t = 0f;
        while (t < duracionAnimacion)
        {
            t += Time.unscaledDeltaTime;
            float progreso = t / duracionAnimacion;

            fondoCanvasGroup.alpha = 1f - progreso;
            cardCanvasGroup.alpha = 1f - progreso;
            cardTransform.localScale = Vector3.Lerp(escalaOriginal, escalaOriginal * 0.8f, progreso);

            yield return null;
        }

        fondoCanvasGroup.alpha = 0f;
        cardCanvasGroup.alpha = 0f;
        cardTransform.localScale = escalaOriginal;
        panelEstadisticas.SetActive(false);

        animando = false;
    }

    float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    IEnumerator CargarEstadisticas()
    {
        UnityWebRequest request = UnityWebRequest.Get($"http://127.0.0.1:5000/get_estadisticas?userId={userId}");
        request.timeout = 5;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            EstadisticasResponse r = JsonUtility.FromJson<EstadisticasResponse>(request.downloadHandler.text);
            if (r != null && r.success)
            {
                textoMejorPuntaje.text = r.mejor_puntaje_global.ToString("N0");
                textoPartidas.text = r.partidas_jugadas.ToString("N0");
                textoMonedasActuales.text = r.monedas.ToString("N0");
                textoMonedasTotales.text = r.monedas_totales_ganadas.ToString("N0");
            }
        }
        else
        {
            textoMejorPuntaje.text = "Error";
        }
    }
}