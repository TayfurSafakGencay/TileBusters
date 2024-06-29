using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
  public class SoundManager : MonoBehaviour
  {
    [SerializeField]
    private List<SoundVo> _sounds = new();

    private readonly Dictionary<SoundKey, AudioClip> _soundDictionary = new();
    
    private AudioSource _audioSource;

    private void Awake()
    {
      _audioSource = GetComponent<AudioSource>();

      foreach (SoundVo sound in _sounds.Where(sound => !_soundDictionary.ContainsKey(sound.key)))
      {
        _soundDictionary.Add(sound.key, sound.clip);
      }
    }

    public void PlaySound(SoundKey key)
    {
      if (_soundDictionary.TryGetValue(key, out AudioClip value))
      {
        _audioSource.PlayOneShot(value);
      }
      else
      {
        Debug.LogWarning("Sound with key: " + key + " not found!");
      }
    }
  }
  
  [System.Serializable]
  public class SoundVo
  {
    public SoundKey key;
      
    public AudioClip clip;
  }

  [System.Serializable]
  public enum SoundKey
  {
    Click,
    Match,
    Fail,
    Win
  }
}