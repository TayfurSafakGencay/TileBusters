using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.Tile
{
  public class IceTile : Tile
  {
    private int _health = 2;

    [SerializeField]
    private List<Sprite> _iceSpriteList;

    [SerializeField]
    private Image _iceImage;

    protected override void Awake()
    {
      base.Awake();
      
      _iceImage.sprite = _iceSpriteList[_health];
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
      if (_health >= 0) return;
      
      base.OnPointerClick(eventData);
    }

    protected override void TileRemoved(int Id)
    {
      if (TileFeatureVo.Lock)
      {
        base.TileRemoved(Id);
        return;
      }
      
      base.TileRemoved(Id);

      if (_health <= 0)
      {
        _health--;
        _iceImage.gameObject.SetActive(false);
        return;
      }

      _health--;
      _iceImage.sprite = _iceSpriteList[_health];
    }
  }
}