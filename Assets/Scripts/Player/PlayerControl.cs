using System;
using UnityEngine;

[Serializable]
public class PlayerControl {
    private const string AXE_SWING = "AxeSwing";
    private const string ATTACK = "Attack";

    private Player player;
    private Rigidbody rigidbody;
    private AnimationManager animationManager;
    private Collider axeCollider;
    private bool canSwing = true;

    public PlayerControl (Player player, Collider axeCollider) {
        this.player = player;
        rigidbody = player.GetComponent<Rigidbody>();
        animationManager = player.GetComponent<AnimationManager>();
        this.axeCollider = axeCollider;

        animationManager.OnExit += AnimatorManager_OnExit;
    }

    public void Update () {
        // Update translation
        rigidbody.velocity = GetInputSpeed() * player.statistics.movementSpeed;

        // Update rotation
        try {
            rigidbody.MoveRotation(Quaternion.LookRotation(GetTargetedPoint() - rigidbody.transform.position));
        } catch (InvalidOperationException) {
            Debug.LogWarning("No point targeted by cursor");
        }

        // Trigger jump
        // TODO

        // Trigger axe swing
        if (Input.GetButtonDown(ATTACK) && canSwing) {
            TriggerAxeSwing();
        }
    }

    private void TriggerAxeSwing () {
        axeCollider.enabled = true;
        animationManager.animator.SetTrigger(AXE_SWING);
        canSwing = false;
    }

    private Vector3 GetInputSpeed () {
        Vector2 rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return new Vector3(rawInput.x * Mathf.Sqrt(1f - 0.5f * rawInput.y), 0f, rawInput.y * Mathf.Sqrt(1f - 0.5f * rawInput.x));
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
        if (eventName == AXE_SWING) {
            if (Input.GetButton(ATTACK))
                animationManager.animator.SetTrigger(AXE_SWING);
            else {
                axeCollider.enabled = false;
                canSwing = true;
            }
        }
    }
}
