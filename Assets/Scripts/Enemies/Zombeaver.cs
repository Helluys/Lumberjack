using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AnimationManager))]
public class Zombeaver : MonoBehaviour {
    private const string ATTACK = "Attack";

    [SerializeField] private ZombeaverStatistics statistics;

    private Player player;
    private new Rigidbody rigidbody;
    private AnimationManager animationManager;
    private bool canSwing = true;

    // Use this for initialization
    private void Start () {
        player = GameManager.instance.player;
        rigidbody = GetComponent<Rigidbody>();
        animationManager = GetComponent<AnimationManager>();

        animationManager.OnExit += AnimatorManager_OnExit;
    }

    public void Update () {
        Vector3 playerDelta = player.transform.position - transform.position;

        // Update translation
        rigidbody.velocity = playerDelta.normalized * statistics.speed * Mathf.Min(0.5f * playerDelta.magnitude, 1f);

        // Update rotation
        rigidbody.MoveRotation(Quaternion.LookRotation(rigidbody.velocity));

        // Trigger jump
        // TODO

        // Trigger axe swing
        if (playerDelta.magnitude < statistics.attackRange && canSwing) {
            animationManager.animator.SetTrigger(ATTACK);
            canSwing = false;
        }
    }

    private void AnimatorManager_OnExit (string eventName) {
        if (eventName == ATTACK) {
            if ((transform.position - player.transform.position).magnitude < statistics.attackRange) {
                player.Damage(statistics.damage);
                animationManager.animator.SetTrigger(ATTACK);
            } else
                canSwing = true;
        }
    }

}
