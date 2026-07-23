using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public GameObject prefabExplosion;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            if (AudioManager.instancia != null)
                AudioManager.instancia.PlayChoque();
            if (CameraShake.instancia != null)
            {
                CameraShake.instancia.Shake(0.5f, 0.3f);
                CameraShake.instancia.StartCoroutine(
                    CameraShake.instancia.ZoomAlMorir(transform));
            }
            GameManager.instancia.GameOver();
        }
    }
}
