using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TrailerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Inspector �zerinden atay�n
    public string nextSceneName;     // Y�klenecek sahnenin ad�

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Video bitti�inde �a�r�lacak event
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Yeni sahneyi y�kle
        SceneManager.LoadScene("");
    }
}
