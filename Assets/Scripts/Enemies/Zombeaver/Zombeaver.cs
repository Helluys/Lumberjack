using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Zombeaver : MonoBehaviour, IDamageable, IKnockable {

    private const string ATTACK = "Attack";

    public event System.EventHandler<Zombeaver> OnDeath;

    [SerializeField] private AnimationManager animationManager = null;
    [SerializeField] private float health = 0f;
    [SerializeField] private ZombeaverStatistics statistics = null;

    private Player player;
    private new Rigidbody rigidbody;
    private bool canSwing = true;

    private float currentSpeed = 0f;

    private void Start () {
        player = GameManager.instance.player;
        rigidbody = GetComponent<Rigidbody>();

        animationManager.OnExit += AnimatorManager_OnExit;

        health = statistics.maxHealth;
    }

    public void Update () {
        animationManager.animator.SetFloat("Speed", currentSpeed);
    }

    public void FixedUpdate () {
        Vector3 playerDelta = player.transform.position - transform.position;

        // Update translation
        currentSpeed = statistics.speed * Mathf.Min(0.5f * playerDelta.magnitude, 1f);
        transform.position += currentSpeed * Time.fixedDeltaTime * playerDelta.normalized;
        
        // Update rotation
        transform.rotation = Quaternion.LookRotation(playerDelta);

        // Trigger attack
        if (playerDelta.magnitude < statistics.attackRange && canSwing) {
            animationManager.animator.SetTrigger(ATTACK);
            canSwing = false;
        }
    }

    private void AnimatorManager_OnExit (string eventName) {
        if (eventName == ATTACK) {
            Vector3 playerDelta = player.transform.position - transform.position;

            if (playerDelta.magnitude < statistics.attackRange) {
                player.Damage(new HitData() {
                    damage = statistics.damage,
                    direction = playerDelta.normalized,
                    knockback = statistics.knockback
                });

                GameManager.instance.ModifyScore(-statistics.damage);
                animationManager.animator.SetTrigger(ATTACK);
            } else
                canSwing = true;
        }
    }

    public void Damage (HitData hitData) {
        health -= hitData.damage;
        rigidbody.AddForce(hitData.knockback * hitData.direction, ForceMode.Impulse);

        if (health < 0f) {
            health = 0f;
            OnDeath?.Invoke(this, this);
            GameManager.instance.ModifyScore(statistics.score);
            Destroy(gameObject);
        }
    }

    public void KnockBack (Vector3 force) {
        rigidbody.AddForce(force, ForceMode.Impulse);
    }
}
