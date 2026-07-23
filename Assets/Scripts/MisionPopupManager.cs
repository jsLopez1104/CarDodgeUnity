using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;

public class MisionPopupManager : MonoBehaviour
{
    public static MisionPopupManager instancia;

    [Header("UI")]
    public GameObject panelPopup;          // Panel raíz del popup
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoRecompensa;
    public CanvasGroup canvasGroup;
    public RectTransform rectPopup;        // RectTransform del propio panelPopup

    [Header("Tiempos")]
    public float tiempoVisible = 2.5f;
    public float duracionAnimacion = 0.35f;

    [Header("Posiciones (offset en Y desde su posición ancla)")]
    public float offsetOculto = 80f;   // cuánto sube/se esconde arriba
    public float offsetVisible = 0f;   // posición final visible

    private Queue<MisionCompletada> cola = new Queue<MisionCompletada>();
    private bool mostrando = false;
    private Vector2 posBase;

    void Awake()
    {
        instancia = this;
        if (rectPopup != null)
            posBase = rectPopup.anchoredPosition;
        if (panelPopup != null)
            panelPopup.SetActive(false);
    }

    public void MostrarMisiones(MisionCompletada[] misiones)
    {
        foreach (var m in misiones)
            cola.Enqueue(m);

        if (!mostrando)
            StartCoroutine(ProcesarCola());
    }

    IEnumerator ProcesarCola()
    {
        mostrando = true;

        while (cola.Count > 0)
        {
            MisionCompletada mision = cola.Dequeue();

            textoDescripcion.text = "¡Misión completada! " + mision.descripcion;
            textoRecompensa.text = "+" + mision.recompensa + " monedas";

            panelPopup.SetActive(true);

            // Posición inicial: arriba y oculto
            rectPopup.anchoredPosition = posBase + new Vector2(0f, offsetOculto);
            canvasGroup.alpha = 0f;

            yield return AnimarEntrada();

            yield return new WaitForSecondsRealtime(tiempoVisible);

            yield return AnimarSalida();

            panelPopup.SetActive(false);
        }

        mostrando = false;
    }

    IEnumerator AnimarEntrada()
    {
        float t = 0f;
        Vector2 desde = posBase + new Vector2(0f, offsetOculto);
        Vector2 hasta = posBase + new Vector2(0f, offsetVisible);

        while (t < duracionAnimacion)
        {
            t += Time.unscaledDeltaTime;
            float progreso = EaseOutBack(t / duracionAnimacion);
            rectPopup.anchoredPosition = Vector2.LerpUnclamped(desde, hasta, progreso);
            canvasGroup.alpha = Mathf.Clamp01(t / duracionAnimacion);
            yield return null;
        }

        rectPopup.anchoredPosition = hasta;
        canvasGroup.alpha = 1f;
    }

    IEnumerator AnimarSalida()
    {
        float t = 0f;
        Vector2 desde = posBase + new Vector2(0f, offsetVisible);
        Vector2 hasta = posBase + new Vector2(0f, offsetOculto);

        while (t < duracionAnimacion)
        {
            t += Time.unscaledDeltaTime;
            float progreso = t / duracionAnimacion;
            rectPopup.anchoredPosition = Vector2.Lerp(desde, hasta, progreso);
            canvasGroup.alpha = 1f - progreso;
            yield return null;
        }

        rectPopup.anchoredPosition = hasta;
        canvasGroup.alpha = 0f;
    }

    // Easing con un pequeño "rebote" al entrar, más vistoso que lineal
    float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}