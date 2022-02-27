using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    enum TutorialStep
    {
        Move, Jump, Rotate, Convert, Done
    }

    TutorialStep state;

    float walkTimer = 1f;
    float rotateTimer = 1f;
    float doneTimer = 2f;

    float jumpCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        state = TutorialStep.Move;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == TutorialStep.Move)
        {
            if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                walkTimer -= Time.deltaTime;
            }
            if(walkTimer <= 0)
            {
                state = TutorialStep.Jump;
                GetComponent<Text>().text = "Press Space to Fly (repeat while in air)";
            }
        }
        if(state == TutorialStep.Jump)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                jumpCount += 1;
            }
            if(jumpCount > 2)
            {
                state = TutorialStep.Rotate;
                GetComponent<Text>().text = "Click and Drag to Rotate Camera";
            }
        }
        if(state == TutorialStep.Rotate)
        {
            if (Input.GetAxis("Horizontal") != 0 && Input.GetMouseButton(0))
            {
                rotateTimer -= Time.deltaTime;
            }
            if (rotateTimer <= 0)
            {
                state = TutorialStep.Convert;
                GetComponent<Text>().text = "Press (E) While Near the Flower to Turn it Into a Chicken";
            }
        }
        if(state == TutorialStep.Convert)
        {
            if(GameObject.FindGameObjectsWithTag("Chicken").Length > 0)
            {
                GetComponent<Text>().text = "Good Luck!";
                state = TutorialStep.Done;
            }
        }
        if(state == TutorialStep.Done)
        {
            doneTimer -= Time.deltaTime;
            if(doneTimer < 0)
            {
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}
