using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instancia;

    private Vector3 posicionOriginal;

    void Awake()
    {
        instancia = this;
        posicionOriginal = transform.localPosition;
        Debug.Log("CameraShake iniciado en: " + gameObject.name);
    }

    public void Shake(float duracion, float magnitud)
    {
        StartCoroutine(ShakeCoroutine(duracion, magnitud));
    }

    IEnumerator ShakeCoroutine(float duracion, float magnitud)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float x = Random.Range(-1f, 1f) * magnitud;
            float y = Random.Range(-1f, 1f) * magnitud;

            transform.localPosition = new Vector3(
                posicionOriginal.x + x,
                posicionOriginal.y + y,
                posicionOriginal.z
            );

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = posicionOriginal;
    }

    public IEnumerator ZoomAlMorir(Transform objetivo)
    {
        Vector3 posicionInicial = transform.position;
        Vector3 posicionObjetivo = objetivo.position + new Vector3(0, 2f, -3f);
        float duracionZoom = 0.8f;
        float tiempo = 0f;

        // Primero hacer zoom hacia el carro
        while (tiempo < duracionZoom)
        {
            tiempo += Time.unscaledDeltaTime;
            float t = tiempo / duracionZoom;
            transform.position = Vector3.Lerp(posicionInicial, posicionObjetivo, t);
            transform.LookAt(objetivo.position);
            yield return null;
        }

        // Luego orbitar alrededor del carro indefinidamente
        float angulo = 0f;
        float radio = 3f;
        float altura = 2f;
        float velocidadOrbita = 60f;

        while (!GameManager.instancia.juegoActivo)
        {
            angulo += velocidadOrbita * Time.unscaledDeltaTime;
            float x = objetivo.position.x + Mathf.Sin(angulo * Mathf.Deg2Rad) * radio;
            float z = objetivo.position.z + Mathf.Cos(angulo * Mathf.Deg2Rad) * radio;
            transform.position = new Vector3(x, objetivo.position.y + altura, z);
            transform.LookAt(objetivo.position);
            yield return null;
        }

        // Al reiniciar, volver a posición original
        transform.position = posicionInicial;
        transform.rotation = Quaternion.Euler(25f, 0f, 0f);
    }
}