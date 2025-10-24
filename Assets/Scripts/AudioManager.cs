using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // El componente que reproducirá la música
    [SerializeField] private AudioSource sfxSource;   // El componente para efectos de sonido cortos

    private void Awake()
    {
        // Patrón Singleton para asegurar que solo haya una instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: para que la música no se corte entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

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