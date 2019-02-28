using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AnimationManager))]
public class Player : MonoBehaviour, IDamageable {
    public PlayerStatistics statistics;

    public float health { get; private set; }
    public float carriedWood { get; private set; }

    [SerializeField] private Collider axeCollider;

    private PlayerControl playerMovement;

    // Start is called before the first frame update
    private void Start () {
        playerMovement = new PlayerControl(this, axeCollider);
    }

    // Update is called once per frame
    private void Update () {
        playerMovement.Update();
    }

    private void OnTriggerEnter (Collider collider) {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null) {
            damageable.Damage(statistics.axeDamage);
        }
    }

    public void Damage (float damage) {
        health -= damage;
    }

    public void AddWood (float wood) {
        carriedWood += wood;
    }
}
