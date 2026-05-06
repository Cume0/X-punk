using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] public Button button;

    GameEvents gameEvents;

    public void Initialize()
    {
        gameEvents = GameManager.gameEvents;
        button.onClick.AddListener(onButtonClick);
    }

    public void onButtonClick()
    {
        gameEvents.EndTurn();
    }
}
