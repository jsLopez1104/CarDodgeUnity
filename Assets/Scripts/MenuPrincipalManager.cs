using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textoUsuario;
    public Button botonJugar;
    public Button botonGarage;
    public Button botonConfiguracion;
    public Button botonCerrarSesion;
    public Button botonSalir;
    public Button botonMisiones;

    [Header("Panel Configuración")]
    public GameObject panelConfiguracion;

    void Start()
    {
        string nombre = PlayerPrefs.GetString("nombre", "Jugador");
        textoUsuario.text = "Hola, " + nombre + "!";
        botonJugar.onClick.AddListener(Jugar);
        botonGarage.onClick.AddListener(IrAlGarage);
        botonConfiguracion.onClick.AddListener(AbrirConfiguracion);
        botonCerrarSesion.onClick.AddListener(CerrarSesion);
        botonSalir.onClick.AddListener(Salir);
        if (panelConfiguracion != null)
            panelConfiguracion.SetActive(false);
    }

    void Jugar()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelectScene");
    }

    void IrAlGarage()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GarageScene");
    }

    void AbrirConfiguracion()
    {
        if (panelConfiguracion != null)
            panelConfiguracion.SetActive(!panelConfiguracion.activeSelf);
    }

    void CerrarSesion()
    {
        PlayerPrefs.DeleteKey("userId");
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("nombre");
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }

    void Salir()
    {
        Application.Quit();
    }
}