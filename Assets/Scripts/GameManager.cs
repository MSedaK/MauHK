using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject characterA;
    public GameObject characterB;

    private GameObject activeCharacter;

    void Start()
    {
        activeCharacter = characterA;
        characterA.SetActive(true);
        characterB.SetActive(false);
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
        activeCharacter.SetActive(false);

        activeCharacter = (activeCharacter == characterA) ? characterB : characterA;

        activeCharacter.SetActive(true);
    }
}
