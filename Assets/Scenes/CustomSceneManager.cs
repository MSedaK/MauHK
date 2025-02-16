using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    // Inspector'dan sahne adýný belirleyebilirsiniz.
    public string sceneName;

    // Buton OnClick olayýna ekleyebileceðiniz metod.
    public void LoadScene()
    {
        // Build Settings'te eklediðiniz sahne olduðundan emin olun.
        SceneManager.LoadScene(sceneName);
    }

    // Ýsterseniz parametre alan bir metod da tanýmlayabilirsiniz:
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene("Trailer");
    }
}
