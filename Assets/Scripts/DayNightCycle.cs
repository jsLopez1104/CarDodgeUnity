using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Skybox")]
    public Material skyboxNoche;

    [Header("Luces del carro")]
    public GameObject lucesDelanteras;

    void Start()
    {
        RenderSettings.skybox = skyboxNoche;
        DynamicGI.UpdateEnvironment();

        if (lucesDelanteras != null)
            lucesDelanteras.SetActive(true);

        if (RenderSettings.sun != null)
            RenderSettings.sun.intensity = 0.3f;
    }
}