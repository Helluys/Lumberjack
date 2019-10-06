using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour {

    private Player player;

    [SerializeField] private Text healthText = null;
    [SerializeField] private Text woodText = null;
    [SerializeField] private Text scoreText = null;

    private void Start () {
        player = GameManager.instance.player;
    }

    private void Update () {
        healthText.text = "Health: " + player.health + "/" + player.statistics.maxHealth;
        woodText.text = "Wood: " + player.carriedWood;
        scoreText.text = "Score: " + GameManager.instance.score + "/" + GameManager.instance.GetWinConditionScore();
    }
}
