using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Enum;
using Manager;
using Tools.DebugX;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vo;
using Time = Enum.Time;

namespace View.Tile
{
  [RequireComponent(typeof(GraphicRaycaster))]
  public abstract class Tile : MonoBehaviour, IPointerClickHandler
  {
    public TileFeatureVo TileFeatureVo { get; private set; }

    [SerializeField]
    private int _layer;

    [SerializeField]
    private TileKey _tileKey;

    protected Image _image;

    private Canvas _canvas;

    private bool _isClickable = true;

    protected virtual void Awake()
    {
      _image = GetComponent<Image>();

      _canvas = GetComponent<Canvas>();
      _canvas.overrideSorting = true;
      _canvas.sortingOrder = _layer;
      
      _destroyImage.DOFade(0, 0);

      if (_tileKey == TileKey.Empty)
      {
        Debug.LogError("Empty Tile Key!");
      }
    }

    private void Start()
    {
      TileFeatureVo = new TileFeatureVo
      {
        Tile = this,
        Id = transform.GetSiblingIndex(),
        Key = _tileKey,
        Layer = _layer,
        Lock = false,
      };
      SetTileFeatures(TileFeatureVo);
      
      UnLock();
      
      _image.sprite = GameManager.Instance.TileManager.GetSpecificTileSprite(_tileKey);

      GameManager.Instance.TileManager.TileRemoved += TileRemoved;
    }
    
    public void SetTileFeatures(TileFeatureVo tileFeatureVo)
    {
      TileFeatureVo = tileFeatureVo;
      
      GameManager.Instance.TileManager.AddTile(tileFeatureVo);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
      if (!_isClickable) return;
      if (TileFeatureVo.Lock) return;

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

    protected virtual void UnLock()
    {
      TileFeatureVo.Lock = false;

      _image.color = Color.white;

    }

    private void Lock()
    {
      TileFeatureVo.Lock = true;

      _image.color = Color.gray;
    }

    [SerializeField]
    private Image _destroyImage;

    private const float _lastScale = 1.2f;
    public async void DestroyGameObject()
    {
      await Task.Delay((int)(Time.MoveTime * 1000));

      _destroyImage.DOFade(1, Time.DestroyTime);
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
  }
}