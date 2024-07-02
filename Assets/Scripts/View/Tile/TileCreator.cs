using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Enum;
using Manager;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Vo;
using Time = Enum.Time;

namespace View.Tile
{
  public class TileCreator : MonoBehaviour
  {
    [SerializeField]
    private int _layer;

    [SerializeField]
    private GameObject _tile;

    [SerializeField]
    private List<TileKey> _tiles;

    [SerializeField]
    private Canvas _textCanvas;

    private bool _lock;

    private readonly List<int> _upperLayerTiles = new();

    private readonly List<int> _creatorTiles = new();

    private int _currentTileId = -1;

    [Space]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private TextMeshProUGUI _countText;

    [SerializeField]
    private Transform _targetPlace;

    [SerializeField]
    private SpriteRenderer _whiteDestroySprite;

    private void Awake()
    {
      _whiteDestroySprite.DOFade(0, 0);
    }
    
    private async void Start()
    {
      gameObject.name = "Tile Creator";
      
      GameManager.Instance.TileManager.TileRemoved += TileRemoved;
      
      Vector3 position = transform.position;
      transform.position = new Vector3(position.x, position.y, 0);
      
      _countText.text = _tiles.Count.ToString();
      
      for (int i = 0; i < _tiles.Count; i++)
      {
        GameObject tile = Instantiate(_tile, transform.position, quaternion.identity, transform.parent);
        Tile tileScript = tile.GetComponent<Tile>();
        
        tileScript.SetFeatures(_layer, _tiles[i]);

        await Task.Delay(10);
        _creatorTiles.Add(tileScript.TileFeatureVo.Id);
      }
      
      InitialLockCheck();
    }
    
    private const string _tileTag = "Tile";

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject.CompareTag(_tileTag))
      {
        Tile otherTile = other.gameObject.GetComponent<Tile>();

        if (otherTile.TileFeatureVo == null) return;

        if (_creatorTiles.Contains(otherTile.TileFeatureVo.Id)) return;
        if (otherTile.TileFeatureVo.Layer > _layer)
        {
          _upperLayerTiles.Add(otherTile.TileFeatureVo.Id);
          
          Lock();
        }
      }
    }

    private async void TileRemoved(int Id)
    {
      if (_upperLayerTiles.Contains(Id))
      {
        _upperLayerTiles.Remove(Id);

        if (_upperLayerTiles.Count != 0) return;
        
        UnLock();
        
        await Task.Delay((int)(Time.MoveTime * 1000));

        ExtractNewTile();
      }

      if (Id != _currentTileId) return;
      
      await Task.Delay((int)(Time.MoveTime * 1000));

      ExtractNewTile();
    }

    private void Lock()
    {
      _lock = true;
      
      _countText.color = Color.gray;
      _spriteRenderer.color = Color.gray;
    }

    private void UnLock()
    {
      _lock = false;
      
      _countText.color = Color.white;
      _spriteRenderer.color = Color.white;
    }

    private async void InitialLockCheck()
    {
      await Task.Delay(700);

      if (_lock) return;
      
      UnLock();
      ExtractNewTile();
    }

    private void ExtractNewTile()
    {
      if (_creatorTiles.Count == 0)
      {
        DestroyAnimation();
        return;
      }
      
      int Id = _creatorTiles[0];
      TileFeatureVo tileFeatureVo = GameManager.Instance.TileManager.GetTile(Id);
      
      tileFeatureVo.Tile.MoveToTheTarget(_targetPlace.position, Ease.Linear, Time.CreatorMoveTime);
      _currentTileId = Id;
      
      _creatorTiles.Remove(Id);

      _countText.text = _creatorTiles.Count.ToString();
      
      if (_creatorTiles.Count == 0)
      {
        Destroy(_targetPlace.gameObject);
      }
    }
    

    private const float _lastScale = 1.2f;
    private void DestroyAnimation()
    {
      _whiteDestroySprite.DOFade(1, Time.DestroyTime);
      transform.DOScale(new Vector2(_lastScale, _lastScale), Time.DestroyTime).OnComplete(() =>
      {
        Destroy(gameObject);
      });
    }

    private void OnDestroy()
    {
      GameManager.Instance.TileManager.TileRemoved -= TileRemoved;
    }

    private void OnValidate()
    {
      _spriteRenderer.sortingOrder = _layer;
      _textCanvas.sortingOrder = _layer;
    }
  }
}