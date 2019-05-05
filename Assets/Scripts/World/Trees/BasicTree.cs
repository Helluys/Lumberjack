using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicTree : MonoBehaviour, IDamageable {

    private const float FALLING_TORQUE = 100f;

    [SerializeField] private float health;
    [SerializeField] private float wood;
    [SerializeField] private int fallenDivisions;
    [SerializeField] private GameObject fallenTreePrefab;
    [SerializeField] private AnimationCurve fallCurve;
    [SerializeField] private float fallingHitRange;

    private new Rigidbody rigidbody;

    private TreeState state = TreeState.ALIVE;

    private void Start () {
        gameObject.tag = "Tree";

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = transform.InverseTransformPoint(GetComponent<Collider>().bounds.center);

    }

    public void Damage (HitData hitData) {
        if (state == TreeState.ALIVE) {

            health -= hitData.damage;

            if (health < 0) {
                StartCoroutine(Fall(hitData));
            }
        }
    }

    private System.Collections.IEnumerator Fall (HitData hitData) {
        state = TreeState.FALLING;

        // Animate falling tree
        bool ended = false;
        Quaternion originalRotation = transform.rotation;
        Quaternion fallRotation = Quaternion.AngleAxis(90f, Vector3.Cross(hitData.direction, Vector3.up));
        float startTime = Time.time;
        while (!ended) {
            float elapsedTime = Time.time - startTime;
            transform.rotation = Quaternion.Lerp(Quaternion.identity, fallRotation, fallCurve.Evaluate(elapsedTime)) * originalRotation;
            ended = elapsedTime > fallCurve.keys[fallCurve.length - 1].time;
            yield return null;
        }

        // Falling ended
        state = TreeState.FALLEN;

        DamageOnFall();
        SpawnFallenTree();

        Destroy(gameObject);
    }

    private void DamageOnFall () {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        float radius = capsule.radius + fallingHitRange;
        Vector3 bottom = transform.TransformPoint(capsule.center - 0.5f * capsule.height * Vector3.up);
        Vector3 top = transform.TransformPoint(capsule.center + 0.5f * capsule.height * Vector3.up);

        ISet<GameObject> hitObjects = new HashSet<GameObject>();
        foreach (Collider col in Physics.OverlapCapsule(bottom, top, radius)) {
            hitObjects.Add(col.gameObject.GetRootParent());
        }

        foreach (GameObject hitObject in hitObjects) {
            IDamageable damageable = hitObject.GetComponentInParent<IDamageable>();
            if (damageable != null) {
                damageable.Damage(new HitData() {
                    damage = rigidbody.mass * 0.5f,
                    direction = Vector3.ProjectOnPlane((hitObject.transform.position - transform.position), transform.up),
                    knockback = rigidbody.mass * 0.5f
                });
            }
        }
    }

    private void SpawnFallenTree () {
        float height = GetComponent<CapsuleCollider>().height;
        for (int i = 0; i < fallenDivisions; i++) {
            Vector3 position = transform.position + height * ((i + 0.5f) / fallenDivisions) * transform.TransformDirection(Vector3.up);
            FallenTree fallenTree = Instantiate(fallenTreePrefab, position, transform.rotation).GetComponent<FallenTree>();
            fallenTree.wood = wood / fallenDivisions;
        }
    }

    private void OnDrawGizmosSelected () {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        float radius = capsule.radius + fallingHitRange;
        Vector3 bottom = transform.TransformPoint(capsule.center - 0.5f * capsule.height * Vector3.up);
        Vector3 top = transform.TransformPoint(capsule.center + 0.5f * capsule.height * Vector3.up);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bottom, radius);
        Gizmos.DrawWireSphere(top, radius);
        Gizmos.DrawLine(top + transform.rotation * (radius * Vector3.right), bottom + transform.rotation * (radius * Vector3.right));
        Gizmos.DrawLine(top + transform.rotation * (radius * Vector3.left), bottom + transform.rotation * (radius * Vector3.left));
        Gizmos.DrawLine(top + transform.rotation * (radius * Vector3.forward), bottom + transform.rotation * (radius * Vector3.forward));
        Gizmos.DrawLine(top + transform.rotation * (radius * Vector3.back), bottom + transform.rotation * (radius * Vector3.back));
    }

}
