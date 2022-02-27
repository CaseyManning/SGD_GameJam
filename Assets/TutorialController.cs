using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    enum TutorialStep
    {
        Move, Jump, Rotate, Convert, Convert2, Conclusion, Done
    }

    TutorialStep state;

    float walkTimer = 1f;
    float rotateTimer = 0.5f;
    float convertTimer = 5f;
    float conclusionTimer = 3f;
    float doneTimer = 3f;

    float jumpCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        state = TutorialStep.Move;

        GameObject.FindGameObjectWithTag("Boombox").GetComponent<AudioController>().audioMixerSnapshots[1].TransitionTo(0f);
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
            if (Input.GetAxis("Horizontal") != 0 && (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)))
            {
                rotateTimer -= Time.deltaTime;
            }
            if (rotateTimer <= 0)
            {
                state = TutorialStep.Convert;
                GetComponent<Text>().text = "Press (E) while near a Flower to turn it into a Chicken";
            }
        }
        if(state == TutorialStep.Convert)
        {
            if(GameObject.FindGameObjectsWithTag("Chicken").Length > 0)
            {
                state = TutorialStep.Convert2;
            }
        }
        if (state == TutorialStep.Convert2)
        {
            GetComponent<Text>().text = "Larger objects require more nearby chickens to convert";
            convertTimer -= Time.deltaTime;
            if(convertTimer <= 0)
            {
                GetComponent<Text>().text = "Try to turn the Tree into a Chicken";
            }
            if (GameObject.FindGameObjectsWithTag("Chicken").Length == 5)
            {
                state = TutorialStep.Conclusion;
            }
        }
        if (state == TutorialStep.Conclusion)
        {  
            GetComponent<Text>().text = "Everything in the world can be turned into a chicken";
            conclusionTimer -= Time.deltaTime;
            if (conclusionTimer <= 0)
            {
                state = TutorialStep.Done;
            }
        }
        if(state == TutorialStep.Done)
        {
            GetComponent<Text>().text = "Good Luck!";
            doneTimer -= Time.deltaTime;
            if(doneTimer <= 0)
            {
                SceneManager.LoadScene("Start");
            }
        }
    }
}
