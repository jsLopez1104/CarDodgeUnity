using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager instancia;

    [Header("Audio")]
    public AudioSource fuenteAudio;
    public AudioClip clipClick;
    [Range(0f, 1f)]
    public float volumen = 0.7f;

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;

        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        ConectarBotonesEnEscenaActual();
    }

    void OnSceneLoaded(Scene escena, LoadSceneMode modo)
    {
        ConectarBotonesEnEscenaActual();
    }

    void ConectarBotonesEnEscenaActual()
    {
        Button[] botones = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button b in botones)
        {
            // Evita duplicar el listener si la escena se recarga
            b.onClick.RemoveListener(PlayClick);
            b.onClick.AddListener(PlayClick);
        }
    }

    public void PlayClick()
    {
        if (fuenteAudio != null && clipClick != null)
            fuenteAudio.PlayOneShot(clipClick, volumen);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}