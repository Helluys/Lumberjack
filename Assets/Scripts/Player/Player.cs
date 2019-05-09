using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IDamageable {
    public PlayerStatistics statistics;
    public PlayerCombat combat;
    public Axe axe;
    public new AnimationManager animation;
    [SerializeField] private Transform axeHolder;

    public event EventHandler<Player> OnDeath;

    public float health { get; private set; }
    public float carriedWood { get; private set; }
    public bool isAlive { get; private set; }

    [SerializeField] private PlayerControl playerControl;
    private new Rigidbody rigidbody;

    private void Start () {
        combat.Setup(this);
        playerControl.Setup(this);
        rigidbody = GetComponent<Rigidbody>();

        health = statistics.maxHealth;
        carriedWood = 0f;
        isAlive = true;
    }

    private void Update () {
        playerControl.Update();
        axe.transform.position = axeHolder.transform.position;
        axe.transform.rotation = axeHolder.transform.rotation;
    }

    private void LateUpdate () {
        playerControl.LateUpdate();    
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
