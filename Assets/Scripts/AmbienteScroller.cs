using UnityEngine;

public class AmbienteScroller : MonoBehaviour
{
    public float largoAmbiente = 200f;
    private GameObject copia;
    private float posicionInicialZ;

    void Start()
    {
        posicionInicialZ = transform.position.z;

        copia = Instantiate(gameObject, transform.position + new Vector3(0, 0, largoAmbiente), transform.rotation);
        copia.transform.parent = transform.parent;
        Destroy(copia.GetComponent<AmbienteScroller>());
    }

    void Update()
    {
        if (GameManager.instancia == null || !GameManager.instancia.juegoActivo) return;

        float velocidad = GameManager.instancia.velocidadActual;

        transform.Translate(0, 0, -velocidad * Time.deltaTime, Space.World);
        if (copia != null)
            copia.transform.Translate(0, 0, -velocidad * Time.deltaTime, Space.World);

        // Cuando el original sale por detrás, lo ponemos delante de la copia
        if (transform.position.z <= posicionInicialZ - largoAmbiente)
        {
            float nuevaZ = copia != null ? copia.transform.position.z + largoAmbiente : posicionInicialZ + largoAmbiente;
            transform.position = new Vector3(transform.position.x, transform.position.y, nuevaZ);
        }

        // Cuando la copia sale por detrás, la ponemos delante del original
        if (copia != null && copia.transform.position.z <= posicionInicialZ - largoAmbiente)
        {
            float nuevaZ = transform.position.z + largoAmbiente;
            copia.transform.position = new Vector3(copia.transform.position.x, copia.transform.position.y, nuevaZ);
        }
    }
}