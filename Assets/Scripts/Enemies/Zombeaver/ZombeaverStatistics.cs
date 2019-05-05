using UnityEngine;

[CreateAssetMenu(fileName ="ZombeaverStatistics", menuName ="ZombeaverStatistics")]
public class ZombeaverStatistics : ScriptableObject {
    public float maxHealth;
    public float speed;
    public float damage;
    public float attackRange;
    public float score;
    public float knockback;
}