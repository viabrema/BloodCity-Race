using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [Tooltip("Fator de parallax. 1 = velocidade do jogador, <1 = mais distante")]
    [Range(0f, 2f)]
    public float parallaxFactor = 1f;

    private float backgroundWidth;

    void Start()
    {
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (RaceManager.Instance == null) return;

        float speed = RaceManager.Instance.currentSpeed;

        // Aplica o parallax
        transform.Translate(Vector2.left * speed * parallaxFactor * Time.deltaTime);

        // Loop horizontal
        if (transform.position.x <= -backgroundWidth)
        {
            transform.position += new Vector3((backgroundWidth * 2f) - 0.1f, 0f, 0f);
        }
    }
}
