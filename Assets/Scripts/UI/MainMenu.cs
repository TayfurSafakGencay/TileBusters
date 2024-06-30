using Enum;
using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
  public class MainMenu : MonoBehaviour
  {
    [SerializeField]
    private GameObject _battlePassPanel;
    
    [SerializeField]
    private TextMeshProUGUI _healthText;

    [SerializeField]
    private TextMeshProUGUI _coinText;

    [SerializeField]
    private TextMeshProUGUI _starText;

    [SerializeField]
    private TextMeshProUGUI _levelButtonText;
    
    public void OnGameStart()
    {
      GameManager.StartGame();
    }

    private void OnEnable()
    {
      _battlePassPanel.SetActive(false);
      
      _healthText.text = SaveManager.GetInt(PlayerPrefKey.Health).ToString();
      _coinText.text = SaveManager.GetInt(PlayerPrefKey.Coin).ToString();
      _starText.text = SaveManager.GetInt(PlayerPrefKey.Star).ToString();

      _levelButtonText.text = "Level " + SaveManager.GetInt(PlayerPrefKey.Level);
    }

    public void OpenBattlePassPanel()
    {
      _battlePassPanel.SetActive(true);
    }
  }
}