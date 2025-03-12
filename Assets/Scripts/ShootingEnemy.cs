using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    public bool facedLeft = false;
    private Rigidbody2D rb;
    public Projectile projectile;
    private bool shootingOnCooldown = false;
    private Transform innerObject;
    public float cooldown = 1.0f;
    private GameManager gameManager;
    private IEnumerator currentRoutine;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        gameManager = GetComponent<Spike>().gameManager;
        innerObject = transform.GetChild(0);

        gameManager.resetInteractive += Reset;
    }

    private void Reset() {
        if (currentRoutine != null) {
            StopCoroutine(currentRoutine);
        }
        shootingOnCooldown = false;
    }

    void FixedUpdate() {
        var velocity = rb.velocity.x;
        if (velocity > 0) {
            facedLeft = false;
        } else if (velocity < 0) {
            facedLeft = true;
        }

        if (!shootingOnCooldown) {
            shootingOnCooldown = true;
            currentRoutine = Shoot();
            StartCoroutine(currentRoutine);
        }
    }

    private IEnumerator Shoot() {
        var createdProjectile = Instantiate(projectile, innerObject.position, projectile.transform.rotation);
        createdProjectile.leftDirected = facedLeft;
        createdProjectile.GetComponent<Spike>().gameManager = gameManager;
        yield return new WaitForSeconds(cooldown);
        shootingOnCooldown = false;
    }
}
