using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Enum;
using Manager;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattlePass
{
  public class BattlePass : MonoBehaviour
  {
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private TextMeshProUGUI _starAmountText;

    [SerializeField]
    private RectTransform _paidItemPool;

    [SerializeField]
    private RectTransform _freeItemPool;

    [SerializeField]
    private GameObject _paidItem;

    [SerializeField]
    private GameObject _freeItem;

    public List<BattlePassItemVo> BattlePassItemVos;

    public Dictionary<int, BattlePassItemVo> BattlePassItemData = new();

    private int _battlePass;

    private int _starCount;

    public static Action<int> Reached;

    private void Awake()
    {
      for (int i = 0; i <  BattlePassItemVos.Count; i++)
      {
        BattlePassItemVo vo = BattlePassItemVos[i];
        vo.ID = i;

        BattlePassItemData.Add(i, vo);
      }
    }

    public static Action BattlePassPanelOpened;
    private void OnEnable()
    {
      _slider.value = 0;
      
      BattlePassPanelOpened?.Invoke();
      
      _battlePass = SaveManager.GetInt(PlayerPrefKey.BattlePass);
      _starCount = SaveManager.GetInt(PlayerPrefKey.Star);

      _buyBattlePassButton.gameObject.SetActive(_battlePass != 1);
      if (_battlePass == 1) _battlePassLockImage.sprite = _goldUnlockSprite;

      InstantiateItem();
    }

    private async void InstantiateItem()
    {
      List<BattlePassItem> items = new();
      for (int i = 0; i < BattlePassItemData.Count; i++)
      {
        BattlePassItemVo vo = BattlePassItemData.ElementAt(i).Value;
        
        RectTransform parent = vo.IsPaid ? _paidItemPool : _freeItemPool;
        GameObject itemObject = vo.IsPaid ? _paidItem : _freeItem;

        GameObject item = Instantiate(itemObject, new Vector3(0, 0, 0), quaternion.identity, parent);
        item.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        BattlePassItem battlePassItem = item.GetComponent<BattlePassItem>();
        battlePassItem.SetData(vo, this);
        items.Add(battlePassItem);
      }

      for (int i = 0; i < items.Count; i++)
      {
        items[i].InitialAnimation();
        
        await Task.Delay(350);
      }
      
      SliderAnimation();
    }

    private void SliderAnimation()
    {
      _reachedItem = 0;
      
      _slider.DOValue(_starCount, 1)
        .SetEase(Ease.Linear)
        .OnUpdate(UpdatePanel);
    }

    private int _reachedItem;

    public static Action<int> IsItemActive;
    private void UpdatePanel()
    {
      float value = _slider.value;
      
      _starAmountText.text = value.ToString("f0") + " / " + BattlePassItemData[_reachedItem].NecessaryStarAmount.ToString("f0");

      if (value >= BattlePassItemData[_reachedItem].NecessaryStarAmount)
      {
        if (BattlePassItemData[_reachedItem].Activated)
        {
          IsItemActive.Invoke(BattlePassItemData[_reachedItem].ID);
        }
        
        if (_battlePass == 0)
        {
          if (BattlePassItemData[_reachedItem].IsPaid)
          {
            _reachedItem++;
          }
          else
          {
            Reached.Invoke(_reachedItem);
            
            _reachedItem++;
          }
        }
        else
        {
          Reached.Invoke(_reachedItem);
            
          _reachedItem++;
        }
      }
    }

    [SerializeField]
    private Button _buyBattlePassButton;

    [SerializeField]
    private Image _battlePassLockImage;

    [SerializeField]
    private Sprite _goldUnlockSprite;
    
    public void BuyBattlePass()
    {
      SaveManager.SaveInt(PlayerPrefKey.BattlePass, 1);
      _battlePass = SaveManager.GetInt(PlayerPrefKey.BattlePass);

      _battlePassLockImage.sprite = _goldUnlockSprite;
      _buyBattlePassButton.gameObject.SetActive(false);
      
      _slider.value = 0;
      SliderAnimation();
    }

    public void ActivatedItem(int id, BattlePassItemVo battlePassItemVo)
    {
      BattlePassItemData[id].Activated = true;
      
      GameManager.Instance.PanelManager.MainMenu.AddBattlePassItem(battlePassItemVo);
    }

    public void ClosePanel()
    {
      GameManager.Instance.PanelManager.MainMenu.ExecuteAnimation();

      gameObject.SetActive(false);
    }
  }

  [Serializable]
  public class BattlePassItemVo
  {
    [HideInInspector]
    public int ID;

    public Sprite Sprite;

    public BattlePassItemType BattlePassItemType;

    public int Amount;

    public int NecessaryStarAmount;

    [HideInInspector]
    public bool Activated;

    public bool IsPaid;
  }

  public enum BattlePassItemType
  {
    Coin,
    Health,
    Skin
  }
}