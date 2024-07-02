using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Enum;
using Manager;
using TMPro;
using UI.BattlePass;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Vo;
using Random = UnityEngine.Random;

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

    [SerializeField]
    private GameObject _title;

    public void OnGameStart()
    {
      GameManager.StartGame();
    }

    private async void OnEnable()
    {
      _title.transform.DOScale(1.3f, 1f)
        .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
      
      _battlePassPanel.SetActive(false);

      await Task.Delay(50);
      
      _healthText.text = SaveManager.GetInt(PlayerPrefKey.Health).ToString();
      _coinText.text = SaveManager.GetInt(PlayerPrefKey.Coin).ToString();
      _starText.text = SaveManager.GetInt(PlayerPrefKey.Star).ToString();

      _levelButtonText.text = "Level " + SaveManager.GetInt(PlayerPrefKey.Level);
    }

    public void OpenBattlePassPanel()
    {
      _battlePassPanel.SetActive(true);
    }

    #region BattlePass Animations

    private readonly List<BattlePassItemVo> _battlePassItems = new();

    public void AddBattlePassItem(BattlePassItemVo battlePassItemVo)
    {
      _battlePassItems.Add(battlePassItemVo);
    }

    public async void ExecuteAnimation()
    {
      await Task.Delay(250);
      
      await PlayAnimationsSequentially();
    }
    
    private async Task PlayAnimationsSequentially()
    {
      for (int i = 0; i < _battlePassItems.Count; i++)
      {
        await AnimationParameters(_battlePassItems[i]);
      }
      
      _battlePassItems.Clear();
    }

    [Header("Targets")]

    [SerializeField]
    private Transform _heart;

    [SerializeField]
    private Transform _coin;
    
    private async Task AnimationParameters(BattlePassItemVo battlePassItemVo)
    {
      switch (battlePassItemVo.BattlePassItemType)
      {
        case BattlePassItemType.Coin:
          await PlayAnimation(_coin, _coinText, 10, _coinAnimationObject, battlePassItemVo.Amount, PlayerPrefKey.Coin, _animationTime);
          break;
        case BattlePassItemType.Health:
          await PlayAnimation(_heart, _healthText, battlePassItemVo.Amount, _heartAnimationObject, battlePassItemVo.Amount, PlayerPrefKey.Health, 0);
          break;
        case BattlePassItemType.Skin:
          await SkinAnimation(battlePassItemVo.Amount);
          break;
      }
    }

    [Header("Animation Objects")]
    
    [SerializeField]
    private GameObject _coinAnimationObject;

    [SerializeField]
    private GameObject _heartAnimationObject;


    private const float _animationTime = 1.5f;
    
    private async Task PlayAnimation(Transform target, TextMeshProUGUI text, int itemAmount, GameObject animationObject, int amount, PlayerPrefKey playerPrefKey, float textTime)
    {
      for (int i = 0; i < itemAmount; i++)
      {
        Vector3 position = new(Random.Range(-200, 200), Random.Range(-200, 200), 0);

        GameObject item = Instantiate(animationObject, Vector3.zero, quaternion.identity, transform);
        item.transform.localPosition = position;

        if (i == 0)
        {
          item.transform.DOMove(target.position, _animationTime).SetEase(Ease.InBack).OnComplete(() =>
          {
            UpdateText(text, playerPrefKey, textTime, amount);
            GrowAndShrinkAnimation(target);
            Destroy(item);
          });
        }
        else if (i + 1 == itemAmount)
        {
          await item.transform.DOMove(target.position, _animationTime).SetEase(Ease.InBack).OnComplete(() =>
          {
            GrowAndShrinkAnimation(target);
            Destroy(item);
          }).AsyncWaitForCompletion();
        }
        else
        {
          item.transform.DOMove(target.position, _animationTime).SetEase(Ease.InBack).OnComplete(() =>
          {
            GrowAndShrinkAnimation(target);
            Destroy(item);
          });
        }

        await Task.Delay(100);
      }
    }
    
    private const float _endScale = 1.25f;

    private const float _initialAnimationTime = 0.1f;
    
    private void GrowAndShrinkAnimation(Transform target)
    {
      target.DOScale(_endScale, _initialAnimationTime)
        .OnComplete(() => 
        {
          target.DOScale(1, _initialAnimationTime);
        });
    }

    private void UpdateText(TextMeshProUGUI counterText, PlayerPrefKey playerPrefKey, float time, int amount)
    {
      int data = SaveManager.GetInt(playerPrefKey);
      int ff = data;
      
      DOTween.To(() => ff, x => ff = x, amount + data, time).OnUpdate(() =>
      {
        counterText.text = ff.ToString("f0");
      });

      SaveManager.SaveInt(playerPrefKey, amount + data);
    }

    [SerializeField]
    private GameObject _skinAnimationObject;
    private async Task SkinAnimation(int skinIndex)
    {
      SaveManager.SaveInt(PlayerPrefKey.Skin, skinIndex);
      GameManager.Instance.TileManager.UpdateSkin(skinIndex);

      List<TileSpriteVo> spriteVos = GameManager.Instance.TileManager.GetTilesSprites();

      int x = -300;
      int y = 75;
      
      for (int s = 0; s < spriteVos.Count; s++)
      {
        if (spriteVos.ElementAt(s).TileKey == TileKey.Empty) continue;

        GameObject instantiate = Instantiate(_skinAnimationObject, Vector3.zero, quaternion.identity, transform);
        Image image = instantiate.GetComponent<Image>();
        instantiate.transform.localPosition = new Vector3(x, y, 0);

        image.color = new Color(1, 1, 1, 0);
        image.sprite = spriteVos.ElementAt(s).Sprite;
        image.DOFade(1, Random.Range(0.75f, 1.25f)).OnComplete(() =>
        {
          image.DOFade(0, Random.Range(0.75f, 1.25f)).OnComplete(() =>
          {
            Destroy(instantiate);
          });
        });
        
        x += 150;
        if (x <= 300) continue;
        x = -300;
        y = -75;
      }

      await Task.Delay(1500);
    }

  #endregion
  }
}