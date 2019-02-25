using UnityEngine;

public class Tree : MonoBehaviour, IDamageable {

    [SerializeField] private float health;
    [SerializeField] private float wood;

    private void Start () {
        gameObject.tag = "Tree";
    }

    public void Damage (float damage) {
        health -= damage;
        if (health < 0) {
            GameManager.instance.player.AddWood(wood);
            Destroy(gameObject);
        }
    }

}
