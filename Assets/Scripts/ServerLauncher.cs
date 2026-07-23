using UnityEngine;
using System.Diagnostics;

public class ServerLauncher : MonoBehaviour
{
    [Header("Ruta al servidor")]
    public string rutaScript = "C:/Users/sebas/OneDrive/Desktop/CarCode/face_server.py";

    private Process proceso;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        IniciarServidor();
    }

    void IniciarServidor()
    {
        try
        {
            if (!System.IO.File.Exists(rutaScript))
            {
                UnityEngine.Debug.LogError("No se encontró el archivo: " + rutaScript);
                return;
            }

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "python";
            info.Arguments = "\"" + rutaScript + "\"";
            info.UseShellExecute = true;
            info.WindowStyle = ProcessWindowStyle.Minimized;

            proceso = Process.Start(info);
            UnityEngine.Debug.Log("Servidor iniciado.");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (proceso != null && !proceso.HasExited)
        {
            proceso.Kill();
            UnityEngine.Debug.Log("Servidor detenido.");
        }
    }
}