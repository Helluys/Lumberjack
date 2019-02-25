using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public Player player;

    private void Start () {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Two GameManagers in scene");
    }

}
