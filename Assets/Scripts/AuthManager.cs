using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class AuthManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public TMP_InputField inputNombre;
    public TMP_Text textoError;

    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(inputEmail.text) || string.IsNullOrEmpty(inputPassword.text))
        {
            textoError.text = "Por favor completa todos los campos.";
            return;
        }
        StartCoroutine(Login());
    }

    public void OnClickRegistro()
    {
        if (string.IsNullOrEmpty(inputEmail.text) ||
            string.IsNullOrEmpty(inputPassword.text) ||
            string.IsNullOrEmpty(inputNombre.text))
        {
            textoError.text = "Por favor completa todos los campos.";
            return;
        }

        if (inputPassword.text.Length < 6)
        {
            textoError.text = "La contraseña debe tener al menos 6 caracteres.";
            return;
        }

        if (!inputEmail.text.Contains("@") || !inputEmail.text.Contains("."))
        {
            textoError.text = "Ingresa un email válido.";
            return;
        }

        StartCoroutine(Registro());
    }

    void Start()
    {
        // Si ya hay sesión activa, ir directo al juego
        if (PlayerPrefs.HasKey("userId") && PlayerPrefs.HasKey("token"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipalScene");
            return;
        }

        // Deshabilitar botones hasta que el servidor esté listo
        GameObject.Find("BotonLogin").GetComponent<UnityEngine.UI.Button>().interactable = false;
        GameObject.Find("BotonRegistro").GetComponent<UnityEngine.UI.Button>().interactable = false;

        GameObject.Find("BotonLogin").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickLogin);
        GameObject.Find("BotonRegistro").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickRegistro);

        StartCoroutine(EsperarServidor());
    }

    IEnumerator EsperarServidor()
    {
        textoError.text = "Conectando...";

        bool servidorListo = false;
        while (!servidorListo)
        {
            UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:5000/face");
            request.timeout = 1;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                servidorListo = true;
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }

        textoError.text = "";
        GameObject.Find("BotonLogin").GetComponent<UnityEngine.UI.Button>().interactable = true;
        GameObject.Find("BotonRegistro").GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    IEnumerator Login()
    {
        textoError.text = "Iniciando sesión...";

        string json = JsonUtility.ToJson(new AuthRequest
        {
            email = inputEmail.text,
            password = inputPassword.text
        });

        UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:5000/login", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 5;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            AuthResponse respuesta = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            if (respuesta.success)
            {
                PlayerPrefs.SetString("userId", respuesta.userId);
                PlayerPrefs.SetString("token", respuesta.token);
                PlayerPrefs.SetString("nombre", respuesta.nombre);
                PlayerPrefs.Save();
                UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipalScene");
            }
            else
            {
                textoError.text = "Email o contraseña incorrectos.";
            }
        }
        else
        {
            textoError.text = "Error de conexión.";
        }
    }

    IEnumerator Registro()
    {
        textoError.text = "Registrando...";

        string json = JsonUtility.ToJson(new AuthRequest
        {
            email = inputEmail.text,
            password = inputPassword.text,
            nombre = inputNombre.text
        });

        UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:5000/register", "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 5;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success ||
            request.responseCode == 400)
        {
            AuthResponse respuesta = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            if (respuesta.success)
            {
                textoError.text = "¡Cuenta creada! Inicia sesión.";
            }
            else
            {
                textoError.text = respuesta.error;
            }
        }
        else
        {
            textoError.text = "Error de conexión.";
        }
    }

    [System.Serializable]
    public class AuthRequest
    {
        public string email;
        public string password;
        public string nombre;
    }

    [System.Serializable]
    public class AuthResponse
    {
        public bool success;
        public string userId;
        public string token;
        public string nombre;
        public string error;
    }
}