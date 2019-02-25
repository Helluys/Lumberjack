using UnityEngine;

[CreateAssetMenu(fileName ="PlayerStatistics", menuName ="PlayerStatistics")]
public class PlayerStatistics : ScriptableObject {
    public float maxHealth;
    public float health;

    public float movementSpeed;

    public float axeDamage;
    public float carriedWood;
}
