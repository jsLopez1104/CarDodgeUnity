using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [Header("Ambientes en escena")]
    public GameObject[] ambientes;

    [Header("Materiales de carretera")]
    public Material[] materialesCarretera;

    void Start()
    {
        int mapaSeleccionado = PlayerPrefs.GetInt("mapaSeleccionado", 0);

        for (int i = 0; i < ambientes.Length; i++)
        {
            if (ambientes[i] != null)
                ambientes[i].SetActive(i == mapaSeleccionado);
        }

        if (mapaSeleccionado < materialesCarretera.Length && materialesCarretera[mapaSeleccionado] != null)
        {
            Material mat = materialesCarretera[mapaSeleccionado];
            GameObject.Find("Road").GetComponent<Renderer>().material = mat;
            GameObject.Find("Road2").GetComponent<Renderer>().material = mat;
            GameObject.Find("Road3").GetComponent<Renderer>().material = mat;
        }
    }
}