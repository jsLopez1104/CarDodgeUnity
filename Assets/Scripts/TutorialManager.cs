using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelTutorial;
    public PanelAnimator animadorTutorial;
    public Button botonEntendido;

    [Header("Configuración")]
    public bool forzarMostrar = false;

    void Start()
    {
        Debug.Log("TutorialManager Start() ejecutándose");
        Debug.Log("botonEntendido es null? " + (botonEntendido == null));

        panelTutorial.SetActive(false);
        botonEntendido.onClick.AddListener(CerrarTutorial);

        Debug.Log("Listener agregado, cantidad actual: " + botonEntendido.onClick.GetPersistentEventCount());

        bool yaVisto = PlayerPrefs.GetInt("tutorialVisto", 0) == 1;

        if (!yaVisto || forzarMostrar)
        {
            MostrarTutorial();
        }
    }

    void MostrarTutorial()
    {
        panelTutorial.SetActive(true); // activa el padre PRIMERO
        Time.timeScale = 0f;
        animadorTutorial.Mostrar();
    }

    void CerrarTutorial()
    {
        Debug.Log("CerrarTutorial fue llamado");
        Time.timeScale = 1f;
        animadorTutorial.Ocultar();

        PlayerPrefs.SetInt("tutorialVisto", 1);
        PlayerPrefs.Save();

        Invoke(nameof(DesactivarPanel), animadorTutorial.duracion);
    }

    void DesactivarPanel()
    {
        panelTutorial.SetActive(false);
    }
}