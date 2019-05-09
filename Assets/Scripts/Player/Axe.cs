using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Axe : MonoBehaviour {
    public float damage;
    public float knockBack;
    public float range;
    public float speed;
    public float slamRadius;

    public event EventHandler<IDamageable> AxeHit;

    private new Rigidbody rigidbody;

    private void Start () {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter (Collider collider) {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null)
            AxeHit?.Invoke(this, damageable);
    }

}
