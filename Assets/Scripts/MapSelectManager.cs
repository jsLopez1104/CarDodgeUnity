using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSelectManager : MonoBehaviour
{
    [Header("Botones Mapas")]
    public Button[] botonesMapas;
    public Button botonVolver;

    [Header("Botones Dificultad")]
    public Button botonFacil;
    public Button botonNormal;
    public Button botonDificil;

    [Header("Boton Jugar")]
    public Button botonJugar;

    private int mapaSeleccionado = 0;
    private int dificultadSeleccionada = 1; // 0=Facil, 1=Normal, 2=Dificil

    private Color colorSeleccionado = new Color(0.306f, 0.804f, 0.769f); // #4ECDC4
    private Color colorNormal = new Color(0.165f, 0.133f, 0.251f); // #2A2240

    void Start()
    {
        // Conectar botones de mapas
        for (int i = 0; i < botonesMapas.Length; i++)
        {
            int index = i;
            botonesMapas[i].onClick.AddListener(() => SeleccionarMapa(index));
        }

        // Conectar botones de dificultad
        botonFacil.onClick.AddListener(() => SeleccionarDificultad(0));
        botonNormal.onClick.AddListener(() => SeleccionarDificultad(1));
        botonDificil.onClick.AddListener(() => SeleccionarDificultad(2));

        // Conectar boton jugar
        botonJugar.onClick.AddListener(Jugar);
        botonVolver.onClick.AddListener(() =>
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipalScene"));

        // Seleccionar defaults
        SeleccionarMapa(0);
        SeleccionarDificultad(1);
    }

    void SeleccionarMapa(int index)
    {
        mapaSeleccionado = index;

        // Resaltar boton seleccionado
        for (int i = 0; i < botonesMapas.Length; i++)
        {
            ColorBlock cb = botonesMapas[i].colors;
            cb.normalColor = i == index ? colorSeleccionado : colorNormal;
            botonesMapas[i].colors = cb;
        }
    }

    void SeleccionarDificultad(int index)
    {
        dificultadSeleccionada = index;

        Button[] botones = { botonFacil, botonNormal, botonDificil };
        for (int i = 0; i < botones.Length; i++)
        {
            ColorBlock cb = botones[i].colors;
            cb.normalColor = i == index ? colorSeleccionado : colorNormal;
            botones[i].colors = cb;
        }
    }

    void Jugar()
    {
        PlayerPrefs.SetInt("mapaSeleccionado", mapaSeleccionado);
        PlayerPrefs.SetInt("dificultadSeleccionada", dificultadSeleccionada);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }
}