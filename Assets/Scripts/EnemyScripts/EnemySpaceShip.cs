using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceShip : MonoBehaviour
{

    
    public GameObject deathParticlesPrefab;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

    }

    public void Update()
    {
        if (GameManager.instance.isGameOver) return; // Stop if game is over
    }

    

   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);

            if (playerController.tripleShotActive == true || playerController.fiveShotActive == true)
            {
                Destroy(other.transform.parent.gameObject);
            }

            if (playerController != null)
                {
                    playerController.AddScore();
                }

                // Instantiate death particles
                GameObject particles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
                // Get the duration of the particle system
                float particleDuration = particles.GetComponent<ParticleSystem>().main.duration;
                // Destroy the particle system after its duration
                Destroy(particles, particleDuration);

            }

            if (other.CompareTag("Player"))
            {
                PlayerController playerController = other.transform.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    playerController.Damage();
                    Destroy(gameObject);

                }
                 else
                {
                   return;
                }

            // Instantiate death particles
            GameObject particles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            // Get the duration of the particle system
            float particleDuration = particles.GetComponent<ParticleSystem>().main.duration;
            // Destroy the particle system after its duration
            Destroy(particles, particleDuration);

        }
        }
    }





