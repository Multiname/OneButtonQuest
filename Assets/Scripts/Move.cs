using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    public InputAction TapInputAction;

    private Rigidbody2D rb;
    public PhysicsMaterial2D NoFriction;
    public PhysicsMaterial2D LegsFriction;
    private bool wasPressed = false;

    private bool onGround = false;
    private bool nearLadder = false;
    private bool isMovingRight = false;
    private bool isMovingLeft = false;
    private bool stopLeftMovement = false;
    private bool stopRightMovement = false;
    private bool leftSlowDown = false;
    private bool rightSlowDown = false;

    private float slowdownVelocity = 0.0f;
    private float baseHorizontalVelocity = 0.0f;

    public Elevator elevator = null;
    private List<Collider2D> ladderColliders = new();
    public Action buttonAction;

    private int cycleMod = 0;
    public bool cyclesEnabled = false;

    public float speed = 3.0f;

    public GameObject meleeAttackHitBox;
    public bool facedLeftInitially = false;
    private bool isAttacking = false;

    public Projectile projectile;
    private bool rangeAttackIsOnCooldown = false;
    private bool facingLeft = false;
    public SpriteRenderer ammoCounter;
    private int ammoCount = 3;
    private Color[] counterValues = new Color[4] { 
        Color.black,
        Color.red,
        Color.yellow,
        Color.green
    };

    public enum CycleMode {
        Switch,
        FirstOnly,
        SecondOnly
    }

    private CycleMode cycleMode;

    public bool invulnerable = false;
    private bool temporarilyInvulnerable = false;
    private IEnumerator invulnerabilityRoutine;
    private SpriteRenderer sr;
    public Action handleInvulnerabilityDisabling;

    private Action<Move>[] tapAction = new Action<Move>[2];
    private Action<Move>[] pressAction = new Action<Move>[2];
    private Action<Move>[] releaseAction = new Action<Move>[2];
    private Action<Move>[] noPressAction = new Action<Move>[2];

    private Action<Move, bool> doNothing = (Move move, bool impulseMode) => {};
    private Action<Move, bool> moveLeft = (Move move, bool impulseMode) => {
        move.Face(true);
        move.baseHorizontalVelocity = -move.speed;

        // Vector2 velocity = move.rb.velocity;
        // velocity.x = -3.0f;
        // move.rb.velocity= velocity;

        if (impulseMode) {
            move.slowdownVelocity = -move.speed;

            move.leftSlowDown = true;
            move.rightSlowDown = false;
            move.stopRightMovement = false;
        } else {
            move.isMovingLeft = true;
            move.stopLeftMovement = true;
            move.stopRightMovement = false;
            move.rightSlowDown = false;
            move.leftSlowDown = false;
        }
    };
    private Action<Move, bool> moveRight = (Move move, bool impulseMode) => {
        move.Face(false);
        move.baseHorizontalVelocity = move.speed;

        // Vector2 velocity = move.rb.velocity;
        // velocity.x = 3.0f;
        // move.rb.velocity= velocity;

        if (impulseMode) {
            move.slowdownVelocity = move.speed;

            move.rightSlowDown = true;
            move.leftSlowDown = false;
            move.stopLeftMovement = false;
        } else {
            move.isMovingRight = true;
            move.stopRightMovement = true;
            move.stopLeftMovement = false;
            move.leftSlowDown = false;
            move.rightSlowDown = false;
        }
    };
    private Action<Move, bool> jump = (Move move, bool impulseMode) => {
        if (move.onGround) {
            Vector2 velocity = move.rb.velocity;
            // velocity.y = 11.5f;
            velocity.y = 9.5f;
            move.rb.velocity= velocity;
        }
    };
    private Action<Move, bool> climb = (Move move, bool impulseMode) => {
        if (move.nearLadder) {
            float climbVelocity = 3.0f;
            if (impulseMode) {
                climbVelocity = 6.0f;
            }

            Vector2 velocity = move.rb.velocity;
            if (velocity.y < climbVelocity) {
                velocity.y = climbVelocity;
                move.rb.velocity= velocity;
            }
        }
    };
    private Action<Move, bool> activateButton = (Move move, bool impulseMode) => {
        move.buttonAction?.Invoke();
    };
    private Action<Move, bool> attackInMelee = (Move move, bool impulseMode) => {
        move.StartMeleeAttack();
    };
    private Action<Move, bool> attackInRange = (Move move, bool impulseMode) => {
        move.StartRangeAttack();
    };
    private Action<Move, bool> becomeInvulnerable = (Move move, bool impulseMode) => {
        move.sr.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        if (impulseMode) {
            move.StartInvulnerability();
        } else {
            move.invulnerable = true;
        }
    };

    private List<Action<Move, bool>> actionList;

    private void StartMeleeAttack() {
        if (!isAttacking) {
            isAttacking = true;
            StartCoroutine(AttackInMelee());
        }
    }

    private IEnumerator AttackInMelee() {
        meleeAttackHitBox.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        meleeAttackHitBox.SetActive(false);
        isAttacking = false;
    }

    private void Face(bool left) {
        facingLeft = left;
        float x = Mathf.Pow(-1, Convert.ToInt32(left));
        meleeAttackHitBox.transform.localPosition = new Vector2(x, 0);
    }

    private void StartRangeAttack() {
        if (!rangeAttackIsOnCooldown && ammoCount > 0) {
            rangeAttackIsOnCooldown = true;
            --ammoCount;
            ammoCounter.color = counterValues[ammoCount];
            StartCoroutine(AttackInRange());
        }
    }

    private IEnumerator AttackInRange() {
        Instantiate(projectile, transform.position, projectile.transform.rotation).leftDirected = facingLeft;
        yield return new WaitForSeconds(1.0f);
        rangeAttackIsOnCooldown = false;
    }

    private void StartInvulnerability () {
        if (temporarilyInvulnerable) {
            StopCoroutine(invulnerabilityRoutine);
        }

        temporarilyInvulnerable = true;
        invulnerable = true;
        invulnerabilityRoutine = BecomeInvulnerableTemporarily();
        StartCoroutine(invulnerabilityRoutine);
    }

    private IEnumerator BecomeInvulnerableTemporarily() {
        yield return new WaitForSeconds(1.0f);
        sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        invulnerable = false;
        temporarilyInvulnerable = false;
        handleInvulnerabilityDisabling();
    }

    private void OnEnable() {
        TapInputAction.Enable();
        rb.simulated = true;

        ammoCount = 3;
        ammoCounter.color = counterValues[ammoCount];
    }

    private void OnDisable() {
        Die();
        
        TapInputAction.Disable();
        rb.simulated = false;
    }

    public void Die() {
        rb.velocity = new Vector2(0.0f, 0.0f);

        wasPressed = false;
        onGround = false;
        nearLadder = false;
        isMovingRight = false;
        isMovingLeft = false;
        stopLeftMovement = false;
        stopRightMovement = false;
        leftSlowDown = false;
        rightSlowDown = false;

        slowdownVelocity = 0.0f;
        baseHorizontalVelocity = 0.0f;

        elevator = null;
        ladderColliders.Clear();
        buttonAction = null;

        Face(facedLeftInitially);
        isAttacking = false;
        meleeAttackHitBox.SetActive(false);

        rangeAttackIsOnCooldown = false;

        cycleMod = 0;
        if (cyclesEnabled) {
            cycleMode = CycleMode.Switch;
        } else {
            cycleMode = CycleMode.FirstOnly;
        }

        sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        invulnerable = false;
        temporarilyInvulnerable = false;
    }

    public void SetCycleMode(CycleMode mode) {
        cycleMode = mode;
        if (cycleMode == CycleMode.FirstOnly) {
            cycleMod = 0;
        } else if (cycleMode == CycleMode.SecondOnly) {
            cycleMod = 1;
        }
    }

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Face(facedLeftInitially);
        ammoCounter.color = counterValues[ammoCount];

        if (cyclesEnabled) {
            cycleMode = CycleMode.Switch;
        } else {
            cycleMode = CycleMode.FirstOnly;
        }

        actionList = new() {
            doNothing,
            moveLeft,
            moveRight,
            jump,
            climb,
            activateButton,
            attackInMelee,
            attackInRange,
            becomeInvulnerable
        };
    }

    void FixedUpdate() {
        if (!temporarilyInvulnerable) {
            sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            invulnerable = false;
        }

        isMovingLeft = false;
        isMovingRight = false;

        if (TapInputAction.ReadValue<float>() == 1.0) {
            if (!wasPressed) {
                wasPressed = true;
                tapAction[cycleMod](this);
            } else {
                pressAction[cycleMod](this);
            }
        } else {
            if (wasPressed) {
                wasPressed = false;
                releaseAction[cycleMod](this);

                if (cycleMode == CycleMode.Switch){
                    cycleMod = (cycleMod + 1) % 2;
                }
            } else {
                noPressAction[cycleMod](this);
            }
        }

        if (!isMovingLeft && stopLeftMovement) {
            baseHorizontalVelocity += speed;

            // Vector2 velocity = rb.velocity;
            // velocity.x += 3.0f;
            // rb.velocity = velocity;

            stopLeftMovement = false;
        }
        if (!isMovingRight && stopRightMovement) {
            baseHorizontalVelocity -= speed;

            // Vector2 velocity = rb.velocity;
            // velocity.x -= 3.0f;
            // rb.velocity = velocity;
            
            stopRightMovement = false;
        }

        if (leftSlowDown) {
            slowdownVelocity += speed * Time.fixedDeltaTime;
            if (slowdownVelocity >= 0) {
                slowdownVelocity = 0;
                leftSlowDown = false;
            }

            baseHorizontalVelocity = slowdownVelocity;

            // Vector2 velocity = rb.velocity;
            // velocity.x = slowdownVelocity;
            // rb.velocity = velocity;

            // Vector2 velocity = rb.velocity;
            // velocity.x += 3.0f * Time.fixedDeltaTime;
            // if (velocity.x >= 0) {
            //     velocity.x = 0;
            //     leftSlowDown = false;
            // }
            // rb.velocity = velocity;
        }
        if (rightSlowDown) {
            slowdownVelocity -= speed * Time.fixedDeltaTime;
            if (slowdownVelocity <= 0) {
                slowdownVelocity = 0;
                rightSlowDown = false;
            }

            baseHorizontalVelocity = slowdownVelocity;

            // Vector2 velocity = rb.velocity;
            // velocity.x = slowdownVelocity;
            // rb.velocity = velocity;

            // Vector2 velocity = rb.velocity;
            // velocity.x -= 3.0f * Time.fixedDeltaTime;
            // if (velocity.x <= 0) {
            //     velocity.x = 0;
            //     rightSlowDown = false;
            // }
            // rb.velocity = velocity;
        }

        Vector2 velocity = rb.velocity;
        velocity.x = baseHorizontalVelocity;

        if (elevator != null) {
            if (!elevator.isVertical) {
                velocity.x += elevator.rb.velocity.x;
            }
        }

        rb.velocity = velocity;

        if (!invulnerable) {
            handleInvulnerabilityDisabling();
        }
    }

    public void SetActions(int[] actionForTap, int[] actionForPress, int[] actionForRelease, int[] actionForNoPress) {
        tapAction[0] = (Move move) => { actionList[actionForTap[0]](move, true); };
        pressAction[0] = (Move move) => { actionList[actionForPress[0]](move, false); };
        releaseAction[0] = (Move move) => { actionList[actionForRelease[0]](move, true); };
        noPressAction[0] = (Move move) => { actionList[actionForNoPress[0]](move, false); };

        if (cyclesEnabled) {
            tapAction[1] = (Move move) => { actionList[actionForTap[1]](move, true); };
            pressAction[1] = (Move move) => { actionList[actionForPress[1]](move, false); };
            releaseAction[1] = (Move move) => { actionList[actionForRelease[1]](move, true); };
            noPressAction[1] = (Move move) => { actionList[actionForNoPress[1]](move, false); };
        } else {
            tapAction[1] = tapAction[0];
            pressAction[1] = pressAction[0];
            releaseAction[1] = releaseAction[0];
            noPressAction[1] = noPressAction[0];
        }
    }

    public void SetOnGround(bool onGround) {
        this.onGround = onGround;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Ladder")) {
            ladderColliders.Add(collider);
            nearLadder = true;
        } else if (collider.CompareTag("Ammo")) {
            ammoCount = 3;
            ammoCounter.color = counterValues[ammoCount];
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Ladder")) {
            ladderColliders.Remove(collider);
            if (ladderColliders.Count == 0) {
                nearLadder = false;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        if (collider.CompareTag("RightWall") && rightSlowDown || collider.CompareTag("LeftWall") && leftSlowDown) {
            foreach (var contact in collision.contacts) {
                if (math.abs(transform.position.y - contact.point.y) < 0.49) {
                    slowdownVelocity = 0.0f;
                    return;
                }
            }
        }
    }
}
