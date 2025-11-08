using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioData audio_data;

    private AudioSource audio_source_bgm, audio_source_sfx;
    [SerializeField] private AudioMixer bgm_mixer;
    [SerializeField] private AudioMixer sfx_mixer;

    protected void Awake()
    {
        base.Awake();

        audio_source_bgm = transform.GetChild(0).GetComponent<AudioSource>();
        audio_source_sfx = transform.GetChild(1).GetComponent<AudioSource>();
    }

    public AudioMixer GetMixerBGM()
    {
        return bgm_mixer;
    }

    public AudioMixer GetMixerSFX()
    {
        return sfx_mixer;
    }

    public AudioClip GetAudioBGM(string name)
    {
        foreach (var item in audio_data.bgm_content)
        {
            if (item.name != name) continue;

            return item.audio;
        }

        return null;
    }

    public AudioClip GetAudioSFX(string name)
    {
        foreach (var item in audio_data.sfx_content)
        {
            if (item.name != name) continue;

            return item.audio;
        }

        return null;
    }

    public void PlayAudioBGM(string name)
    {
        audio_source_bgm.clip = GetAudioBGM(name);
        audio_source_bgm.loop = true;
        audio_source_bgm.Play();
    }

    public void StopAudioBGM()
    {
        audio_source_bgm.Stop();
    }

    public void PlayAudioSFX(string name)
    {
        Debug.LogError("KONYOL");
        audio_source_sfx.clip = GetAudioSFX(name);
        audio_source_sfx.loop = false;
        //audio_source_sfx.PlayOneShot(audio_source_sfx.clip);
        audio_source_sfx.Play();
    }
}
