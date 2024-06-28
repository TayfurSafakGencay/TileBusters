using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Enum;
using Manager;
using Tools.DebugX;
using UnityEngine;
using UnityEngine.Rendering;
using Vo;
using Time = Enum.Time;

namespace View.Tile
{
  [RequireComponent(typeof(SpriteRenderer))]
  public abstract class Tile : MonoBehaviour
  {
    public TileFeatureVo TileFeatureVo { get; private set; }

    [SerializeField]
    private TileKey _tileKey;

    protected SpriteRenderer _spriteRenderer;

    private bool _isClickable = true;

    [SerializeField]
    private int _layer;

    public GameObject Glow;

    protected virtual void Awake()
    {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _destroySprite.DOFade(0, 0);

      if (_tileKey == TileKey.Empty)
      {
        Debug.LogError("Empty Tile Key!");
      }
    }

    protected virtual void Start()
    {
      _spriteRenderer.sortingOrder = _layer;
      Glow.GetComponent<SortingGroup>().sortingOrder = _layer;
      
      Vector3 position = transform.position;
      transform.position = new Vector3(position.x, position.y, 25 - _layer);

      TileFeatureVo = new TileFeatureVo
      {
        Tile = this,
        Id = 0,
        Key = _tileKey,
        Layer = _layer,
        Lock = false,
      };
      SetTileFeatures(TileFeatureVo);
      
      UnLock(true);
      
      _spriteRenderer.sprite = GameManager.Instance.TileManager.GetSpecificTileSprite(_tileKey);

      GameManager.Instance.TileManager.TileRemoved += TileRemoved;
    }
    
    public void SetTileFeatures(TileFeatureVo tileFeatureVo)
    {
      TileFeatureVo.Id = GameManager.Instance.TileManager.AddTile(tileFeatureVo);
    }

    public void SetFeatures(int layer, TileKey key)
    {
      _layer = layer;
      _tileKey = key;
    }

    // public virtual void OnPointerClick(PointerEventData eventData)
    // {
    //   OnClick();
    // }
    
    protected virtual void OnMouseDown()
    {
      OnClick();
    }

    protected virtual void OnClick()
    {
      if (!_isClickable) return;
      if (TileFeatureVo.Lock) return;

      _spriteRenderer.sortingLayerName = "CollectedTile";
      Glow.GetComponent<SortingGroup>().sortingLayerName = "CollectedTile";
      
      _isClickable = false;
      
      Debugger.Debug(new Color(0.5f, 0.3f, 0.8f), DebugKey.Click, TileFeatureVo.Id.ToString());

      Destroy(gameObject.GetComponent<BoxCollider2D>());
      Destroy(gameObject.GetComponent<Rigidbody2D>());
      
      GameManager.Instance.BottomCollector.FillSlot(TileFeatureVo);
    }
    
    public void MoveToTheTarget(Vector2 position, Ease ease = Ease.InBack, float time = Time.MoveTime)
    {
      transform.DOMove(position, time).SetEase(ease);
    }

    private const string _tileTag = "Tile";
    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject.CompareTag(_tileTag))
      {
        Tile otherTile = other.gameObject.GetComponent<Tile>();

        if (otherTile.TileFeatureVo == null || TileFeatureVo == null) return;

        if (otherTile.TileFeatureVo.Layer > TileFeatureVo.Layer)
        {
          _upperLayerTiles.Add(otherTile.TileFeatureVo.Id);
          
          Lock();
        }

        if (otherTile.TileFeatureVo.Layer == TileFeatureVo.Layer)
        {
          _adjacentTiles.Add(otherTile.TileFeatureVo.Id);
        }
      }
    }

    protected virtual void TileRemoved(int Id)
    {
      if (_adjacentTiles.Contains(Id))
      {
        _adjacentTiles.Remove(Id);
      }

      if (_upperLayerTiles.Contains(Id))
      {
        _upperLayerTiles.Remove(Id);
      }
      
      if (_upperLayerTiles.Count == 0)
      {
        UnLock();
      }
    }

    protected virtual void UnLock(bool initial = false)
    {
      TileFeatureVo.Lock = false;

      if (initial)
        _spriteRenderer.color = Color.white;
      else
        _spriteRenderer.DOColor(new Color(1, 1, 1, 1), Time.MoveTime);

      Glow.SetActive(true);
    }

    private void Lock()
    {
      TileFeatureVo.Lock = true;

      _spriteRenderer.color = Color.gray;
      Glow.SetActive(false);
    }

    [SerializeField]
    private SpriteRenderer _destroySprite;

    private const float _lastScale = 1.2f;
    public async void DestroyGameObject()
    {
      await Task.Delay((int)(Time.MoveTime * 1000));

      _destroySprite.DOFade(1, Time.DestroyTime);
      transform.DOScale(new Vector2(_lastScale, _lastScale), Time.DestroyTime).OnComplete(() =>
      {
        Destroy(gameObject);
      });
    }

    private void OnDestroy()
    {
      GameManager.Instance.TileManager.TileRemoved -= TileRemoved;
    }

    protected readonly List<int> _adjacentTiles = new();

    protected readonly List<int> _upperLayerTiles = new();

#if UNITY_EDITOR
    protected int GetInspectorLayer()
    {
      return _layer;
    }

    protected void SetInspectorLayer(int layer)
    {
      _layer = layer;
    }

    protected virtual void OnValidate()
    {
      GetComponent<SpriteRenderer>().sortingOrder = _layer;
      Glow.GetComponent<SortingGroup>().sortingOrder = _layer;
    }
#endif

  }
}