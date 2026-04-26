using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip btnClickSfx;

    private bool _isMuted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static AudioManager GetInstance() => _instance;
    public bool IsMuted() => _isMuted;
    public void ToggleMute()
    {
        _isMuted = !_isMuted;
        AudioListener.pause = _isMuted;
    }

    public void PlayAudio(AudioClip clip, bool loop = false)
    {
        if (clip == null) return;

        if (loop)
        {
            audioSource.pitch = 1f; // Reset pitch to normal
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();
        } else
        {
            float originalPitch = audioSource.pitch;
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clip);
            audioSource.pitch = originalPitch;
        }
    }

    public void PlayBtnSfx()
    {
        if (btnClickSfx != null)
        {
            audioSource.PlayOneShot(btnClickSfx);
        }
    }
}