using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class CarroData
{
    public string idCarro;
    public string nombreMostrar;
    public int precio;
    public GameObject[] prefabsVariantes; // las texturas/colores
}

public class GarageManager : MonoBehaviour
{
    [Header("Carros disponibles")]
    public CarroData[] carros;

    [Header("UI")]
    public TMP_Text textoMonedas;
    public TMP_Text nombreCarroTexto;
    public TMP_Text precioCarroTexto;
    public Button botonAnterior;
    public Button botonSiguiente;
    public Button botonComprar;
    public Button botonSeleccionar;
    public Button botonJugar;
    public Transform puntoSpawnCarro;
    public Button botonVolver;

    [Header("Indicadores de textura")]
    public Transform panelTexturas;
    public GameObject prefabIndicadorTextura;

    private int indiceCarroActual = 0;
    private int indiceTexturaActual = 0;
    private List<string> carrosDesbloqueados = new List<string>();
    private int monedas = 0;
    private GameObject carroInstanciado;
    private string userId;

    [Header("Pantalla de carga")]
    public GameObject panelCarga;
    public Slider barraProgreso;

    void Start()
    {
        userId = PlayerPrefs.GetString("userId", "anonimo");

        botonAnterior.onClick.AddListener(CarroAnterior);
        botonSiguiente.onClick.AddListener(CarroSiguiente);
        botonComprar.onClick.AddListener(ComprarCarroActual);
        botonSeleccionar.onClick.AddListener(SeleccionarCarroActual);
        botonJugar.onClick.AddListener(Jugar);
        botonVolver.onClick.AddListener(() =>
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipalScene"));

        panelCarga.SetActive(true);
        barraProgreso.value = 0f;

        StartCoroutine(CargarDatosGarage());
    }

    IEnumerator CargarDatosGarage()
    {
        barraProgreso.value = 0.1f;

        // Cargar monedas
        UnityWebRequest reqMonedas = UnityWebRequest.Get($"http://127.0.0.1:5000/get_monedas?userId={userId}");
        yield return reqMonedas.SendWebRequest();

        barraProgreso.value = 0.5f;

        if (reqMonedas.result == UnityWebRequest.Result.Success)
        {
            MonedasResponse resp = JsonUtility.FromJson<MonedasResponse>(reqMonedas.downloadHandler.text);
            monedas = resp.monedas;
            textoMonedas.text = "Monedas: " + monedas;
        }

        // Cargar garage
        UnityWebRequest reqGarage = UnityWebRequest.Get($"http://127.0.0.1:5000/get_garage?userId={userId}");
        yield return reqGarage.SendWebRequest();

        barraProgreso.value = 0.8f;

        if (reqGarage.result == UnityWebRequest.Result.Success)
        {
            GarageResponse resp = JsonUtility.FromJson<GarageResponse>(reqGarage.downloadHandler.text);
            carrosDesbloqueados = new List<string>(resp.carros_desbloqueados);

            for (int i = 0; i < carros.Length; i++)
            {
                if (carros[i].idCarro == resp.carro_activo)
                {
                    indiceCarroActual = i;
                    break;
                }
            }
            indiceTexturaActual = resp.textura_activa;
        }

        ActualizarVistaCarro();

        barraProgreso.value = 1f;
        yield return new WaitForSeconds(0.3f);

        panelCarga.SetActive(false);
    }

    void ActualizarVistaCarro()
    {
        CarroData carro = carros[indiceCarroActual];

        // Destruir carro anterior
        if (carroInstanciado != null)
            Destroy(carroInstanciado);

        // Crear contenedor que rota
        GameObject contenedor = new GameObject("ContenedorCarro");
        contenedor.transform.position = puntoSpawnCarro.position;
        contenedor.transform.rotation = puntoSpawnCarro.rotation;

        // Instanciar carro dentro del contenedor
        GameObject modeloCarro = Instantiate(
            carro.prefabsVariantes[indiceTexturaActual],
            puntoSpawnCarro.position,
            puntoSpawnCarro.rotation
        );
        modeloCarro.transform.SetParent(contenedor.transform);

        // Agregar rotacion al contenedor
        CarroRotacion rotacion = contenedor.AddComponent<CarroRotacion>();
        rotacion.velocidadRotacion = 45f;

        // Guardar referencia al contenedor para destruirlo después
        carroInstanciado = contenedor;

        nombreCarroTexto.text = carro.nombreMostrar;

        bool desbloqueado = carrosDesbloqueados.Contains(carro.idCarro);

        if (desbloqueado)
        {
            precioCarroTexto.text = "DESBLOQUEADO";
            botonComprar.gameObject.SetActive(false);
            botonSeleccionar.gameObject.SetActive(true);
        }
        else
        {
            precioCarroTexto.text = carro.precio + " monedas";
            botonComprar.gameObject.SetActive(true);
            botonSeleccionar.gameObject.SetActive(false);
        }

        ActualizarIndicadoresTextura(carro);
    }

    void ActualizarIndicadoresTextura(CarroData carro)
    {
        // Limpiar indicadores anteriores
        foreach (Transform t in panelTexturas)
            Destroy(t.gameObject);

        // Crear un indicador por cada variante
        for (int i = 0; i < carro.prefabsVariantes.Length; i++)
        {
            int index = i;
            GameObject indicador = Instantiate(prefabIndicadorTextura, panelTexturas);
            Button btn = indicador.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => SeleccionarTextura(index));

            // Resaltar la actual
            Image img = indicador.GetComponent<Image>();
            if (img != null)
                img.color = (i == indiceTexturaActual) ? new Color(0.306f, 0.804f, 0.769f) : new Color(0.165f, 0.133f, 0.251f);
        }
    }

