using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public bool isVertical = true;
    public float distance = 0.0f;
    public float maxSpeed = 3.0f;
    public float acceleration = 1.0f;
    public float offset = 0.0f;

    public Rigidbody2D rb;
    private int directionMultiplier = 1;
    private float distanceToSlowDown;

    private Vector3 startPosition;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        distanceToSlowDown = Mathf.Pow(maxSpeed, 2) / (2 * acceleration);
    }

    void OnDisable() {
        transform.position = startPosition;
    }

    void FixedUpdate() {
        float positionCoordinate;
        float velocityCoordinate;
        Vector2 velocity = rb.velocity;
        if (isVertical) {
            positionCoordinate = transform.position.y;
            velocityCoordinate = velocity.y;
        } else {
            positionCoordinate = transform.position.x;
            velocityCoordinate = velocity.x;
        }

        CalculateMultiplier(positionCoordinate);
        velocityCoordinate = CalculateVelocity(velocityCoordinate);

        if (isVertical) {
            velocity.y = velocityCoordinate;
        } else {
            velocity.x = velocityCoordinate;
        }
        rb.velocity = velocity;
    }

    private void CalculateMultiplier(float coordinate) {
        if (coordinate >= distance - distanceToSlowDown + offset) {
            directionMultiplier = -1;
        } else if (coordinate <= distanceToSlowDown - offset) {
            directionMultiplier = 1;
        }
    }

    private float CalculateVelocity(float coordinate) {
        coordinate += directionMultiplier * acceleration * Time.fixedDeltaTime;
        if (Mathf.Abs(coordinate) > maxSpeed) {
            coordinate = directionMultiplier * maxSpeed;
        }
        return coordinate;
    }
}
