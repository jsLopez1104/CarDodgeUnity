using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfiguracionManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelConfiguracion;
    public Button botonConfiguracion;
    public Button botonCerrarConfig;

    [Header("Animación")]
    public PanelAnimator animadorConfiguracion;

    [Header("Sliders")]
    public Slider sliderMusica;
    public Slider sliderEfectos;
    public Slider sliderSensibilidad;

    void Start()
    {
        panelConfiguracion.SetActive(false);
        botonConfiguracion.onClick.AddListener(AbrirConfiguracion);
        botonCerrarConfig.onClick.AddListener(CerrarConfiguracion);

        sliderMusica.value = PlayerPrefs.GetFloat("volumenMusica", 0.2f);
        sliderEfectos.value = PlayerPrefs.GetFloat("volumenEfectos", 0.7f);
        sliderSensibilidad.value = PlayerPrefs.GetFloat("sensibilidad", 5f);

        sliderMusica.onValueChanged.AddListener(v => GuardarConfig());
        sliderEfectos.onValueChanged.AddListener(v => GuardarConfig());
        sliderSensibilidad.onValueChanged.AddListener(v => GuardarConfig());
    }

    void AbrirConfiguracion()
    {
        animadorConfiguracion.Mostrar();
    }

    void CerrarConfiguracion()
    {
        animadorConfiguracion.Ocultar();
        GuardarConfig();
    }

    void GuardarConfig()
    {
        PlayerPrefs.SetFloat("volumenMusica", sliderMusica.value);
        PlayerPrefs.SetFloat("volumenEfectos", sliderEfectos.value);
        PlayerPrefs.SetFloat("sensibilidad", sliderSensibilidad.value);
        PlayerPrefs.Save();
    }
}