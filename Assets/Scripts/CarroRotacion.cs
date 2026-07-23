using UnityEngine;

public class CarroRotacion : MonoBehaviour
{
    public float velocidadRotacion = 45f;
    private Transform carroActual;

    void Update()
    {
        // Buscar el carro instanciado como hijo
        if (transform.childCount > 0)
        {
            carroActual = transform.GetChild(0);
            carroActual.Rotate(0, velocidadRotacion * Time.deltaTime, 0);
        }
    }
}