    void SeleccionarTextura(int index)
    {
        indiceTexturaActual = index;
        ActualizarVistaCarro();
    }

    void CarroAnterior()
    {
        indiceCarroActual--;
        if (indiceCarroActual < 0) indiceCarroActual = carros.Length - 1;
        indiceTexturaActual = 0;
        ActualizarVistaCarro();
    }

    void CarroSiguiente()
    {
        indiceCarroActual++;
        if (indiceCarroActual >= carros.Length) indiceCarroActual = 0;
        indiceTexturaActual = 0;
        ActualizarVistaCarro();
    }

    void ComprarCarroActual()
    {
        StartCoroutine(ComprarCarroCoroutine());
    }

    IEnumerator ComprarCarroCoroutine()
    {
        CarroData carro = carros[indiceCarroActual];

        string json = JsonUtility.ToJson(new CompraRequest
        {
            userId = userId,
            carro = carro.idCarro,
            precio = carro.precio
        });

        UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:5000/comprar_carro", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 5;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success || request.responseCode == 400)
        {
            CompraResponse resp = JsonUtility.FromJson<CompraResponse>(request.downloadHandler.text);
            if (resp.success)
            {
                monedas = resp.monedas;
                textoMonedas.text = "Monedas: " + monedas;
                carrosDesbloqueados.Add(carro.idCarro);
                ActualizarVistaCarro();
            }
        }
    }

    void SeleccionarCarroActual()
    {
        StartCoroutine(SeleccionarCarroCoroutine());
    }

    IEnumerator SeleccionarCarroCoroutine()
    {
        CarroData carro = carros[indiceCarroActual];

        string json = JsonUtility.ToJson(new SeleccionRequest
        {
            userId = userId,
            carro = carro.idCarro,
            textura = indiceTexturaActual
        });

        UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:5000/set_carro_activo", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 5;

        yield return request.SendWebRequest();

        PlayerPrefs.SetString("carroActivo", carro.idCarro);
        PlayerPrefs.SetInt("texturaActiva", indiceTexturaActual);
        PlayerPrefs.Save();
    }

    void Jugar()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelectScene");
    }

    [System.Serializable]
    public class MonedasResponse
    {
        public bool success;
        public int monedas;
    }

    [System.Serializable]
    public class GarageResponse
    {
        public bool success;
        public string[] carros_desbloqueados;
        public string carro_activo;
        public int textura_activa;
    }

    [System.Serializable]
    public class CompraRequest
    {
        public string userId;
        public string carro;
        public int precio;
    }

    [System.Serializable]
    public class CompraResponse
    {
        public bool success;
        public int monedas;
        public string error;
    }

    [System.Serializable]
    public class SeleccionRequest
    {
        public string userId;
        public string carro;
        public int textura;
    }
}