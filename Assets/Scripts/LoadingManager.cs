using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("UI")]
    public Slider barraProgreso;
    public TMP_Text textoCargando;
    public TMP_Text textoMapa;

    private string[] nombresMapa = { "Ciudad", "Bosque", "Desierto", "Espacio", "Fantasy" };
    private string[] nombresDificultad = { "Fácil", "Normal", "Difícil" };

    void Start()
    {
        int mapa = PlayerPrefs.GetInt("mapaSeleccionado", 0);
        int dificultad = PlayerPrefs.GetInt("dificultadSeleccionada", 1);

        textoMapa.text = nombresMapa[mapa] + " - " + nombresDificultad[dificultad];

        StartCoroutine(CargarEscena());
    }

    IEnumerator CargarEscena()
    {
        yield return new WaitForSeconds(0.2f);

        AsyncOperation operacion = SceneManager.LoadSceneAsync("SampleScene");
        operacion.allowSceneActivation = false;

        while (operacion.progress < 0.9f)
        {
            barraProgreso.value = operacion.progress;
            yield return null;
        }

        barraProgreso.value = 1f;
        textoCargando.text = "Listo!";

        yield return new WaitForSeconds(0.5f);

        operacion.allowSceneActivation = true;
    }
}