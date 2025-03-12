using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool leftDirected = false;
    public float speed = 10.0f;
    public bool hostile;

    void Start() {
        GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Pow(-1, Convert.ToInt32(leftDirected)) * speed, 0.0f);
    }
    
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("RightWall") || collider.CompareTag("LeftWall") ||
            hostile && collider.CompareTag("Player") ||
            !hostile && (
                collider.CompareTag("Enemy") ||
                collider.CompareTag("EnemyWithShield") ||
                collider.CompareTag("EnemyWithBarrier") ||
                collider.CompareTag("ShootingTarget")
        )) {
            Destroy(gameObject);
        }
    }
}