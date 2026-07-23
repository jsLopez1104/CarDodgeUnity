using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelOpciones;

    [Header("Sliders")]
    public Slider sliderMusica;
    public Slider sliderEfectos;

    [Header("Botones")]
    public Button botonOpciones;
    public Button botonCerrarOpciones;

    void Start()
    {
        botonOpciones.onClick.AddListener(AbrirOpciones);
        botonCerrarOpciones.onClick.AddListener(CerrarOpciones);

        // Cargar valores guardados
        sliderMusica.value = PlayerPrefs.GetFloat("volumenMusica", 0.2f);
        sliderEfectos.value = PlayerPrefs.GetFloat("volumenEfectos", 0.7f);

        // Aplicar valores al iniciar
        AplicarVolumenes();

        // Escuchar cambios
        sliderMusica.onValueChanged.AddListener(v => AplicarVolumenes());
        sliderEfectos.onValueChanged.AddListener(v => AplicarVolumenes());

        panelOpciones.SetActive(false);
    }

    void AbrirOpciones()
    {
        panelOpciones.SetActive(true);
    }

    void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
        // Guardar preferencias
        PlayerPrefs.SetFloat("volumenMusica", sliderMusica.value);
        PlayerPrefs.SetFloat("volumenEfectos", sliderEfectos.value);
        PlayerPrefs.Save();
    }

    void AplicarVolumenes()
    {
        if (AudioManager.instancia == null) return;
        AudioManager.instancia.musicaFondo.volume = sliderMusica.value;
        AudioManager.instancia.efectos.volume = sliderEfectos.value;
        AudioManager.instancia.motorCarro.volume = sliderMusica.value * 0.6f;
    }
}