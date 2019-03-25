using System;
using UnityEngine;

[Serializable]
public class PlayerControl {
    private const string AXE_SWING = "AxeSwing";
    private const string TORNADO = "Tornado";
    private const string ATTACK = "Attack";

    private Player player;
    private Rigidbody rigidbody;
    private Collider axeCollider;
    private bool canSwing = true;

    private bool attackLoading;
    private float attackButtonDownTime;

    public PlayerControl (Player player, Collider axeCollider) {
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
                rigidbody.rotation = Quaternion.LookRotation(GetTargetedPoint() - rigidbody.transform.position);
            } catch (InvalidOperationException) {
                Debug.LogWarning("No point targeted by cursor");
            }

            // Trigger jump
            // TODO

            // Trigger axe swing
            if (Input.GetButtonDown(ATTACK) && canSwing) {
                attackLoading = true;
                attackButtonDownTime = Time.time;
            }

            if (Input.GetButtonUp(ATTACK) && canSwing && attackLoading) {
                attackLoading = false;

                if (Time.time - attackButtonDownTime > player.statistics.tornadoChargeTime) {
                    TriggerTornado();
                } else {
                    TriggerAxeSwing();
                }
            }
        }
    }

    private void TriggerAxeSwing () {
        axeCollider.enabled = true;
        player.animation.animator.SetTrigger(AXE_SWING);
        canSwing = false;
    }

    private void TriggerTornado () {
        axeCollider.enabled = true;
        player.animation.animator.SetTrigger(TORNADO);
        canSwing = false;
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
        }
    }

    private void Player_OnDeath (object sender, Player e) {
        player.animation.animator.SetTrigger("Death");
    }
}
