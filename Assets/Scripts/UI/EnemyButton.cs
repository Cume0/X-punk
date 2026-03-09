using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyButton : MonoBehaviour
{
    [SerializeField] public Button button;

    int AttackerID;
    int DefenderID;
    TextMeshProUGUI label;

    GameEvents gameEvents;

    public void Initialize(int GivenAttackerID, int GivenDefenderID)
    {
        gameEvents = GameManager.gameEvents;

        button.onClick.AddListener(onButtonClick);

        AttackerID = GivenAttackerID;
        DefenderID = GivenDefenderID;

        label = GetComponentInChildren<TextMeshProUGUI>();
        label.text = $"{DefenderID}";
    }
    
    public void onButtonClick()
    {
        gameEvents.AttackTrigger(AttackerID, DefenderID);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(onButtonClick);
    }
    void OnDestroy()
    {
        button.onClick.RemoveListener(onButtonClick);
    }
}
