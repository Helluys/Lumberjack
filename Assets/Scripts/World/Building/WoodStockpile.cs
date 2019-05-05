using UnityEngine;

public class WoodStockpile : Building, IUsable {

    [SerializeField] private float maxWood;

    public float wood { get; private set; }

    [SerializeField] private float _range;
    public float range { get { return _range; } }

    public float DepositWood (float amount) {
        float depositedWood = Mathf.Min(amount, maxWood - wood);
        wood += depositedWood;
        return depositedWood;
    }

    public void Use (Player player) {
        player.DepositWood(this);
    }

    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
