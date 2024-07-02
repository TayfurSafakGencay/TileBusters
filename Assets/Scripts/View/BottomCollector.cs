using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
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
    }

    private void Start()
    {
      _tileManager = GameManager.Instance.TileManager;

      GameManager.GameStarted += GameStarted;
    }

    private void GameStarted()
    {
      _isGameEnded = false;

      for (int i = 0; i < _slots.Count; i++)
      {
        Transform key = _slots.ElementAt(i).Key;
        _slots[key] = new TileFeatureVo();
      }
    }

    public async void FillSlot(TileFeatureVo tileFeatureVo)
    {
      _tileManager.RemoveTile(tileFeatureVo.Id);
      
      await CheckSameKey(tileFeatureVo);
      
      CheckWiningCondition();
    }

    private bool _isGameEnded;
    private async void CheckWiningCondition()
    {
      if (_isGameEnded) return;
      
      int count = 0;
      for (int i = 0; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key == TileKey.Empty)
        {
          count++;
        }
      }

      if (count == _slots.Count)
      {
        if (GameManager.Instance.TileManager.GetAllTilesCount() == 0)
        {
          _isGameEnded = true;

          await Task.Delay((int)(Time.MoveTime * 1500));

          GameManager.Instance.GameWin();
        }
      }
      else if (count == 0)
      {
        _isGameEnded = true;

        await Task.Delay((int)(Time.MoveTime * 1500));

        GameManager.Instance.GameLost();
      }
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
      Debugger.Log(new Color(0.3f, 0.8f, 0.8f), DebugKey.AddItemToEmptySlot, "AddItemToEmptySlot");
      
      Transform transformKey = _slots.ElementAt(index).Key;
      AttachTheVo(transformKey, tileFeatureVo);
    }

    private void AddItemToFirstEmptySlot(TileFeatureVo tileFeatureVo)
    {
      Debugger.Log(new Color(0.8f, 0.3f, 0.8f), DebugKey.AddItemToFirstEmptySlot, "AddItemToFirstEmptySlot");

      for (int i = 0; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key != TileKey.Empty) continue;

        Transform firstEmptySlot = _slots.ElementAt(i).Key;
        AttachTheVo(firstEmptySlot, tileFeatureVo);
        return;
      }
    }

    private Task MoveToRight(int index, TileFeatureVo tileFeatureVo)
    {
      Debugger.Log(new Color(0.8f, 0.8f, 0.3f), DebugKey.MoveToRight, index.ToString());

      for (int i = _slots.Count - 1; i >= 0; i--)
      {
        if (i < index) break;

        KeyValuePair<Transform, TileFeatureVo> tile = _slots.ElementAt(i);

        if (tile.Value.Key == TileKey.Empty) continue;
        if (tile.Value.Key == tileFeatureVo.Key) continue;

        Transform key = _slots.ElementAt(i + 1).Key;

        AttachTheVo(key, tile.Value, _horizontalEase);
      }

      Transform targetKey = _slots.ElementAt(index).Key;
      AttachTheVo(targetKey, tileFeatureVo, _horizontalEase);
      
      Debugger.Log(new Color(0.8f, 0.8f, 0.3f), DebugKey.MoveToRight, "End");

      return Task.Delay(0);
    }

    private async void CheckThreeMatching(int index)
    {
      Debugger.Log(new Color(0.3f, 0.8f, 0.4f), DebugKey.CheckThreeMatching, index.ToString());

      List<Vector3> _destroyedObjectTransforms = new();

      for (int i = index; i < index + 3; i++)
      {
        KeyValuePair<Transform, TileFeatureVo> item = _slots.ElementAt(i);
        
        item.Value.Tile.DestroyGameObject();
        _slots[item.Key] = new TileFeatureVo();
        
        _destroyedObjectTransforms.Add(item.Key.position);
      }
      
      await Task.Delay((int)(Time.MoveTime * 1000));
      
      GameManager.Instance.SoundManager.PlaySound(SoundKey.Match);
      
      for (int i = 0; i < _destroyedObjectTransforms.Count; i++)
      {
        GameManager.Instance.ParticleManager.PlayParticleEffectFromPool(_destroyedObjectTransforms[i], VFX.Match);
      }

      MoveToLeft(index, 3);
    }

    private void MoveToLeft(int index, int moveAmount)
    {
      for (int i = index + moveAmount; i < _slots.Count; i++)
      {
        if (_slots.ElementAt(i).Value.Key == TileKey.Empty) continue;
        
        KeyValuePair<Transform, TileFeatureVo> newSlot = _slots.ElementAt(i - moveAmount);
        KeyValuePair<Transform, TileFeatureVo> oldSlot = _slots.ElementAt(i);
      
        AttachTheVo(newSlot.Key, oldSlot.Value, _horizontalEase);
      
        _slots[oldSlot.Key] = new TileFeatureVo();
      }
    }

    private const Ease _horizontalEase = Ease.InOutQuart;
    private void AttachTheVo(Transform key, TileFeatureVo tileFeatureVo, Ease ease = Ease.InBack)
    {
      _slots[key] = tileFeatureVo;
      _slots[key].Tile.MoveToTheTarget(key.position, ease);
    }
  }
}