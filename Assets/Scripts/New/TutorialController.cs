using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] GameObject Label_PathSteps;
    [SerializeField] GameObject Label_SpecialTiles;
    [SerializeField] GameObject Label_Pools;

    [SerializeField] GameObject Overview;

    [SerializeField] GameObject Step_PieceMovement_1;
    [SerializeField] GameObject Step_PieceMovement_2;
    [SerializeField] GameObject Step_PieceMovement_3;
    [SerializeField] GameObject Step_SpecialTiles_1;
    [SerializeField] GameObject Step_Capturing_1;

    int currentStep = 0;

    void Start()
    {

    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            currentStep++;
            switch (currentStep)
            {
                case 1:
                    Overview.SetActive(false);
                    Label_PathSteps.SetActive(true);
                    Step_PieceMovement_1.SetActive(true);
                    break;
                case 2:
                    Step_PieceMovement_1.SetActive(false);
                    Step_PieceMovement_2.SetActive(true);
                    break;
                case 3:
                    Step_PieceMovement_2.SetActive(false);
                    Step_PieceMovement_3.SetActive(true);
                    break;
                case 4:
                    Step_PieceMovement_3.SetActive(false);
                    Step_SpecialTiles_1.SetActive(true);
                    break;
                case 5:
                    Label_PathSteps.SetActive(false);
                    Label_SpecialTiles.SetActive(true);
                    break;
                case 6:
                    Step_SpecialTiles_1.SetActive(false);
                    Step_Capturing_1.SetActive(true);
                    break;
                case 7:
                    SceneManager.LoadScene("MainMenu");
                    break;
            }
        }
    }
}
