using UnityEngine;

public static class GameObjectExtension {
    public static GameObject GetRootParent (this GameObject gameObject) {
        return (gameObject.transform.parent == null) ? gameObject : gameObject.transform.parent.gameObject.GetRootParent();
    }
}