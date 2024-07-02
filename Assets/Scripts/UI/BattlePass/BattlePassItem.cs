using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattlePass
{
  public class BattlePassItem : MonoBehaviour
  {
    private BattlePassItemVo _battlePassItemVo;

    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Image _portrait;

    [SerializeField]
    private TextMeshProUGUI _amountText;

    [SerializeField]
    private Image _onPortraitImage;

    [SerializeField]
    private Sprite _lockSprite;
    
    [SerializeField]
    private Sprite _unlockSprite;

    [SerializeField]
    private Sprite _activatedSprite;

    private BattlePass _battlePass;

    private Button _activateButton;

    private void Awake()
    {
      BattlePass.Reached += Reached;
      BattlePass.IsItemActive += IsItemActive;
      BattlePass.BattlePassPanelOpened += BattlePassOpened;
    }

    private void Start()
    {
      _onPortraitImage.gameObject.SetActive(false);

      _activateButton = _portrait.GetComponent<Button>();
      _activateButton.interactable = false;
    }

    public void SetData(BattlePassItemVo battlePassItemVo, BattlePass battlePass)
    {
      _battlePassItemVo = battlePassItemVo;
      _battlePass = battlePass;
      
      UpdateItem();
    }

    public void InitialAnimation()
    {
      ZeroToOneScale().OnComplete(Locked);
    }

    private void OnEnable()
    {
      _portrait.transform.localScale = new Vector3(0, 0, 0);
    }

    public void IsItemActive(int id)
    {
      if (_battlePassItemVo.ID != id) return;
      
      OnActivated();
    }

    public void OnActivated()
    {
      if (!_battlePassItemVo.Activated)
      {
        _battlePass.ActivatedItem(_battlePassItemVo.ID, _battlePassItemVo);
      }
      
      _activateButton.interactable = false;
      _battlePassItemVo.Activated = true;
      
      _portrait.DOColor(Color.gray, 0.5f).OnComplete(() =>
      {
        _onPortraitImage.color = new Color(1, 1, 1, 0);
        _onPortraitImage.gameObject.SetActive(true);
        _onPortraitImage.sprite = _activatedSprite;
        _onPortraitImage.DOFade(1, 0.5f);
      });
      
      DoSlider().OnComplete(GrowAndShrinkAnimation);
    }

    private void UpdateItem()
    {
      _portrait.sprite = _battlePassItemVo.Sprite;
      _amountText.text = _battlePassItemVo.Amount.ToString();
    }

    public void Reached(int id)
    {
      if (id != _battlePassItemVo.ID) return;
      if (_battlePassItemVo.Activated) return;

      _activateButton.interactable = true;
      
      DoSlider().OnComplete(() =>
      {
        GrowAndShrinkAnimation();
        Unlocked();
      });
    }

    public void Locked()
    {
      _activateButton.interactable = false;

      _portrait.DOColor(Color.gray, 0.5f).OnComplete(() =>
      {
        _onPortraitImage.color = new Color(1, 1, 1, 0);
        _onPortraitImage.gameObject.SetActive(true);
        _onPortraitImage.sprite = _lockSprite;
        _onPortraitImage.DOFade(1, 0.5f);
      });
    }

    public void Unlocked()
    {
      _onPortraitImage.DOFade(0, 0.5f).OnComplete(() =>
      {
        _onPortraitImage.gameObject.SetActive(false);

        _portrait.DOColor(Color.white, 0.5f);
      });
    }

    private const float _endScale = 1.25f;

    private const float _initialAnimationTime = 0.5f;
    
    private void GrowAndShrinkAnimation()
    {
      _portrait.transform.DOScale(_endScale, _initialAnimationTime)
        .OnComplete(() => 
        {
          _portrait.transform.DOScale(1, _initialAnimationTime);
        });
    }

    private TweenerCore<Vector3, Vector3, VectorOptions> ZeroToOneScale()
    {
      return _portrait.transform.DOScale(1, _initialAnimationTime * 2);
    }
    
    public TweenerCore<float, float, FloatOptions> DoSlider()
    {
      return _slider.DOValue(1, 0.6f).SetEase(Ease.Linear);
    }

    private void BattlePassOpened()
    {
      Destroy(gameObject);
    }

    private void OnDestroy()
    {
      BattlePass.Reached -= Reached;
      BattlePass.IsItemActive -= IsItemActive;
      BattlePass.BattlePassPanelOpened -= BattlePassOpened;
    }
  }
}