using UnityEngine;

public class MusicMasterScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioSource[] songs;
    private int currentlyPlaying;

    public void menu()
    {
        songs[currentlyPlaying].Stop();
        songs[0].Play();
        currentlyPlaying = 0;
    }

    public void background()
    {
        songs[currentlyPlaying].Stop();
        songs[1].Play();
        currentlyPlaying = 1;
    }

    public void krakenAttack()
    {
        songs[currentlyPlaying].Stop();
        songs[2].Play();
        currentlyPlaying = 2;
    }

    public void sharkAttack()
    {
        songs[currentlyPlaying].Stop();
        songs[3].Play();
        currentlyPlaying = 3;
    }

    public void fortAttack()
    {
        songs[currentlyPlaying].Stop();
        songs[4].Play();
        currentlyPlaying = 4;
    }

    public void lose()
    {
        songs[currentlyPlaying].Stop();
        songs[5].Play();
        currentlyPlaying = 5;
    }

    public void win()
    {
        songs[currentlyPlaying].Stop();
        songs[6].Play();
        currentlyPlaying = 6;
    }

    void Start()
    {
        currentlyPlaying = 0;
    }
}
