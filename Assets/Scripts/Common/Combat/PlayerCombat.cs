using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerCombat {
    
    private const string AXE_SWING = "AxeSwing";
    private const string TORNADO = "Tornado";
    private const string AXE_SLAM = "AxeSlam";
    private const string AXE_KICK = "AxeKick";

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

    public void TriggerKick() {
        if (player.animation.currentState.Equals("Idle")) {
            player.animation.animator.SetFloat(ATTACK_SPEED, player.axe.speed);
            player.animation.animator.SetTrigger(AXE_KICK);
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
                    Debug.Log("Slam hit " + damageable);
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
        switch (player.animation.currentState) {
            case AXE_SWING:
                if ((object) damageable != player) {
                    Debug.Log("Swing hit " + damageable);
                    damageable.Damage(new HitData() {
                        damage = player.axe.damage,
                        knockback = player.axe.knockBack / 2f,
                        direction = (damageable.gameObject.transform.position - player.axe.transform.position).normalized
                    });
                }
                break;
            case TORNADO:
                if ((object) damageable != player) {
                    Debug.Log("Tornado hit " + damageable);
                    damageable.Damage(new HitData() {
                        damage = player.axe.damage * 2f,
                        knockback = player.axe.knockBack,
                        direction = (damageable.gameObject.transform.position - player.axe.transform.position).normalized
                    });
                }
                break;
            case AXE_KICK:
                if ((object) damageable != player) {
                    Debug.Log("Kick hit " + damageable);
                    damageable.Damage(new HitData() {
                        damage = player.axe.damage / 10f,
                        knockback = player.axe.knockBack * 3f,
                        direction = (damageable.gameObject.transform.position - player.axe.transform.position).normalized
                    });
                }
                break;
        }
    }
}
