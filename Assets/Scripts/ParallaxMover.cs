using UnityEngine;

public class ParallaxMover : MonoBehaviour
{
    [Range(0f, 2f)]
    public float parallaxFactor = 1f; // 1.0 = mesma velocidade do player, >1 = mais rápido, <1 = mais lento

    void Update()
    {
        if (RaceManager.Instance == null) return;

        float speed = RaceManager.Instance.currentSpeed;
        
        transform.Translate(Vector2.left * speed * parallaxFactor * Time.deltaTime);
    }
}
