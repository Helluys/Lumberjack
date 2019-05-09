
using UnityEngine;

public interface IDamageable {

    GameObject gameObject { get; }

    void Damage (HitData hitData);

}
