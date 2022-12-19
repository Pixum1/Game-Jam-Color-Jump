using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField, Range(0, 1)] private float startVolume;
    [SerializeField, Range(0, 1)] private float volume;
    [SerializeField] private float fadeDuration;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != null && Instance != this)
            Destroy(this);

    }

    private void Start()
    {
        StartCoroutine(StartMusic());
    }

    private IEnumerator StartMusic()
    {
        musicSource.volume = startVolume;
        musicSource.Play();

        while (musicSource.volume < volume)
        {
            musicSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }
}
