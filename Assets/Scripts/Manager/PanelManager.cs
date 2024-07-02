using System.Threading.Tasks;
using DG.Tweening;
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

    public RectTransform AllPanels;

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

    private const float _animationTime = 0.65f;
    private async void OnGameStarted()
    {
      OpenOrClose(true, false, true, true);

      await Task.Delay(100);

      PlayableArea.transform.DOLocalMoveX(0, _animationTime);
      AllPanels.DOLocalMoveX(-Screen.width, _animationTime).OnComplete(() =>
      {
        OpenOrClose(false, false, true, true);
      });
    }

    private void OnGameFinished(bool success)
    {
      OpenOrClose(false, true, false, false);
      
      EndGamePanel.SetSuccess(success);
    }

    public void OnContinue()
    {
      OpenOrClose(true, true, false, false);

      PlayableArea.transform.DOLocalMoveX(9, _animationTime);
      AllPanels.DOLocalMoveX(0, _animationTime).OnComplete(() =>
      {
        OpenOrClose(true, false, false, false);
      });
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