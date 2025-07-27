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

        float direction = 0f;

        if (other.CompareTag("Obstacle"))
        {
            Obstacle obstacle = other.GetComponent<Obstacle>();
            if (obstacle.direction != 0)
            {
                direction = obstacle.direction;
                oponent.OnSensorTrigger(direction);
                return;
            }
        }

        if (other.CompareTag("Player") || other.CompareTag("Oponent") || other.CompareTag("Obstacle"))
        {
            direction = other.transform.position.y > transform.position.y ? -1f : 1f;
            oponent.OnSensorTrigger(direction);
            return;
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (oponent == null) return;

        if (other.CompareTag("Player") || other.CompareTag("Oponent") || other.CompareTag("Obstacle"))
        {
            oponent.SetSlowedByObstacle(false, 0f, null);
        }
    }
}
