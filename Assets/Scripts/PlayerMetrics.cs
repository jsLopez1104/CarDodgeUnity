using UnityEngine;

public class PlayerMetrics : MonoBehaviour
{
    public static PlayerMetrics instancia;

    // Posición actual del jugador (-1 izquierda, 0 centro, 1 derecha)
    public float posicionNormalizada { get; private set; }

    // Hacia qué lado esquiva más
    public int esquivasDerecha { get; private set; }
    public int esquivasIzquierda { get; private set; }

    // Tiempo sobrevivido
    public float tiempoVivo { get; private set; }

    // Últimas posiciones para detectar patrones
    private float[] historialPosicion = new float[10];
    private int historialIndex = 0;

    private Transform carro;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        carro = GameObject.Find("Car").transform;
    }

    void Update()
    {
        if (carro == null) return;

        tiempoVivo += Time.deltaTime;

        // Normalizar posición entre -1 y 1
        posicionNormalizada = Mathf.Clamp(carro.position.x / 2f, -1f, 1f);

        // Guardar historial
        historialPosicion[historialIndex % 10] = posicionNormalizada;
        historialIndex++;

        // Contar esquivas
        if (posicionNormalizada > 0.5f)
            esquivasDerecha++;
        else if (posicionNormalizada < -0.5f)
            esquivasIzquierda++;
    }

    public float LadoPreferido()
    {
        // Retorna hacia qué lado tiende el jugador
        // Positivo = derecha, negativo = izquierda
        int total = esquivasDerecha + esquivasIzquierda;
        if (total == 0) return 0f;
        return (esquivasDerecha - esquivasIzquierda) / (float)total;
    }

    public float PromedioHistorial()
    {
        float suma = 0f;
        foreach (float p in historialPosicion)
            suma += p;
        return suma / historialPosicion.Length;
    }
}