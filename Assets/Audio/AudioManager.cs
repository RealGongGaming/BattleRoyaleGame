using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clips")]
    public AudioClip gameBGM;
    public AudioClip lobbyBGM;
    public AudioClip parry;
    public AudioClip stun;
    public AudioClip overall_victory;
    public AudioClip round_victory;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM(lobbyBGM);
    }

    public void PlayBGM(AudioClip newBGM)
    {
        if (musicSource.clip == newBGM) return;

        musicSource.clip = newBGM;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
