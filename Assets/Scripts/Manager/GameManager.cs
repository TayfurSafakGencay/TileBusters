using UnityEngine;
using View;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public TileManager TileManager;

        public BottomCollector BottomCollector;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            
        }
    }
}
