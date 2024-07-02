using System.Collections.Generic;
using UnityEngine;

namespace View.Tile
{
  public class IceTile : Tile
  {
    private int _health = 2;

    [SerializeField]
    private List<Sprite> _iceSpriteList;

    [SerializeField]
    private SpriteRenderer _iceImage;

    protected override void Awake()
    {
      base.Awake();
      
      _iceImage.sprite = _iceSpriteList[_health];
    }

    protected override void Start()
    {
      base.Start();
      
      gameObject.name = TileFeatureVo.Key + " IceTile";

      transform.localPosition -= new Vector3(0, 0, -0.5f);
      
      Glow.SetActive(false);
    }

    protected override void OnMouseDown()
    {
      if (_health >= 0) return;
      
      base.OnMouseDown();
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
        Glow.SetActive(true);
        return;
      }

      _health--;
      _iceImage.sprite = _iceSpriteList[_health];
      Glow.SetActive(false);
    }

    protected override void OnValidate()
    {
      base.OnValidate();

      _iceImage.sortingOrder = GetInspectorLayer();
    }
  }
}