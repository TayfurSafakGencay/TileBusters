using UnityEngine;

namespace Tools.DebugX
{
  public static class Debugger
  {
    public static void Debug(Color color, DebugKey debugKey, string message)
    {
      string htmlColor = ColorUtility.ToHtmlStringRGB(color);
      UnityEngine.Debug.Log($"<color=#{htmlColor}>{debugKey}</color>: " + message);
    }
  }
}