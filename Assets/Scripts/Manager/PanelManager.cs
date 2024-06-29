using UI;
using UnityEngine;

namespace Manager
{
  public class PanelManager : MonoBehaviour
  {
    public static PanelManager Instance;
    
    public MainMenu MainMenu;

    public InnerGamePanel InnerGamePanel;

    public EndGamePanel EndGamePanel;

    public GameObject PlayableArea;

    private void Awake()
    {
      Instance = this;
      
      OpenOrClose(true, false, false, false);
    }

    private void Start()
    {
      GameManager.GameStarted += OnGameStarted;
      GameManager.GameFinished += OnGameFinished;
    }

    private void OnGameStarted()
    {
      OpenOrClose(false, false, true, true);
    }

    private void OnGameFinished(bool success)
    {
      OpenOrClose(false, true, false, false);
      
      EndGamePanel.SetSuccess(success);
    }

    public void OnContinue()
    {
      OpenOrClose(true, false, false,false);
    }

    private void OpenOrClose(bool mainMenu, bool endGame, bool innerGame, bool playableArea)
    {
      MainMenu.gameObject.SetActive(mainMenu);
      EndGamePanel.gameObject.SetActive(endGame);
      InnerGamePanel.gameObject.SetActive(innerGame);
      PlayableArea.SetActive(playableArea);
    }
  }
}