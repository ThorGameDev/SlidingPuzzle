using System.Collections;
using UnityEngine;
public class MusicManager : MonoBehaviour
{
    public AudioSource audioPlayer;
    public Song[] songs;
    static Song[] internalSongs;
    static bool firstSet;
    private string initialSong;
	private int currentSong = -1;
    static int priorSong;
    public float timeUntillRefresh = 120f;
    public float audioBacktrack = 1f;
    public float fade = 0.8f;
    void Awake() 
    {
        //Setting Internals
		initialSong = songs[PlayerPrefs.GetInt("Song",0)].Name;
        if (firstSet == false)
        {
            internalSongs = songs;
            firstSet = true;
        }
		int indexID = 0;
		foreach (Song so in internalSongs)
		{
			if (so.Name == initialSong)
			{
				currentSong = indexID;
				break;
			}
			indexID++;
		}
        UpdateSong();
    }
    private void UpdateSong()
    { 
        if (internalSongs[currentSong].LastPlayed + timeUntillRefresh > Time.time && currentSong != priorSong)
        {
            audioPlayer.clip = internalSongs[currentSong].AC;
            if (internalSongs[currentSong].PointInSong - audioBacktrack > 0)
            {
                audioPlayer.time = internalSongs[currentSong].PointInSong - audioBacktrack;
                StartCoroutine(fadeIn(audioPlayer, fade)) ;
            }
            else
            {
                audioPlayer.time = 0;
            }
            audioPlayer.Play();
        }
        else if(currentSong == priorSong)
        {
            audioPlayer.clip = internalSongs[currentSong].AC;
            audioPlayer.time = internalSongs[currentSong].PointInSong;
            audioPlayer.Play();
        }
        else 
        {
            audioPlayer.clip = internalSongs[currentSong].AC;
            audioPlayer.time = 0;
            audioPlayer.Play();
        }
    }
    public void RequestNewSong(string Name)
    {
        int indexID = 0;
        foreach (Song so in internalSongs)
        {
            if (so.Name == Name)
            {
                currentSong = indexID;
                break;
            }
            indexID++;
        }
        if (currentSong == -1)
        {
            Debug.LogError("Failed to find proper song");
            currentSong = 1;
        }
        UpdateSong();
    }
    void Update()
    {
        internalSongs[currentSong].PointInSong = audioPlayer.time;
        internalSongs[currentSong].LastPlayed = Time.time;
        priorSong = currentSong;
        if(!Infade)
        {
            audioPlayer.volume = 1;
        }
    }
    public void PlaySound(AudioClip Sound)
    {
        audioPlayer.PlayOneShot(Sound,1);
    }
    private static bool Infade;
    public static IEnumerator fadeIn(AudioSource audioSource, float fadeTime)
    {
        Infade = true;
        float startVolume = 0.2f;
        float TrueVolume = 1;
        float SubstituteVolume = 0;
        audioSource.volume = SubstituteVolume * TrueVolume;
        audioSource.Play();
        while (SubstituteVolume < 1.0f)
        {
            SubstituteVolume += startVolume * Time.fixedDeltaTime / fadeTime;
            audioSource.volume = SubstituteVolume * TrueVolume;
            yield return new WaitForEndOfFrame();
        }
        SubstituteVolume = 1f;
        audioSource.volume = SubstituteVolume * TrueVolume;
        Infade = false;
    }
}
[System.Serializable]
public struct Song
{
    public string Name;
    public AudioClip AC;
    public float PointInSong;
    public float LastPlayed;
}
[System.Serializable]
public struct ScenePreset
{
    public string Name;
    public string initialSong;
}
