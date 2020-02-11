using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header ("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 150;

    [Header ("Enemy Shooting")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 1f;
    [SerializeField] float maxTimeBetweenShots = 5f;
    [SerializeField] GameObject projectile;
    [SerializeField] float shotSpeed = 5f;


    [Header ("Visual and Sound effects")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip enemyDeathSFX;
    [SerializeField] [Range(0,1)]float deathSoundVolume = 0.7f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.7f;
    



    // Start is called before the first frame update
    void Start()
    {
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
        shotCounter -= Time.deltaTime;
    }

    private void CountDownAndShoot()
    {
       

        if(shotCounter <= 0f)
        {
            EnemyFire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }

        
    }

    private void EnemyFire()
    {
        GameObject laser = 
           Instantiate (
            projectile,
            transform.position,
            Quaternion.identity
            ) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -shotSpeed);
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        ProcessHit(other, damageDealer);

    }

    private void ProcessHit(Collider2D other, DamageDealer damageDealer)
    {
        health -= damageDealer.getDamage();
        damageDealer.Hit();
        if (health < 1)
        {
            Die(other);
        }
    }

    private void Die(Collider2D other)
    {

        FindObjectOfType<GameSession>().addToScore(scoreValue);
        Destroy(gameObject);
        Destroy(other.gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(enemyDeathSFX, Camera.main.transform.position, deathSoundVolume);
    }
}
