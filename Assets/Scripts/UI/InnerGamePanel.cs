using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
  public class InnerGamePanel : MonoBehaviour
  {
    [SerializeField]
    private TextMeshProUGUI _levelText;

    private void OnEnable()
    {
      _levelText.text = "Level " + GameManager.Level;
    }
  }
}