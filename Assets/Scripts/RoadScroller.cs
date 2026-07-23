using UnityEngine;

public class RoadScroller : MonoBehaviour
{
    [Header("Velocidad")]
    public float velocidad = 10f;

    [Header("Loop")]
    public float largoPlano = 100f;
    public Transform otroPlano;

    void Update()
    {
        transform.Translate(0, 0, -velocidad * Time.deltaTime);

        if (transform.position.z <= -largoPlano)
        {
            // Se posiciona justo detrás del otro plano
            float nuevaZ = otroPlano.position.z + largoPlano;
            transform.position = new Vector3(transform.position.x, transform.position.y, nuevaZ);
        }
    }
}