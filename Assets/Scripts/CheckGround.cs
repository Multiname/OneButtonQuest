using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public Move move;
    public PhysicsMaterial2D NoFriction;
    public PhysicsMaterial2D LegsFriction;

    private List<Collider2D> groundColliders = new();
    private List<Collider2D> elevatorColliders = new();

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Ground") || collider.CompareTag("Elevator")) {
            groundColliders.Add(collider);
            collider.sharedMaterial = LegsFriction;
            move.SetOnGround(true);

            if (collider.CompareTag("Elevator")) {
                if (elevatorColliders.Count == 0) {
                    move.elevator = collider.transform.parent.parent.GetComponent<Elevator>();
                    // collider.transform.parent.parent.GetComponent<Elevator>().player = move.transform;
                }
                elevatorColliders.Add(collider);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Ground") || collider.CompareTag("Elevator")) {
            groundColliders.Remove(collider);
            collider.sharedMaterial = NoFriction;
            if (groundColliders.Count == 0) {
                move.SetOnGround(false);
            }

            if (collider.CompareTag("Elevator")) {
                elevatorColliders.Remove(collider);
                if (elevatorColliders.Count == 0) {
                    move.elevator = null;
                    // collider.transform.parent.parent.GetComponent<Elevator>().player = null;
                }
            }
        }
    }
}
