using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.Tile
{
  public class ChainedTile : Tile
  {
    private bool _chanied = true;

    [SerializeField]
    private bool _isGoldChain;

    [SerializeField]
    private Image _lockImage;

    protected override void Awake()
    {
      base.Awake();
      
      _image.color = Color.gray;
      _lockImage.gameObject.SetActive(true);

      if (_adjacentTiles.Count == 0 )
      {
        OpenChain();
      }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
      if (_chanied) return;
      
      base.OnPointerClick(eventData);
    }

    protected override void TileRemoved(int Id)
    {
      if (TileFeatureVo.Lock)
      {
        base.TileRemoved(Id);
        return;
      }
      
      if (_adjacentTiles.Contains(Id))
      {
        if (!_isGoldChain)
        {
          OpenChain();
        }
      }
      
      base.TileRemoved(Id);

      if (_adjacentTiles.Count == 0)
      {
        OpenChain();
      }
    }

    protected override void UnLock()
    {
      TileFeatureVo.Lock = false;

      if (!TileFeatureVo.Lock && !_chanied)
      {
        base.UnLock();
      }
    }

    private void OpenChain()
    {
      _lockImage.gameObject.SetActive(false);
      _image.color = Color.white;
      _chanied = false;
    }
  }
}