using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject characterA;
    public GameObject characterB;
    private GameObject activeCharacter;
    private CameraFollow cameraFollow;

    void Start()
    {
        activeCharacter = characterA;
        characterA.SetActive(true);
        characterB.SetActive(false);

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.SetTarget(activeCharacter.transform);
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
        Vector3 currentPosition = activeCharacter.transform.position;

        activeCharacter.SetActive(false);

        activeCharacter = (activeCharacter == characterA) ? characterB : characterA;

        activeCharacter.transform.position = currentPosition;
        activeCharacter.SetActive(true);

        cameraFollow.SetTarget(activeCharacter.transform);
    }
}