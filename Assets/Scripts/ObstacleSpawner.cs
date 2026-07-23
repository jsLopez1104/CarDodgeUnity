using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstáculo")]
    public GameObject prefabObstaculo;

    [Header("Spawneo")]
    public float intervaloInicial = 2f;
    public float intervaloMinimo = 0.5f;
    public float[] posicionesX = { -1.5f, 0f, 1.5f };

    private float timer = 0f;
    private float intervaloActual;

    void Start()
    {
        intervaloActual = intervaloInicial;
    }

    void Update()
    {
        // Intervalo se reduce conforme aumenta la velocidad
        float t = GameManager.instancia.velocidadActual;
        intervaloActual = Mathf.Max(intervaloMinimo, intervaloInicial - (t * 0.05f));

        timer += Time.deltaTime;

        if (timer >= intervaloActual)
        {
            Spawnear();
            timer = 0f;
        }
    }

    void Spawnear()
    {
        float x = posicionesX[Random.Range(0, posicionesX.Length)];
        Vector3 pos = new Vector3(x, 0.5f, 20f);

        GameObject obstaculo = Instantiate(prefabObstaculo, pos, Quaternion.identity);

        ObstacleMove move = obstaculo.GetComponent<ObstacleMove>();
        if (move == null)
            move = obstaculo.AddComponent<ObstacleMove>();

        move.velocidad = GameManager.instancia.velocidadActual;

        Destroy(obstaculo, 10f);
    }
}