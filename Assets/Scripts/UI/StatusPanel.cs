using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour {

    private Player player;

    [SerializeField] private Text healthText;
    [SerializeField] private Text woodText;

    private void Start () {
        player = GameManager.instance.player;
    }

    private void Update () {
        healthText.text = "Health: " + player.health + "/" + player.statistics.maxHealth;
        woodText.text = "Wood: " + player.carriedWood;
    }
}
