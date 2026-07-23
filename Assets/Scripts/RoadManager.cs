using UnityEngine;

public class RoadManager : MonoBehaviour
{
    private Transform[] planos;
    private float largoPlano;

    void Start()
    {
        planos = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            planos[i] = transform.GetChild(i);

        largoPlano = planos[0].localScale.z * 10f;

        // Posicionar los planos correctamente al inicio
        for (int i = 0; i < planos.Length; i++)
        {
            planos[i].position = new Vector3(0, 0, i * largoPlano);
        }
    }

    void Update()
    {
        float velocidad = GameManager.instancia.velocidadActual;

        foreach (Transform plano in planos)
        {
            plano.Translate(0, 0, -velocidad * Time.deltaTime);

            if (plano.position.z <= -largoPlano)
            {
                float maxZ = ObtenerMaxZ();
                plano.position = new Vector3(plano.position.x, plano.position.y, maxZ + largoPlano);
            }
        }
    }

    float ObtenerMaxZ()
    {
        float max = float.MinValue;
        foreach (Transform plano in planos)
            if (plano.position.z > max)
                max = plano.position.z;
        return max;
    }
}