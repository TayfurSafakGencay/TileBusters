using System;
using Enum;
using UnityEngine;
using View;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Managers")]
        public TileManager TileManager;

        public PanelManager PanelManager;

        public SoundManager SoundManager;

        public ParticleManager ParticleManager;

        [Header("Collector")]
        public BottomCollector BottomCollector;

        public static int Level;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            SaveManager.InitialValues();
            
            SaveManager.SaveInt(PlayerPrefKey.Star, 215);
            SaveManager.SaveInt(PlayerPrefKey.BattlePass, 0);
        }

        public static Action GameStarted;

        public static void StartGame()
        {
            Level = SaveManager.GetInt(PlayerPrefKey.Level);

            GameStarted?.Invoke();
        }

        public static Action<bool> GameFinished;

        public void GameLost()
        {
            SoundManager.PlaySound(SoundKey.Fail);
            
            Debug.Log("Game Lost");
            
            GameFinished.Invoke(false);
        }

        public void GameWin()
        {
            SoundManager.PlaySound(SoundKey.Win);

            Debug.Log("Game Win");
            
            GameFinished.Invoke(true);

            SaveManager.SaveInt(PlayerPrefKey.Level, Level + 1);
        }
    }
}
