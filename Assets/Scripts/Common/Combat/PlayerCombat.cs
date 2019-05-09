using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerCombat {
    
    private const string AXE_SWING = "AxeSwing";
    private const string TORNADO = "Tornado";
    private const string AXE_SLAM = "AxeSlam";

    private const string ATTACK_SPEED = "AttackSpeed";

    private Player player;

    public void Setup(Player player) {
        this.player = player;
        this.player.animation.OnExit += AnimatorManager_OnExit;
        this.player.axe.AxeHit += Axe_AxeHit;
    }

    public void TriggerSlam () {
        if (player.animation.currentState.Equals("Idle")) {
            player.animation.animator.SetFloat(ATTACK_SPEED, player.axe.speed);
            player.animation.animator.SetTrigger(AXE_SLAM);
        }
    }

    public void TriggerAxeSwing () {
        if (player.animation.currentState.Equals("Idle") || player.animation.currentState.Equals("AxeSwing")) {
            player.animation.animator.SetFloat(ATTACK_SPEED, player.axe.speed);
            player.animation.animator.SetTrigger(AXE_SWING);
        }
    }

    public void TriggerTornado () {
        if (player.animation.currentState.Equals("Idle")) {
            player.animation.animator.SetFloat(ATTACK_SPEED, player.axe.speed);
            player.animation.animator.SetTrigger(TORNADO);
        }
    }

    private void AnimatorManager_OnExit (string eventName) {
        if (eventName.Equals(AXE_SLAM)) {
            ISet<GameObject> hitObjects = new HashSet<GameObject>();
            foreach (Collider col in Physics.OverlapSphere(player.transform.position, player.axe.slamRadius)) {
                hitObjects.Add(col.gameObject.GetRootParent());
            }

            foreach (GameObject hitObject in hitObjects) {
                IDamageable damageable = hitObject.GetComponentInParent<IDamageable>();
                if (hitObject != player.gameObject && damageable != null) {
                    damageable.Damage(new HitData() {
                        damage = player.axe.damage * 1.5f,
                        knockback = player.axe.knockBack * 1.5f,
                        direction = Vector3.ProjectOnPlane((hitObject.transform.position - player.transform.position), Vector3.up)
                    });
                }
            }
        }
    }

    private void Axe_AxeHit (object sender, IDamageable damageable) {
        if ((object) damageable != player) {
            damageable.Damage(new HitData() {
                damage = player.axe.damage,
                knockback = player.axe.knockBack,
                direction = (damageable.gameObject.transform.position - player.axe.transform.position).normalized
            });
        }
    }
}
