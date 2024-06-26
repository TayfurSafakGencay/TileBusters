using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Enum;
using Manager;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
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
    private Transform _tileArea;

    private Canvas _canvas;

    private bool _lock;

    private readonly List<int> _upperLayerTiles = new();

    private readonly List<int> _creatorTiles = new();

    private int _currentTileId = -1;

    [Space]
    [SerializeField]
    private Image _image;

    [SerializeField]
    private TextMeshProUGUI _countText;

    [SerializeField]
    private Transform _targetPlace;

    [SerializeField]
    private Image _whiteDestroyImage;

    private void Awake()
    {
      _canvas = GetComponent<Canvas>();
      _canvas.overrideSorting = true;
      _canvas.sortingOrder = _layer;
      
      _whiteDestroyImage.DOFade(0, 0);
    }
    
    private void Start()
    {
      GameManager.Instance.TileManager.TileRemoved += TileRemoved;
      
      for (int i = 0; i < _tiles.Count; i++)
      {
        GameObject tile = Instantiate(_tile, transform.position, quaternion.identity, _tileArea);
        Tile tileScript = tile.GetComponent<Tile>();

        TileFeatureVo tileFeatureVo = new()
        {
          Tile = tileScript,
          Id = tile.transform.GetSiblingIndex(),
          Layer = _layer,
          Lock = false,
          Key = _tiles[i],
        };
        
        tileScript.SetTileFeatures(tileFeatureVo);
        
        _creatorTiles.Add(tileFeatureVo.Id);
      }
      
      _countText.text = _creatorTiles.Count.ToString();
      
      InitialLockCheck();
    }
    
    private const string _tileTag = "Tile";

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject.CompareTag(_tileTag))
      {
        Tile otherTile = other.gameObject.GetComponent<Tile>();

        if (!_creatorTiles.Contains(otherTile.TileFeatureVo.Id))
        {
          if (otherTile.TileFeatureVo.Layer > _layer)
          {
            _upperLayerTiles.Add(otherTile.TileFeatureVo.Id);
          
            Lock();
          }
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
      _image.color = Color.gray;
    }

    private void UnLock()
    {
      _lock = false;
      
      _countText.color = Color.white;
      _image.color = Color.white;
    }

    private async void InitialLockCheck()
    {
      await Task.Delay(100);

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
      // await Task.Delay((int)(Time.MoveTime * 1000));

      _whiteDestroyImage.DOFade(1, Time.DestroyTime);
      transform.DOScale(new Vector2(_lastScale, _lastScale), Time.DestroyTime).OnComplete(() =>
      {
        Destroy(gameObject);
      });
    }
  }
}