using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.Tile
{
  public class StickyTile : Tile
  {
    [Space(20)]
    [SerializeField]
    private StickyTile _siblingTile;

    [SerializeField]
    private Image _stick;

    public override void OnPointerClick(PointerEventData eventData)
    {
      if (_siblingTile.TileFeatureVo.Lock) return;
      if (TileFeatureVo.Lock) return;

      _stick.DOFade(0, 0.2f);
      
      base.OnPointerClick(eventData);

      _siblingTile.OnClick();
    }

    protected override void OnClick()
    {
      _stick.DOFade(0, 0.2f);

      base.OnClick();
    }
  }
}