
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene02 : CutsceneController
{
    public override void OnLoadCutscene()
    {
        Debug.Log("Cena 02 carregada, tocando animação...");
        // StartCoroutine, animador, etc.
    }

    public override void OnClosingCutscene()
    {
        SceneManager.LoadScene("Race01"); // Carrega a cena da corrida
    }
}

