using UnityEngine;
using UnityEngine.UI;

public class LevelEnd : MonoBehaviour {

    [SerializeField] private Text successText;

    public void SetLevelEndConditions (bool levelSuccess) {
        successText.text = levelSuccess ? "Success" : "Failure";
    }

}
