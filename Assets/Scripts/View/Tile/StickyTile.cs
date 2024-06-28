using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace View.Tile
{
  public class StickyTile : Tile
  {
    [Space(20)]
    [SerializeField]
    private StickyTile _siblingTile;

    [SerializeField]
    private SpriteRenderer _stick;

    protected override void OnMouseDown()
    {
      if (_siblingTile.TileFeatureVo.Lock || TileFeatureVo.Lock) return;

      _stick.DOFade(0, 0.2f);
      
      base.OnMouseDown();

      _siblingTile.BeforeSiblingActivation();
    }

    private async void BeforeSiblingActivation()
    {
      _stick.DOFade(0, 0.2f);

      await Task.Delay(500);
      
      OnClick();
    }

    protected override void OnValidate()
    {
      base.OnValidate();

      _siblingTile.GetComponent<SpriteRenderer>().sortingOrder = GetInspectorLayer();
      _siblingTile.SetInspectorLayer(GetInspectorLayer());
      _siblingTile._stick.sortingOrder = GetInspectorLayer();
      
      _stick.sortingOrder = GetInspectorLayer();
    }
  }
}