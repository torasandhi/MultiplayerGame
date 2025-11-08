using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ContentAudio
{
    public string name;
    public AudioClip audio;
}

[CreateAssetMenu(fileName = "AudioData", menuName = "Data/AudioData", order = 0)]
public class AudioData : ScriptableObject
{
    public List<ContentAudio> bgm_content;
    public List<ContentAudio> sfx_content;
}
