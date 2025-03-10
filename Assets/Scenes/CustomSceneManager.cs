using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    // Inspector'dan sahne adını belirleyebilirsiniz.
    public string sceneName;

    // Buton OnClick olayına ekleyebileceğiniz metod.
    public void LoadScene()
    {
        // Build Settings'te eklediğiniz sahne olduğundan emin olun.
        SceneManager.LoadScene(sceneName);
    }

    // İsterseniz parametre alan bir metod da tanımlayabilirsiniz:
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene("Trailer");
    }
}
