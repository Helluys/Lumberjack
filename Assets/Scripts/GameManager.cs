using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public Player player;

    [SerializeField] private LevelEnd levelEndPanel;

    private void Start () {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Two GameManagers in scene");
    }

    public void EndLevel (bool levelSuccess) {
        levelEndPanel.SetLevelEndConditions(levelSuccess);
        levelEndPanel.gameObject.SetActive(true);
    }

    public void RestartLevel () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
