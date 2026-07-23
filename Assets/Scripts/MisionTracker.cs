using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static GameManager;

public class MisionTracker : MonoBehaviour
{
    public static MisionTracker instancia;

    [System.Serializable]
    public class MisionActiva
    {
        public string id;
        public string descripcion;
        public string tipo;
        public int meta;
        public int recompensa;
        public int progresoGuardado; // progreso acumulado de partidas anteriores (útil para esquivas/partidas)
    }

    [System.Serializable]
    private class MisionData
    {
        public string id;
        public string descripcion;
        public string tipo;
        public int meta;
        public int recompensa;
        public int progreso;
        public bool completada;
    }

    [System.Serializable]
    private class MisionesResponse
    {
        public bool success;
        public MisionData[] misiones;
    }

    private List<MisionActiva> misionesActivas = new List<MisionActiva>();
    private HashSet<string> yaMostradas = new HashSet<string>();
    private float timerRevision = 0f;
    private const float INTERVALO_REVISION = 0.3f; // revisar cada 0.3s, no cada frame

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        StartCoroutine(CargarMisiones());
    }

    IEnumerator CargarMisiones()
    {
        string userId = PlayerPrefs.GetString("userId", "anonimo");
        string url = "http://127.0.0.1:5000/get_misiones?userId=" + userId;

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 5;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            MisionesResponse respuesta = JsonUtility.FromJson<MisionesResponse>(request.downloadHandler.text);
            if (respuesta != null && respuesta.success && respuesta.misiones != null)
            {
                foreach (var m in respuesta.misiones)
                {
                    if (m.completada) continue;

                    misionesActivas.Add(new MisionActiva
                    {
                        id = m.id,
                        descripcion = m.descripcion,
                        tipo = m.tipo,
                        meta = m.meta,
                        recompensa = m.recompensa,
                        progresoGuardado = m.progreso
                    });
                }
                RevisarMisionMapa();
            }
        }
    }

    void RevisarMisionMapa()
    {
        int mapaSeleccionado = PlayerPrefs.GetInt("mapaSeleccionado", 0);
        foreach (var m in misionesActivas)
        {
            if (m.tipo == "mapa" && !yaMostradas.Contains(m.id) && mapaSeleccionado == m.meta)
            {
                Disparar(m);
            }
        }
    }

    void Update()
    {
        if (GameManager.instancia == null || !GameManager.instancia.juegoActivo) return;
        if (misionesActivas.Count == 0) return;

        timerRevision += Time.deltaTime;
        if (timerRevision < INTERVALO_REVISION) return;
        timerRevision = 0f;

        foreach (var m in misionesActivas)
        {
            if (yaMostradas.Contains(m.id)) continue;

            int progresoActual = 0;
            switch (m.tipo)
            {
                case "tiempo":
                    progresoActual = Mathf.FloorToInt(Time.timeSinceLevelLoad);
                    break;
                case "velocidad":
                    progresoActual = Mathf.FloorToInt(GameManager.instancia.velocidadMaxima2);
                    break;
                case "score":
                    progresoActual = Mathf.FloorToInt(GameManager.instancia.scoreActual);
                    break;
                case "esquivas":
                    progresoActual = m.progresoGuardado + GameManager.instancia.esquivasTotales;
                    break;
                default:
                    continue; // "partidas" y "mapa" no se evalúan aquí frame a frame
            }

            if (progresoActual >= m.meta)
                Disparar(m);
        }
    }

    void Disparar(MisionActiva m)
    {
        yaMostradas.Add(m.id);

        if (MisionPopupManager.instancia != null)
        {
            MisionCompletada mc = new MisionCompletada
            {
                id = m.id,
                descripcion = m.descripcion,
                recompensa = m.recompensa
            };
            MisionPopupManager.instancia.MostrarMisiones(new MisionCompletada[] { mc });
        }
    }

    // Usado por GameManager al morir, para no repetir el popup en el GameOver
    public bool YaMostrada(string id)
    {
        return yaMostradas.Contains(id);
    }
}