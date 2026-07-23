using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class PanelAnimator : MonoBehaviour
{
    [Header("Configuración")]
    public float duracion = 0.25f;
    public float escalaInicial = 0.85f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 escalaObjetivo;
    private Coroutine corrutinaActual;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        escalaObjetivo = rectTransform.localScale;
    }

    public void Mostrar()
    {
        gameObject.SetActive(true);

        if (corrutinaActual != null)
            StopCoroutine(corrutinaActual);
        corrutinaActual = StartCoroutine(Animar(true));
    }

    public void Ocultar()
    {
        if (corrutinaActual != null)
            StopCoroutine(corrutinaActual);
        corrutinaActual = StartCoroutine(Animar(false));
    }

    IEnumerator Animar(bool mostrando)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float alphaDesde = mostrando ? 0f : 1f;
        float alphaHasta = mostrando ? 1f : 0f;
        Vector3 escalaDesde = mostrando ? escalaObjetivo * escalaInicial : escalaObjetivo;
        Vector3 escalaHasta = mostrando ? escalaObjetivo : escalaObjetivo * escalaInicial;

        canvasGroup.alpha = alphaDesde;
        rectTransform.localScale = escalaDesde;

        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            float progreso = t / duracion;
            float progresoEase = mostrando ? EaseOutBack(progreso) : progreso;

            canvasGroup.alpha = Mathf.Lerp(alphaDesde, alphaHasta, progreso);
            rectTransform.localScale = Vector3.LerpUnclamped(escalaDesde, escalaHasta, progresoEase);

            yield return null;
        }

        canvasGroup.alpha = alphaHasta;
        rectTransform.localScale = escalaHasta;

        if (mostrando)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}