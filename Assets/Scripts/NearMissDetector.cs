using UnityEngine;
using UnityEngine.UI;

public class NearMissDetector : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaDeteccion = 1.5f;
    public Image flashRojo;
    private float alphaActual = 0f;

    void Update()
    {
        // Si el juego terminó, forzar el flash a apagarse (con tiempo real, no afectado por timeScale)
        if (!GameManager.instancia.juegoActivo)
        {
            alphaActual = Mathf.Lerp(alphaActual, 0f, Time.unscaledDeltaTime * 8f);
            AplicarAlpha();
            return;
        }

        alphaActual = Mathf.Lerp(alphaActual, 0f, Time.deltaTime * 5f);
        Collider[] cercanos = Physics.OverlapSphere(transform.position, distanciaDeteccion);
        foreach (Collider c in cercanos)
        {
            if (c.CompareTag("Obstacle"))
            {
                if (alphaActual < 0.1f) // Solo cuenta cuando el flash estaba apagado
                    GameManager.instancia.esquivasTotales++;
                alphaActual = 0.25f;
                if (AudioManager.instancia != null)
                    AudioManager.instancia.PlayWhoosh();
                break;
            }
        }

        AplicarAlpha();
    }

    void AplicarAlpha()
    {
        if (flashRojo != null)
        {
            Color color = flashRojo.color;
            color.a = alphaActual;
            flashRojo.color = color;
        }
    }
}