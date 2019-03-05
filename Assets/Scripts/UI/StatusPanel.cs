using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour {

    private Player player;

    [SerializeField] private Text healthText;
    [SerializeField] private Text woodText;
    [SerializeField] private Text scoreText;

    private void Start () {
        player = GameManager.instance.player;
    }

    private void Update () {
        healthText.text = "Health: " + player.health + "/" + player.statistics.maxHealth;
        woodText.text = "Wood: " + player.carriedWood;
        scoreText.text = "Score: " + GameManager.instance.score;
    }
}
