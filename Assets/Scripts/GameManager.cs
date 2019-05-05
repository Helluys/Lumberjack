using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public Player player;
    public Usables usables { get; private set; }

    public float score { get; private set; }

    [SerializeField] private float winConditionScore;
    [SerializeField] private LevelEnd levelEndPanel;

    private void Start () {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Two GameManagers in scene");

        usables = new Usables();
        usables.GrabAllUsables();
    }

    public void EndLevel (bool levelSuccess) {
        levelEndPanel.SetLevelEndConditions(levelSuccess);
        levelEndPanel.gameObject.SetActive(true);
    }

    public void RestartLevel () {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public float GetWinConditionScore () {
        return winConditionScore;
    }

    public void ModifyScore (float amount) {
        score += amount;

        if (score > winConditionScore) {
            EndLevel(true);
        }
    }

}
