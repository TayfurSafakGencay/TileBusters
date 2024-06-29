using Enum;
using UnityEngine;

namespace Manager
{
  public static class SaveManager
  {
    public static void InitialValues()
    {
      if (PlayerPrefs.HasKey(PlayerPrefKey.Level.ToString())) return;
      
      PlayerPrefs.SetInt(PlayerPrefKey.Level.ToString(), 1);
      PlayerPrefs.SetInt(PlayerPrefKey.Health.ToString(), 5);
      PlayerPrefs.SetInt(PlayerPrefKey.Star.ToString(), 0);
      PlayerPrefs.SetInt(PlayerPrefKey.Coin.ToString(), 0);
    }
    public static void SaveInt(PlayerPrefKey key, int value)
    {
      PlayerPrefs.SetInt(key.ToString(), value);
    }

    public static int GetInt(PlayerPrefKey key)
    {
      return PlayerPrefs.GetInt(key.ToString());
    }
  }
}