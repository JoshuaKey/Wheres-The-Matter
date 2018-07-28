using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    [Header("Music")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip defaultMusic;
    [SerializeField] float defaultVolume;

    [Header("Mixer")]
    [SerializeField] AudioMixerGroup musicGroup;
    [SerializeField] AudioMixerGroup soundGroup;

    public static AudioManager Instance;

    private List<AudioSource> soundSources = new List<AudioSource>();

    private void Awake() {
        Instance = this;

        musicSource.outputAudioMixerGroup = musicGroup;

        // Initial Sounds
        var soundSource = this.gameObject.AddComponent<AudioSource>();
        soundSource.outputAudioMixerGroup = soundGroup;
        soundSources.Add(soundSource);

        soundSource = this.gameObject.AddComponent<AudioSource>();
        soundSource.outputAudioMixerGroup = soundGroup;
        soundSources.Add(soundSource);
    }

    // Use this for initialization
    void Start () {
        if (defaultMusic != null) {
            PlayMusic(defaultMusic, defaultVolume);
        }
    }
	
    public void PlayMusic(AudioClip clip, float volume = 1.0f) {
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }
    public AudioClip GetMusic() { return musicSource.clip; }
    public bool IsMusicPlaying() { return musicSource.isPlaying; }

    public void StopMusic(bool playDefault = false) {
        musicSource.Stop();
        if (playDefault && defaultMusic != null) {
            PlayMusic(defaultMusic, defaultVolume);
        }
    }

    public IEnumerator FadeMusic(AudioClip clip, float volume = 1.0f, float fadeTime = 1.0f) {
        fadeTime /= 2.0f;

        StartCoroutine(FadeMusicOut(fadeTime));

        musicSource.clip = clip;

        // Fade In
        float currTime = Time.time;
        while (Time.time < currTime + fadeTime) {
            float t = (Time.time - currTime) / fadeTime;

            musicSource.volume = volume * t;

            yield return null;
        }
        musicSource.volume = volume;

    }
    public IEnumerator FadeMusicOut(float fadeTime = 1.0f) {
        float beginVolume = musicSource.volume;

        // Fade out
        float currTime = Time.time;
        while (Time.time < currTime + fadeTime) {
            float t = (Time.time - currTime) / fadeTime;

            musicSource.volume = beginVolume * t;

            yield return null;
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1.0f, int index = -1) {
        if (index == -1) { // Find Next Sound
            AudioSource soundSource = null;
            for (int i = 0; i < soundSources.Count; i++) {
                if (!soundSources[i].isPlaying) {
                    soundSource = soundSources[i];
                    break;
                }
            }
            if (soundSource == null) {
                soundSource = this.gameObject.AddComponent<AudioSource>();
                soundSource.outputAudioMixerGroup = soundGroup;
                soundSources.Add(soundSource);
            }

            soundSource.clip = clip;
            soundSource.volume = volume;
            soundSource.Play();
        } else if(soundSources.Count > index) { // Use Existing Sound
            var soundSource = soundSources[index];
            soundSource.clip = clip;
            soundSource.volume = volume;
            soundSource.Play();
        }
    }
    public void PlayUISound(AudioClip clip) {
        PlaySound(clip, .8f);
    }
}
