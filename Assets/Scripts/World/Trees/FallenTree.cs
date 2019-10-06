using UnityEngine;

public class FallenTree : MonoBehaviour, IUsable {

    public float wood;

    [SerializeField] private float _range = 0f;
    public float range { get { return _range; } }

    private void Start () {
        GameManager.instance.usables.AddUsable(this);
    }

    private void OnDestroy () {
        GameManager.instance.usables.RemoveUsable(this);
    }

    public void Use (Player player) {
        player.AddWood(wood);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
