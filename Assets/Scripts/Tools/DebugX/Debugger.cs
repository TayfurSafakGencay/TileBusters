using UnityEngine;

namespace Tools.DebugX
{
  public static class Debugger
  {
    public static void Log(Color color, DebugKey debugKey, string message)
    {
      string htmlColor = ColorUtility.ToHtmlStringRGB(color);
      Debug.Log($"<color=#{htmlColor}>{debugKey}</color>: " + message);
    }
    
    public static void LogWarning(Color color, DebugKey debugKey, string message)
    {
      string htmlColor = ColorUtility.ToHtmlStringRGB(color);
      Debug.LogWarning($"<color=#{htmlColor}>{debugKey}</color>: " + message);
    }
    
    public static void LogError(Color color, DebugKey debugKey, string message)
    {
      string htmlColor = ColorUtility.ToHtmlStringRGB(color);
      Debug.LogError($"<color=#{htmlColor}>{debugKey}</color>: " + message);
    }
  }
}