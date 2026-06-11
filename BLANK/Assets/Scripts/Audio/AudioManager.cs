using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips (assign in Inspector)")]
    public AudioClip ambientSound;
    public AudioClip footsteps;
    public AudioClip monsterAppear;
    public AudioClip monsterChase;
    public AudioClip scream;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public AudioClip pickup;
    public AudioClip panelClick;
    public AudioClip paper;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = null;

        switch (soundName.ToLower())
        {
            case "ambient": clip = ambientSound; break;
            case "footsteps": clip = footsteps; break;
            case "monsterappear": clip = monsterAppear; break;
            case "monsterchase": clip = monsterChase; break;
            case "scream": clip = scream; break;
            case "dooropen": clip = doorOpen; break;
            case "doorclose": clip = doorClose; break;
            case "pickup": clip = pickup; break;
            case "panelclick": clip = panelClick; break;
            case "paper": clip = paper; break;
        }

        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("[AudioManager] Sound not found or AudioSource missing: " + soundName);
        }
    }

    public void PlayAmbient()
    {
        if (musicSource != null && ambientSound != null)
        {
            musicSource.clip = ambientSound;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopAmbient()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}