using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OptionsManager : MonoBehaviour
{
	public Slider song;
	public Slider boardSize;
	public MusicManager musicManager;
	public TMP_Text boardSizeTitle;
    private void Start()
    {
		song.maxValue = musicManager.songs.Length - 1;
		boardSize.value = PlayerPrefs.GetInt("Size");
		song.value = PlayerPrefs.GetInt("Song");
    }
    public void UpdateSong()
    {
		musicManager.RequestNewSong(musicManager.songs[Mathf.RoundToInt(song.value)].Name); 
    }
	public void UpdateBoard()
	{
		boardSizeTitle.text = $"Board Size: {Mathf.RoundToInt(boardSize.value)}";
	}
	private void OnDestroy()
	{
		PlayerPrefs.SetInt("Song", Mathf.RoundToInt(song.value));
		PlayerPrefs.SetInt("Size", Mathf.RoundToInt(boardSize.value));
	}
}
