using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoreController : MonoBehaviour
{
    public GameObject[] texts;

    float lastChangeTime = 0;

    int currentText = 0;

    // Start is called before the first frame update
    void Start()
    {
        lastChangeTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) && Time.time - lastChangeTime > 1)
        {
            lastChangeTime = Time.time;
            if (currentText == texts.Length - 1)
            {
                SceneManager.LoadScene("GameScene");
            } else
            {
                texts[currentText].SetActive(false);
                currentText += 1;
                texts[currentText].SetActive(true);
            }
        }
    }
}
