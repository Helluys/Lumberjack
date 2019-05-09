using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IDamageable {
    public PlayerStatistics statistics;
    public new AnimationManager animation;
    public Axe axe;
    [SerializeField] private Collider axeCollider;

    public event EventHandler<Player> OnDeath;

    public float health { get; private set; }
    public float carriedWood { get; private set; }
    public bool isAlive { get; private set; }

    [SerializeField] private PlayerControl playerControl;
    private new Rigidbody rigidbody;

    private void Start () {
        playerControl.Setup(this, axeCollider);
        rigidbody = GetComponent<Rigidbody>();

        health = statistics.maxHealth;
        carriedWood = 0f;
        isAlive = true;
    }

    private void Update () {
        playerControl.Update();
    }

    private void LateUpdate () {
        playerControl.LateUpdate();    
    }
    /// <summary>
    /// Trigger collider on the player is the Axe collider. It is active only while attacking.
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter (Collider collider) {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null) {
            damageable.Damage(new HitData() {
                damage = axe.damage,
                direction = (collider.transform.position - transform.position).normalized,
                knockback = axe.knockBack
            });
        }
    }

    public void Damage (HitData hitData) {
        if (isAlive) {
            health -= hitData.damage;
            rigidbody.AddForce(hitData.knockback * hitData.direction, ForceMode.Impulse);

            if (health <= 0f) {
                health = 0f;
                isAlive = false;
                rigidbody.isKinematic = true;
                OnDeath?.Invoke(this, this);
                GameManager.instance.EndLevel(false);
            }
        }
    }

    public void AddWood (float wood) {
        carriedWood += wood;
    }

    public void DepositWood(WoodStockpile stockpile) {
        float depositedWood = stockpile.DepositWood(carriedWood);
        Debug.Log("Deposited wood " + depositedWood);
        carriedWood -= depositedWood;
    }
}
