using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Player")]
    [SerializeField] float movespeed = 12f;
    [SerializeField] int health = 200;
    [SerializeField] GameObject hitbox;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.7f;
    [SerializeField] AudioClip DeathSFX;



    [Header("Projectiles")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float shotSpeed = 10f;
    [SerializeField] float projectileFiringPeriods = 0.1f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.7f;


    float xMin;
    float xMax;
    float yMin;
    float yMax;
    Coroutine firingCoroutine;



    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

   
    // Update is called once per frame
    void Update()
    {
        HandleSlowMode();
        Move();
        fire();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer)
        {
            return;
        }
        ProcessHit(other, damageDealer);
        

    }

    private void ProcessHit(Collider2D other, DamageDealer damageDealer)
    {
        health -= damageDealer.getDamage();
        damageDealer.Hit();
        Destroy(other.gameObject);
        if (health < 1)
        {
            Die();            
        }
    }


    private void Die()
    {
        FindObjectOfType<Level>().LoadGameOver();
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(DeathSFX, Camera.main.transform.position, deathSoundVolume);

    }

            private void fire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            
            firingCoroutine = StartCoroutine(constFire());
            
        }
        if (Input.GetButtonUp("Fire1"))
            StopCoroutine(firingCoroutine);
        {
            
        }
    }


    IEnumerator constFire()
    {
        while (true)
        {
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
        if(movespeed == 4f)
            {
                laser.transform.localScale += new Vector3(2f, 0, 0);
                //change damage
            }
            else
            {
                //possibly change damage back
            }
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, shotSpeed);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
        yield return new WaitForSeconds(projectileFiringPeriods);
        }
    }

    private void HandleSlowMode()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movespeed = 4f;
            hitbox.transform.localScale = new Vector3(.3f, .3f, .3f);
        }
        else
        {
            movespeed = 12f;
            hitbox.transform.localScale = new Vector3(0f, 0f, 0f);
        }
    }

    private void Move()
    {
        float deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * movespeed;
        float deltaY = Input.GetAxis("Vertical") * Time.deltaTime * movespeed;
        var newxpos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newypos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newxpos, newypos);
    }
    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0.01f, 0, 0)).x;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(0.99f, 0, 0)).x;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0.01f, 0)).y;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 0.99f, 0)).y;
    }

}
