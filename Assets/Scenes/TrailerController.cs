using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TrailerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Inspector üzerinden atayýn
    public string nextSceneName;     // Yüklenecek sahnenin adý

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Video bittiðinde çaðrýlacak event
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Yeni sahneyi yükle
        SceneManager.LoadScene("");
    }
}
