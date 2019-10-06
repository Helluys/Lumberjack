using System;
using System.Collections;

using UnityEngine;

[Serializable]
public class PlayerControl {

    private const string ATTACK = "Attack";
    private const string ATTACK_SECONDARY = "AttackSecondary";
    private const string USE = "Use";
    private const string JUMP = "Jump";

    private Player player;
    private Rigidbody rigidbody;

    [SerializeField] private float groundedDistance = 0f;
    [SerializeField] private AnimationCurve jumpCurve = null;

    private bool attackLoading;
    private float attackButtonDownTime;

    public void Setup (Player player) {
        this.player = player;
        rigidbody = player.GetComponent<Rigidbody>();

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

            // Record click time
            if (Input.GetButtonDown(ATTACK)) {
                attackButtonDownTime = Time.time;
            }

            // Trigger attacks
            if (Input.GetButtonUp(ATTACK)) {
                if (!grounded) {
                    player.combat.TriggerSlam();
                } else if (Time.time - attackButtonDownTime > player.statistics.tornadoChargeTime) {
                    player.combat.TriggerTornado();
                } else {
                    player.combat.TriggerAxeSwing();
                }
            }

            // Trigger secondary attack
            if (Input.GetButtonDown(ATTACK_SECONDARY)) {
                player.combat.TriggerKick();
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

    private void TriggerJump () {
        player.animation.animator.SetTrigger("Jump");
        player.StartCoroutine(Jump());
    }

    private IEnumerator Jump () {
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
        // Convert raw input (square) to normalized input (circle)
        Vector2 rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return new Vector3(rawInput.x * Mathf.Sqrt(1f - 0.5f * rawInput.y * rawInput.y), 0f, rawInput.y * Mathf.Sqrt(1f - 0.5f * rawInput.x * rawInput.x));
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

    private void Player_OnDeath (object sender, Player e) {
        player.animation.animator.SetTrigger("Death");
    }
}
