using System.Collections.Generic;
using UnityEngine;
using Vo;

namespace Tools.TileType
{
  [CreateAssetMenu(fileName = "TileTypes", menuName = "Create Tile Data", order = 0)]
  public class TileType : ScriptableObject
  {
    public TileType Instance;
    
    [SerializeField]
    private List<TileSpriteVo> _tiles;

    private void OnEnable()
    {
      Instance = this;
    }

    public List<TileSpriteVo> GetTiles()
    {
      return _tiles;
    }
  }
}