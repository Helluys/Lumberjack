using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class ZombeaverSpawner : MonoBehaviour {

    [SerializeField] private GameObject zombeaverPrefab;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float activationRange;

    private BoxCollider boxCollider;
    private List<GameObject> spawnedZombeavers = new List<GameObject>();
    private WaitForSeconds waitForDelay;
    private WaitUntil waitUntilActive;

    private void Start () {
        boxCollider = GetComponent<BoxCollider>();
        waitForDelay = new WaitForSeconds(spawnDelay);
        waitUntilActive = new WaitUntil(() => IsActive());

        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine () {
        while (true) {
            yield return waitUntilActive;
            while (IsActive()) {
                yield return waitForDelay;
                GameObject spawnedZombeaver = Instantiate(zombeaverPrefab, GetSpawnPoint(), Quaternion.identity);
                spawnedZombeavers.Add(spawnedZombeaver);
                spawnedZombeaver.GetComponent<Zombeaver>().OnDeath += Zombeaver_OnDeath;
            }
        }
    }

    private Vector3 GetSpawnPoint () {
        return transform.TransformPoint(boxCollider.center + new Vector3((Random.value - .5f) * boxCollider.size.x, 0f, (Random.value - .5f) * boxCollider.size.z));
    }

    private bool IsActive () {
        return (transform.position - GameManager.instance.player.transform.position).magnitude < activationRange;
    }

    private void Zombeaver_OnDeath (object sender, Zombeaver e) {
        spawnedZombeavers.Remove(e.gameObject);
    }

    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(GetComponent<BoxCollider>().center), activationRange);
    }

}
