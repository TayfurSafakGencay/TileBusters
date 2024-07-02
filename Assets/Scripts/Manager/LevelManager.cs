using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Manager
{
  public class LevelManager : MonoBehaviour
  {
    public static LevelManager Instance;

    public Transform PlayableArea;

    private GameObject levelObject;

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      GameManager.GameFinished += GameFinished;
    }

    private void GameFinished(bool success)
    {
      if (levelObject != null) Destroy(levelObject);
    }

    public async void CallLevel(int level)
    {
      string key = "Level " + level;
      AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.InstantiateAsync(key, PlayableArea);
      
      await asyncOperationHandle.Task;

      if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
      {
        levelObject = asyncOperationHandle.Result.gameObject;
      }
      else
      {
        Debug.LogError("Level could not load!");
      }
    }
  }
}