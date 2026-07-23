using UnityEngine;
using System.Collections.Generic;

public class ProceduralSpawner : MonoBehaviour
{
    [Header("Prefabs por mapa")]
    public GameObject[] propsCiudad;
    public GameObject[] propsBosque;
    public GameObject[] propsDesierto;
    public GameObject[] propsEspacio;
    public GameObject[] propsFantasy;

    [Header("Configuración")]
    public float distanciaSpawn = 30f;
    public float distanciaDestruccion = -30f;
    public float espaciadoZ = 15f;
    public float distanciaLateralMin = 14f;
    public float distanciaLateralMax = 20f;

    private GameObject[] propsActivos;
    private List<GameObject> objetosGenerados = new List<GameObject>();
    private float proximoSpawnZ = 0f;

    void Start()
    {
        int mapa = PlayerPrefs.GetInt("mapaSeleccionado", 0);

        switch (mapa)
        {
            case 0: propsActivos = propsCiudad; break;
            case 1: propsActivos = propsBosque; break;
            case 2: propsActivos = propsDesierto; break;
            case 3: propsActivos = propsEspacio; break;
            case 4: propsActivos = propsFantasy; break;
            default: propsActivos = propsCiudad; break;
        }

        for (int i = 0; i < 5; i++)
        {
            GenerarPar(i * espaciadoZ);
        }

        proximoSpawnZ = distanciaSpawn;
    }

    void Update()
    {
        if (GameManager.instancia == null || !GameManager.instancia.juegoActivo) return;

        float velocidad = GameManager.instancia.velocidadActual;

        for (int i = objetosGenerados.Count - 1; i >= 0; i--)
        {
            if (objetosGenerados[i] == null)
            {
                objetosGenerados.RemoveAt(i);
                continue;
            }

            objetosGenerados[i].transform.Translate(0, 0, -velocidad * Time.deltaTime, Space.World);

            if (objetosGenerados[i].transform.position.z <= distanciaDestruccion)
            {
                Destroy(objetosGenerados[i]);
                objetosGenerados.RemoveAt(i);
            }
        }

        proximoSpawnZ -= velocidad * Time.deltaTime;

        if (proximoSpawnZ <= 0f)
        {
            GenerarPar(distanciaSpawn);
            proximoSpawnZ = espaciadoZ;
        }
    }

    void GenerarPar(float z)
    {
        if (propsActivos == null || propsActivos.Length == 0) return;

        GameObject prefabIzq = propsActivos[Random.Range(0, propsActivos.Length)];
        float xIzq = -Random.Range(distanciaLateralMin, distanciaLateralMax);
        GameObject objIzq = Instantiate(prefabIzq, new Vector3(xIzq, 0, z), Quaternion.identity);
        objetosGenerados.Add(objIzq);

        GameObject prefabDer = propsActivos[Random.Range(0, propsActivos.Length)];
        float xDer = Random.Range(distanciaLateralMin, distanciaLateralMax);
        GameObject objDer = Instantiate(prefabDer, new Vector3(xDer, 0, z), Quaternion.identity);
        objetosGenerados.Add(objDer);
    }
}