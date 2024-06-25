using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enum;
using Manager;
using Tools.DebugX;
using UnityEngine;
using Vo;
using Time = Enum.Time;

namespace View
{
  public class BottomCollector : MonoBehaviour
  {
    private Dictionary<Transform, TileFeatureVo> _slots = new();

    private TileManager _tileManager;

    private void Awake()
    {
      for (int i = 0; i < transform.childCount; i++)
      {
        _slots.Add(transform.GetChild(i).transform, new TileFeatureVo());
      }

      _tileManager = GameManager.Instance.TileManager;
    }

    public void FillSlot(TileFeatureVo tileFeatureVo)
    {
      _tileManager.RemoveTile(tileFeatureVo.Id);
      
      CheckSameKey(tileFeatureVo);
    }

    private async Task CheckSameKey(TileFeatureVo tileFeatureVo)
    {
      for (int i = 0; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key == tileFeatureVo.Key)
        {
          if (_slots.ElementAt(i + 1).Value.Key == TileKey.Empty)
          { 
            AddItemToEmptySlot(i + 1, tileFeatureVo);
            return;
          }
          
          if (_slots.ElementAt(i + 1).Value.Key == tileFeatureVo.Key)
          {
            Task task = MoveToRight(i + 2, tileFeatureVo);
            await task;
            CheckThreeMatching(i);
            return;
          }
          else
          {
            Task task = MoveToRight(i + 1, tileFeatureVo);
            await task;
            
            return;
          }
        }

        if (_slots.ElementAt(i).Value.Key != TileKey.Empty) continue;
        
        AddItemToFirstEmptySlot(tileFeatureVo);
        return;
      }
    }

    private void AddItemToEmptySlot(int index, TileFeatureVo tileFeatureVo)
    {
      Debugger.Debug(new Color(0.3f, 0.8f, 0.8f), DebugKey.AddItemToEmptySlot, "AddItemToEmptySlot");
      
      Transform transformKey = _slots.ElementAt(index).Key;
      AttachTheVo(transformKey, tileFeatureVo);
    }

    private void AddItemToFirstEmptySlot(TileFeatureVo tileFeatureVo)
    {
      Debugger.Debug(new Color(0.8f, 0.3f, 0.8f), DebugKey.AddItemToFirstEmptySlot, "AddItemToFirstEmptySlot");

      for (int i = 0; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key != TileKey.Empty) continue;

        Transform firstEmptySlot = _slots.ElementAt(i).Key;
        AttachTheVo(firstEmptySlot, tileFeatureVo);
        return;
      }
    }

    private async Task MoveToRight(int index, TileFeatureVo tileFeatureVo)
    {
      Debugger.Debug(new Color(0.8f, 0.8f, 0.3f), DebugKey.MoveToRight, index.ToString());

      for (int i = _slots.Count - 1; i >= 0; i--)
      {
        if (i < index) break;

        KeyValuePair<Transform, TileFeatureVo> tile = _slots.ElementAt(i);

        if (tile.Value.Key == TileKey.Empty) continue;
        if (tile.Value.Key == tileFeatureVo.Key) continue;

        Transform key = _slots.ElementAt(i + 1).Key;

        AttachTheVo(key, tile.Value);
      }

      Transform targetKey = _slots.ElementAt(index).Key;
      AttachTheVo(targetKey, tileFeatureVo);
      
      Debugger.Debug(new Color(0.8f, 0.8f, 0.3f), DebugKey.MoveToRight, "End");
    }

    private async void CheckThreeMatching(int index)
    {
      Debugger.Debug(new Color(0.3f, 0.8f, 0.4f), DebugKey.CheckThreeMatching, index.ToString());

      for (int i = index; i < index + 3; i++)
      {
        KeyValuePair<Transform, TileFeatureVo> item = _slots.ElementAt(i);
        
        // TODO: Score
        item.Value.Tile.DestroyGameObject();
        _slots[item.Key] = new TileFeatureVo();
      }
      
      await Task.Delay((int)(Time.MoveTime * 1000));

      MoveToLeft(index, 3);
    }

    private void MoveToLeft(int index, int moveAmount)
    {
      for (int i = index + moveAmount; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key == TileKey.Empty) continue;
        
        KeyValuePair<Transform, TileFeatureVo> newSlot = _slots.ElementAt(i - moveAmount);
        KeyValuePair<Transform, TileFeatureVo> oldSlot = _slots.ElementAt(i);
      
        AttachTheVo(newSlot.Key, oldSlot.Value);
      
        _slots[oldSlot.Key] = new TileFeatureVo();
      }
    }

    private void AttachTheVo(Transform key, TileFeatureVo tileFeatureVo)
    {
      _slots[key] = tileFeatureVo;
      _slots[key].Tile.MoveToTheTarget(key.position);
    }
  }
}