using System;
using System.Collections.Generic;
using Enum;
using Tools.TileType;
using UnityEngine;
using Vo;

namespace Manager
{
  public class TileManager : MonoBehaviour
  {
    public static TileManager Instance;
    
    private List<TileSpriteVo> _tilesSpriteVos;

    private Dictionary<TileKey, Sprite> _tilesSprites = new();

    private void Awake()
    {
      Instance = this;
      
      _tilesSpriteVos = Resources.Load<TileType>("TileTypes").Instance.GetTiles();

      for (int i = 0; i < _tilesSpriteVos.Count; i++)
      {
        TileSpriteVo tileSpriteVo = _tilesSpriteVos[i];
        _tilesSprites.Add(tileSpriteVo.TileKey, tileSpriteVo.Sprite);
      }
    }

    public List<TileSpriteVo> GetTilesSprites()
    {
      return _tilesSpriteVos;
    }

    public Sprite GetSpecificTileSprite(TileKey tileKey)
    {
      return _tilesSprites[tileKey];
    }

    private Dictionary<int, TileFeatureVo> _allTiles = new();

    public void AddTile(TileFeatureVo tileFeatureVo)
    {
      _allTiles[tileFeatureVo.Id] = tileFeatureVo;
    }

    public Action<int> TileRemoved;
    
    public void RemoveTile(int id)
    {
      _allTiles.Remove(id);
      
      TileRemoved.Invoke(id);
    }

    public TileFeatureVo GetTile(int Id)
    {
      return _allTiles[Id];
    }
  }
}