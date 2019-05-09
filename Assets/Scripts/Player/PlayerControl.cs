using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerControl {
    private const string AXE_SWING = "AxeSwing";
    private const string TORNADO = "Tornado";
    private const string AXE_SLAM = "AxeSlam";

    private const string ATTACK = "Attack";
    private const string USE = "Use";
    private const string JUMP = "Jump";

    private const string ATTACK_SPEED = "AttackSpeed";

    private Player player;
    private Rigidbody rigidbody;
    private Collider axeCollider;
    private bool canSwing = true;

    [SerializeField] private float groundedDistance;
    [SerializeField] private AnimationCurve jumpCurve;

    private bool attackLoading;
    private float attackButtonDownTime;

    private List<IUsable> usablesInRange = new List<IUsable>();
    private IUsable closestUsable;

    public void Setup (Player player, Collider axeCollider) {
        this.player = player;
        rigidbody = player.GetComponent<Rigidbody>();
        this.axeCollider = axeCollider;

        player.animation.OnExit += AnimatorManager_OnExit;
        player.OnDeath += Player_OnDeath;
    }

    public void Update () {
        if (player.isAlive) {
            // Update translation
            rigidbody.AddForce(GetInputSpeed() * (player.statistics.movementSpeed - rigidbody.velocity.magnitude), ForceMode.Force);

            // Update rotation
            try {
                rigidbody.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(GetTargetedPoint() - rigidbody.transform.position, Vector3.up));
            } catch (InvalidOperationException) {
                Debug.LogWarning("No point targeted by cursor");
            }

            // Trigger jump
            bool grounded = Physics.Raycast(player.transform.position + 0.1f * Vector3.up, Vector3.down, groundedDistance, LayerMask.GetMask("Terrain"));
            player.animation.animator.SetBool("Grounded", grounded);
            if (Input.GetButtonDown(JUMP) && grounded) {
                TriggerJump();
            }

            // Trigger axe swing
            if (Input.GetButtonDown(ATTACK) && canSwing) {
                attackLoading = true;
                attackButtonDownTime = Time.time;
            }

            if (Input.GetButtonUp(ATTACK) && canSwing && attackLoading) {
                attackLoading = false;

                if (!grounded) {
                    TriggerSlam();
                } else if (Time.time - attackButtonDownTime > player.statistics.tornadoChargeTime) {
                    TriggerTornado();
                } else {
                    TriggerAxeSwing();
                }
            }

            // Trigger use
            if (Input.GetButtonDown(USE)) {
                GameManager.instance.usables.currentUsable?.Use(player);
            }
        }
    }

    public void LateUpdate () {
        Vector3 localVelocity = player.transform.InverseTransformVector(rigidbody.velocity);
        player.animation.animator.SetFloat("speedX", localVelocity.x);
        player.animation.animator.SetFloat("speedZ", localVelocity.z);
    }

    private void TriggerSlam() {
        player.animation.animator.SetFloat(ATTACK_SPEED, player.axe.speed);
        player.animation.animator.SetTrigger(AXE_SLAM);
        canSwing = false;
    }

    private void TriggerAxeSwing () {
        axeCollider.enabled = true;
        player.animation.animator.SetFloat(ATTACK_SPEED, player.axe.speed);
        player.animation.animator.SetTrigger(AXE_SWING);
        canSwing = false;
    }

    private void TriggerTornado () {
        axeCollider.enabled = true;
        player.animation.animator.SetFloat(ATTACK_SPEED, player.axe.speed);
        player.animation.animator.SetTrigger(TORNADO);
        canSwing = false;
    }

    private void TriggerJump () {
        player.StartCoroutine(Jump());
    }

    private IEnumerator Jump() {
        // Animate jump
        bool ended = false;
        float startTime = Time.time;
        float startHeight = rigidbody.position.y;
        while (!ended) {
            float elapsedTime = Time.time - startTime;

            Vector3 position = rigidbody.position;
            position.y = startHeight + jumpCurve.Evaluate(elapsedTime);
            rigidbody.position = position;

            ended = elapsedTime > jumpCurve.keys[jumpCurve.length - 1].time;
            yield return null;
        }
    }

    private Vector3 GetInputSpeed () {
        Vector2 rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return new Vector3(rawInput.x, 0f, rawInput.y);
    }

    private Vector3 GetTargetedPoint () {
        Vector3 targetedPoint;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(mouseRay, out float enter)) {
            targetedPoint = mouseRay.GetPoint(enter);
        } else {
            throw new InvalidOperationException();
        }
        
        return targetedPoint;
    }

    private void AnimatorManager_OnExit (string eventName) {
        if (eventName == AXE_SWING || eventName == TORNADO) {
            axeCollider.enabled = false;
            canSwing = true;
        } else if (eventName == AXE_SLAM) {
            axeCollider.enabled = false;
            canSwing = true;

            ISet<GameObject> hitObjects = new HashSet<GameObject>();
            foreach (Collider col in Physics.OverlapSphere(player.transform.position, player.axe.slamRadius)) {
                hitObjects.Add(col.gameObject.GetRootParent());
            }

            foreach (GameObject hitObject in hitObjects) {
                IDamageable damageable = hitObject.GetComponentInParent<IDamageable>();
                if (hitObject != player.gameObject && damageable != null) {
                    damageable.Damage(new HitData() {
                        damage = player.axe.damage * 1.5f,
                        direction = Vector3.ProjectOnPlane((hitObject.transform.position - player.transform.position), Vector3.up),
                        knockback = player.axe.knockBack * 1.5f
                    });
                }
            }
        }
    }

    private void Player_OnDeath (object sender, Player e) {
        player.animation.animator.SetTrigger("Death");
    }
}
