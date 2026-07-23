using UnityEngine;

[System.Serializable]
public class CarroPrefabData
{
    public string idCarro;
    public GameObject[] variantes;
}

public class CarroSelector : MonoBehaviour
{
    [Header("Prefabs disponibles")]
    public CarroPrefabData[] carrosDisponibles;

    void Start()
    {
        string carroActivo = PlayerPrefs.GetString("carroActivo", "race");
        int texturaActiva = PlayerPrefs.GetInt("texturaActiva", 0);

        foreach (CarroPrefabData carro in carrosDisponibles)
        {
            if (carro.idCarro == carroActivo)
            {
                int index = Mathf.Clamp(texturaActiva, 0, carro.variantes.Length - 1);
                GameObject modelo = Instantiate(carro.variantes[index], Vector3.zero, Quaternion.identity, transform);
                modelo.transform.localPosition = new Vector3(0, -0.6f, 0);
                modelo.transform.localRotation = Quaternion.identity;
                modelo.transform.localScale = Vector3.one;

                // Ajustar collider según el carro
                AjustarCollider(carro.idCarro);
                return;
            }
        }

        if (carrosDisponibles.Length > 0 && carrosDisponibles[0].variantes.Length > 0)
        {
            Instantiate(carrosDisponibles[0].variantes[0], Vector3.zero, Quaternion.identity, transform);
            AjustarCollider(carrosDisponibles[0].idCarro);
        }
    }

    void AjustarCollider(string idCarro)
    {
        BoxCollider col = GetComponent<BoxCollider>();
        if (col == null) return;

        switch (idCarro)
        {
            case "race":
            case "kart":
                col.size = new Vector3(1f, 5f, 1.5f);
                col.center = new Vector3(0f, 0.3f, 0f);
                break;
            case "firetruck":
                col.size = new Vector3(1.2f, 5f, 2f);
                col.center = new Vector3(0f, 5f, 0f);
                break;
            case "police":
                col.size = new Vector3(1.1f, 8f, 1.8f);
                col.center = new Vector3(0f, 0.4f, 0f);
                break;
            case "tractor":
                col.size = new Vector3(1f, 2f, 1.5f);
                col.center = new Vector3(0f, 0.6f, 0f);
                break;
        }
    }
}