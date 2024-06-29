using System;
using System.Threading.Tasks;
using DG.Tweening;
using Enum;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
  public class EndGamePanel : MonoBehaviour
  {
    private bool _success;

    public void SetSuccess(bool success)
    {
      _success = success;
    }
    
    private async void OnEnable()
    {
      await Task.Delay(100);
      
      OpenPanel();

      (_success ? (Action)OnWin : OnLose)();
    }

    private void OpenPanel()
    {
      _winPanel.SetActive(_success);
      _losePanel.SetActive(!_success);
    }

    #region Win

    [Header("Win")]
    [SerializeField]
    private GameObject _winPanel;

    [SerializeField]
    private TextMeshProUGUI _earnedStarText;

    [SerializeField]
    private TextMeshProUGUI _earnedCoinsText;

    [SerializeField]
    private Transform _star;

    private int _earnedStar;

    private int _earnedCoin;

    private void OnWin()
    {
      int level = GameManager.Level;
      _earnedCoin = Random.Range(10 + level, 20 + level);
      _earnedStar = Random.Range(5 + level, 15 + level);

      _earnedCoinsText.text = _earnedCoin.ToString();
      _earnedStarText.text = _earnedStar.ToString();
      
      _star.DORotate(new Vector3(0, 0, 360), 4f, RotateMode.FastBeyond360)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }

    public void Continue()
    {
      int coin = SaveManager.GetInt(PlayerPrefKey.Coin);
      int star = SaveManager.GetInt(PlayerPrefKey.Star);
      
      SaveManager.SaveInt(PlayerPrefKey.Coin, coin + _earnedCoin);
      SaveManager.SaveInt(PlayerPrefKey.Star, star + _earnedStar);
      
      PanelManager.Instance.OnContinue();
    }

    public void OpenAD()
    {
      // TODO: Safak - Show AD and x3 coins
      Debug.Log("Open AD");
    }
    #endregion

    #region Lose

    [Header("Lose")]
    [SerializeField]
    private GameObject _losePanel;

    [SerializeField]
    private Button _playOnButton;

    private const int _playOnPrice = 250;
    private void OnLose()
    {
      _playOnButton.interactable = SaveManager.GetInt(PlayerPrefKey.Coin) >= _playOnPrice;
    }

    public void PlayOn()
    {
      int newCoin = SaveManager.GetInt(PlayerPrefKey.Coin);
      newCoin -= _playOnPrice;
      SaveManager.SaveInt(PlayerPrefKey.Coin, newCoin);
      
      GameManager.StartGame();
    }

    public void GiveUp()
    {
      int newHealth = SaveManager.GetInt(PlayerPrefKey.Health);
      newHealth--;
      SaveManager.SaveInt(PlayerPrefKey.Health, newHealth);
      
      PanelManager.Instance.OnContinue();
    }
    
    #endregion
  }
}