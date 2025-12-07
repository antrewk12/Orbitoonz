using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float speed = 3f;
    [SerializeField]
    private int powerUpId;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.down);

        if (transform.position.y < -6.5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            PlayerController playerController = other.transform.GetComponent<PlayerController>();

            if (playerController != null)
            {
                if (powerUpId == 1)
                {
                    if (playerController.tripleShotActive)
                    {
                        // If triple shot is already active, upgrade to five shot
                        playerController.ActivateFiveShot();
                    }
                    else if(playerController.fiveShotActive)
                    {
                        playerController.ResetFiveShotCoroutine();
                    }
                    else
                    {
                        playerController.ActivateTripleShot();
                    }
                }
                else if (powerUpId == 2)
                {
                    playerController.ActivateShield();
                }
            }
        }
    }
}




