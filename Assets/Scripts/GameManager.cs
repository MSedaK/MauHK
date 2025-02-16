using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject characterA;
    public GameObject characterB;
    private GameObject activeCharacter;
    private CameraFollow cameraFollow;

    [Header("VFX Settings")]
    public GameObject swapVFXPrefab; // VFX prefab'ini buraya atacaðýz

    [Header("Music Settings")]
    public AudioClip backgroundMusic;  // Müzik dosyasýný buraya atayacaðýz
    private AudioSource audioSource;    // AudioSource component'ini tanýmlýyoruz

    void Start()
    {
        activeCharacter = characterA;
        characterA.SetActive(true);
        characterB.SetActive(false);

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.SetTarget(activeCharacter.transform);

        // Müzik çalar (AudioSource) component'ini alýyoruz ve müziði baþlatýyoruz
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;  // Müzik sürekli çalsýn
            audioSource.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwapCharacter();
        }
    }

    void SwapCharacter()
    {
        if (swapVFXPrefab != null)
        {
            Instantiate(swapVFXPrefab, activeCharacter.transform.position, Quaternion.identity);
        }

        Vector3 currentPosition = activeCharacter.transform.position;

        activeCharacter.SetActive(false);

        activeCharacter = (activeCharacter == characterA) ? characterB : characterA;

        activeCharacter.transform.position = currentPosition;

        activeCharacter.SetActive(true);

        cameraFollow.SetTarget(activeCharacter.transform);
    }
}
