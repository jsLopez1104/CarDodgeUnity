using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    [Header("Fuentes de audio")]
    public AudioSource musicaFondo;
    public AudioSource motorCarro;
    public AudioSource efectos;

    [Header("Clips")]
    public AudioClip clipMusica;
    public AudioClip clipMotor;
    public AudioClip clipChoque;
    public AudioClip clipWhoosh;
    public AudioClip clipClick;

    // Volúmenes base (antes de aplicar el multiplicador de PlayerPrefs)
    private const float MUSICA_BASE = 0.2f;
    private const float MOTOR_BASE = 0.3f;

    private float volumenMusica = 1f;
    private float volumenEfectos = 1f;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        // Leer volumen guardado (default 1f = 100% si nunca se guardó nada)
        volumenMusica = PlayerPrefs.GetFloat("volumenMusica", 1f);
        volumenEfectos = PlayerPrefs.GetFloat("volumenEfectos", 1f);

        // Iniciar música de fondo
        musicaFondo.clip = clipMusica;
        musicaFondo.loop = true;
        musicaFondo.volume = MUSICA_BASE * volumenMusica;
        musicaFondo.Play();

        // Iniciar motor
        motorCarro.clip = clipMotor;
        motorCarro.loop = true;
        motorCarro.volume = MOTOR_BASE * volumenEfectos;
        motorCarro.Play();

        // Volumen base de la fuente de efectos (afecta todos los PlayOneShot)
        efectos.volume = volumenEfectos;
    }

    void Update()
    {
        if (GameManager.instancia == null) return;

        // Ajustar pitch del motor según velocidad
        float velocidadNormalizada = GameManager.instancia.velocidadActual / 30f;
        motorCarro.pitch = Mathf.Lerp(0.8f, 1.8f, velocidadNormalizada);

        // Silenciar motor en game over
        if (!GameManager.instancia.juegoActivo)
            motorCarro.volume = Mathf.Lerp(motorCarro.volume, 0f, Time.deltaTime * 2f);
    }

    // --- Métodos públicos para conectar a los sliders del panel de opciones ---
    // Llamar desde el OnValueChanged del slider de música
    public void SetVolumenMusica(float valor)
    {
        volumenMusica = valor;
        musicaFondo.volume = MUSICA_BASE * volumenMusica;
        PlayerPrefs.SetFloat("volumenMusica", volumenMusica);
    }

    // Llamar desde el OnValueChanged del slider de efectos
    public void SetVolumenEfectos(float valor)
    {
        volumenEfectos = valor;
        motorCarro.volume = MOTOR_BASE * volumenEfectos;
        efectos.volume = volumenEfectos;
        PlayerPrefs.SetFloat("volumenEfectos", volumenEfectos);
    }

    public void PlayChoque()
    {
        efectos.PlayOneShot(clipChoque);
    }

    public void PlayWhoosh()
    {
        efectos.PlayOneShot(clipWhoosh, 0.5f);
    }

    public void PlayClick()
    {
        efectos.PlayOneShot(clipClick, 0.7f);
    }
}