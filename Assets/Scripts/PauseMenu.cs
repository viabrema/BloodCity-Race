using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{

    public Button exitGameButton;
    public Button resumeButton;
    // Start is called before the first frame update
    void Start()
    {
        if (exitGameButton == null || resumeButton == null)
        {
            Debug.LogError("PauseScreen: Missing references in the inspector.");
            return;
        }

        exitGameButton.onClick.AddListener(() => Application.Quit());
        resumeButton.onClick.AddListener(() => RaceManager.Instance.PauseGame(false));

    }

    // Update is called once per frame
    void Update()
    {

    }
}
