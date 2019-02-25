using UnityEngine;
[ExecuteInEditMode]
public class ObjectTracker : MonoBehaviour {

    [SerializeField] private Transform trackedObject = null;
    [SerializeField] private float distance = 5;
    [SerializeField] private float elevation = 5;
    [SerializeField] private float lookAngle = 45f;

    private void Update () {
        if (trackedObject != null) {
            Vector3 positionDelta = Quaternion.Euler(0f, lookAngle, 0f) * Vector3.back * distance;
            Vector3 position = trackedObject.position + positionDelta;
            position.y = elevation;
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(-positionDelta - elevation * Vector3.up);
        }
    }

    private void OnValidate () {
        Update();
    }
}
