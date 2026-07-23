using UnityEngine;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{
    [Header("Panel de pausa")]
    public GameObject panelPausa;

    [Header("Animación")]
    public PanelAnimator animadorPausa;

    [Header("Botones")]
    public Button botonContinuar;
    public Button botonMenu;
    public Button botonCerrarSesion;
    public Button botonSalir;

    private bool pausado = false;

    void Start()
    {
        panelPausa.SetActive(false);
        botonContinuar.onClick.AddListener(Continuar);
        botonMenu.onClick.AddListener(IrAlMenu);
        botonCerrarSesion.onClick.AddListener(CerrarSesion);
        botonSalir.onClick.AddListener(SalirJuego);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado)
                Continuar();
            else
                Pausar();
        }
    }

    void Pausar()
    {
        pausado = true;
        animadorPausa.Mostrar();
        Time.timeScale = 0f;
    }

    void Continuar()
    {
        pausado = false;
        animadorPausa.Ocultar();
        Time.timeScale = 1f;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipalScene");
    }

    public void CerrarSesion()
    {
        PlayerPrefs.DeleteKey("userId");
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("nombre");
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}