using UnityEngine;

public class MusicMasterScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioSource[] songs;
    private int currentlyPlaying;
    public int enemyCannons;
    public bool kraken;
    public bool shark;

    public void menu()
    {
        songs[currentlyPlaying].Stop();
        songs[0].Play();
        currentlyPlaying = 0;
    }

    public void background()
    {
        if (enemyCannons > 0)
        {
            fortAttack();
        }
        else if(shark)
        {
            sharkAttack();
        }
        else if (kraken)
        {
            krakenAttack();
        }
        else
        {
            Debug.Log("Here");
            songs[currentlyPlaying].Stop();
            songs[1].Play();
            currentlyPlaying = 1;
        }
        
    }

    public void krakenAttack()
    {
        if (!kraken)
        {
            songs[currentlyPlaying].Stop();
            songs[2].Play();
            currentlyPlaying = 2;
            kraken = true;
        }
        
    }

    public void sharkAttack()
    {
        if (!shark)
        {
            songs[currentlyPlaying].Stop();
            songs[3].Play();
            currentlyPlaying = 3; 
            shark = true;
        }
        
    }

    public void fortAttack()
    {
        if(enemyCannons == 0)
        {
            Debug.Log("Fortnite");
            songs[currentlyPlaying].Stop();
            songs[4].Play();
            currentlyPlaying = 4;
        }
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

    public void stop()
    {
        songs[currentlyPlaying].Stop();
    }

    public void fortEnd()
    {
        Debug.Log("lslsls");
        enemyCannons = 0;
        background();
    }

    void Start()
    {
        currentlyPlaying = 0;
    }
}
