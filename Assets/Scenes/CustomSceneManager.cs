using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    // Inspector'dan sahne ad�n� belirleyebilirsiniz.
    public string sceneName;

    // Buton OnClick olay�na ekleyebilece�iniz metod.
    public void LoadScene()
    {
        // Build Settings'te ekledi�iniz sahne oldu�undan emin olun.
        SceneManager.LoadScene(sceneName);
    }

    // �sterseniz parametre alan bir metod da tan�mlayabilirsiniz:
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene("Trailer");
    }
}
