using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Time = Enum.Time;

namespace View.Tile
{
  public class ChainedTile : Tile
  {
    private bool _chained = true;

    [Space(30)]
    [SerializeField]
    private bool _isGoldChain;

    [SerializeField]
    private SpriteRenderer _lockSpriteRenderer;

    [SerializeField]
    private Sprite _unlockImage;

    protected override void Awake()
    {
      base.Awake();
      
      _spriteRenderer.color = Color.gray;
      _lockSpriteRenderer.gameObject.SetActive(true);
      
      Glow.SetActive(false);
    }

    protected override void Start()
    {
      base.Start();
      
      WaitToOpenChain();
    }

    protected override void OnMouseDown()
    {
      if (_chained) return;
      
      base.OnMouseDown();
    }

    protected override void TileRemoved(int Id)
    {
      if (_adjacentTiles.Contains(Id))
      {
        if (!_isGoldChain && !TileFeatureVo.Lock)
        {
          OpenChain();
        }
      }
      
      base.TileRemoved(Id);

      if (_adjacentTiles.Count == 0 && !TileFeatureVo.Lock)
      {
        OpenChain();
      }
    }

    protected override void UnLock(bool initial = false)
    {
      TileFeatureVo.Lock = false;

      if (!TileFeatureVo.Lock && !_chained)
      {
        base.UnLock();
      }
    }

    private async void WaitToOpenChain()
    {
      await Task.Delay(100);
      
      if (_adjacentTiles.Count == 0 )
      {
        OpenChain();
      }
    }

    private void OpenChain()
    {
      _lockSpriteRenderer.sprite = _unlockImage;
      _spriteRenderer.color = Color.white;
      _chained = false;

      _lockSpriteRenderer.DOFade(0, Time.MoveTime);
      
      UnLock();
    }

    protected override void OnValidate()
    {
      base.OnValidate();

      _lockSpriteRenderer.sortingOrder = GetInspectorLayer();
    }
  }
}