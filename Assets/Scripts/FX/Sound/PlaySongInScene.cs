using UnityEngine;

public class PlaySongInScene : MonoBehaviour
{
    public int SongID = 0;
    void Start()
    {
        MusicManager.PlayNewSong(SongID);
    }

}
