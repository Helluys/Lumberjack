using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AnimationManager))]
public class Player : MonoBehaviour, IDamageable {
    public PlayerStatistics statistics;

    [SerializeField] private Collider axeCollider;

    private PlayerControl playerMovement;

    // Start is called before the first frame update
    private void Start () {
        playerMovement = new PlayerControl(this, axeCollider);
        statistics = Instantiate(statistics);
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
        statistics.health -= damage;
    }

    public void AddWood (float wood) {
        statistics.carriedWood += wood;
    }
}
