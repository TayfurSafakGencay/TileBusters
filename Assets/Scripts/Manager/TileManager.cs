using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enum;
using Tools.DebugX;
using Tools.TileType;
using UnityEngine;
using Vo;
using Debug = UnityEngine.Debug;
using Debugger = Tools.DebugX.Debugger;

namespace Manager
{
  public class TileManager : MonoBehaviour
  {
    public static TileManager Instance;
    
    private List<TileSpriteVo> _tilesSpriteVos;

    private readonly Dictionary<TileKey, Sprite> _tilesSprites = new();

    private static int _lastId;

    private void Awake()
    {
      Instance = this;
      
      UpdateSkin(SaveManager.GetInt(PlayerPrefKey.Skin));
    }

    public void UpdateSkin(int skinIndex)
    {
      SkinKey skinKey = (SkinKey)skinIndex;
      
      _tilesSpriteVos = Resources.Load<TileType>(skinKey.ToString()).Instance.GetTiles();
      
      _tilesSprites.Clear();
      
      for (int i = 0; i < _tilesSpriteVos.Count; i++)
      {
        TileSpriteVo tileSpriteVo = _tilesSpriteVos[i];
        _tilesSprites.Add(tileSpriteVo.TileKey, tileSpriteVo.Sprite);
      }
    }

    private void Start()
    {
      GameManager.GameStarted += CheckTiles;
      GameManager.GameFinished += GameFinished;
    }

    private async void CheckTiles()
    {
      _lastId = 0;
      
      await Task.Delay(250);
      
      Stopwatch stopwatch = new();
      stopwatch.Start();
      
      TileKey[] tileKeys = (TileKey[]) System.Enum.GetValues(typeof(TileKey));
      for (int tileKeyIndex = 0; tileKeyIndex < tileKeys.Length; tileKeyIndex++)
      {
        int count = 0;
        
        for (int tileIndex = 0; tileIndex < _allTiles.Count; tileIndex++)
        {
          if (_allTiles.ElementAt(tileIndex).Value.Key == TileKey.Empty)
          {
            Debug.LogError("There is an Empty Tile in the scene!");
          }

          if (tileKeys[tileKeyIndex] == _allTiles.ElementAt(tileIndex).Value.Key)
          {
            count++;
          }
        }

        if (count == 0)
        {
          string message = $"It is not on the scene: ({tileKeys[tileKeyIndex]})";
          Debugger.Log(Color.yellow, DebugKey.TileCheck, message);
        }
        else if (count % 3 != 0)
        {
          string message = $"Not Enough Tiles! ({tileKeys[tileKeyIndex]})";
          Debugger.Log(Color.red, DebugKey.TileCheck, message);
        }
        else
        {
          string message = $"There is no issue: ({tileKeys[tileKeyIndex]})";
          Debugger.Log(Color.green, DebugKey.TileCheck, message);
        }
      }
      
      stopwatch.Stop();
      Debugger.Log(Color.white, DebugKey.Timer, $"Time taken: {stopwatch.ElapsedMilliseconds}ms");
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

    public int AddTile(TileFeatureVo tileFeatureVo)
    {
      while (true)
      {
        if (_allTiles.ContainsKey(_lastId))
        {
          _lastId++;
        }
        else
        {
          tileFeatureVo.Id = _lastId;
          break;
        }
      }

      _allTiles[tileFeatureVo.Id] = tileFeatureVo;
      return tileFeatureVo.Id;
    }

    public Action<int> TileRemoved;
    
    public void RemoveTile(int id)
    {
      _allTiles.Remove(id);
      
      TileRemoved.Invoke(id);
    }

    private void GameFinished(bool success)
    {
      _allTiles.Clear();
    }

    public Dictionary<int, TileFeatureVo> GetAllTiles()
    {
      return _allTiles;
    }

    public int GetAllTilesCount()
    {
      return _allTiles.Count;
    }

    public TileFeatureVo GetTile(int Id)
    {
      return _allTiles[Id];
    }
  }
}