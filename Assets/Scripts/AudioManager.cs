using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; 
    [SerializeField] private AudioSource sfxSource;   

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

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

        musicSource.Stop();
    
        musicSource.clip = musicClip;
        
        musicSource.Play();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void UnpauseMusic()
    {
        musicSource.UnPause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Método para reproducir efectos de sonido
    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null) return;
        
        // PlayOneShot permite reproducir múltiples SFX sin que se corten entre sí
        sfxSource.PlayOneShot(sfxClip);
    }
}