using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AnimationManager))]
public class Player : MonoBehaviour, IDamageable {
    public PlayerStatistics statistics;

    public event EventHandler<Player> OnDeath;

    public float health { get; private set; }
    public float carriedWood { get; private set; }
    public bool isAlive { get; private set; }

    [SerializeField] private Collider axeCollider;

    private PlayerControl playerControl;

    private void Start () {
        playerControl = new PlayerControl(this, axeCollider);

        health = statistics.maxHealth;
        carriedWood = 0f;
        isAlive = true;
    }

    private void Update () {
        playerControl.Update();
    }

    private void OnTriggerEnter (Collider collider) {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null) {
            damageable.Damage(statistics.axeDamage);
        }
    }

    public void Damage (float damage) {
        if (isAlive) {
            health -= damage;

            if (health <= 0f) {
                health = 0f;
                isAlive = false;
                OnDeath?.Invoke(this, this);
                GameManager.instance.EndLevel(false);
            }
        }
    }

    public void AddWood (float wood) {
        carriedWood += wood;
    }
}
