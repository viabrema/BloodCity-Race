using UnityEngine;

public class OponentSensor : MonoBehaviour
{
    private Oponent oponent;

    void Start()
    {
        oponent = GetComponentInParent<Oponent>();
        if (oponent == null)
        {
            Debug.LogError("OponentSensor: Oponent script not found on parent!");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (oponent == null) return;

        if (other.CompareTag("Obstacle"))
        {
            float direction = other.transform.position.y > transform.position.y ? -1f : 1f;
            oponent.SetSlowedByObstacle(true, direction, other);
        }
        else if (other.CompareTag("Player") || other.CompareTag("Oponent"))
        {
            float direction = other.transform.position.y > transform.position.y ? -1f : 1f;
            oponent.OnSensorTrigger(direction);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (oponent == null) return;

        if (other.CompareTag("Obstacle"))
        {
            oponent.SetSlowedByObstacle(false, 0f, null);
        }
    }
}
