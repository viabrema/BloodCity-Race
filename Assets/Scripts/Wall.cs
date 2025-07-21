using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Wall : MonoBehaviour
{
    [Tooltip("Direção sugerida para desvio: 1 = para cima, -1 = para baixo")]
    public float suggestedAvoidDirection = 1f;

    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (RaceManager.Instance == null) return;

        // Move o wall no sentido contrário à velocidade do player
        float playerSpeed = RaceManager.Instance.currentSpeed;
        transform.Translate(Vector2.left * playerSpeed * Time.deltaTime);
    }
}
