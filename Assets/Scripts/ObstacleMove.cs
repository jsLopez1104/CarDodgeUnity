using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    public float velocidad = 10f;

    void Update()
    {
        transform.Translate(0, 0, -velocidad * Time.deltaTime);
    }
}