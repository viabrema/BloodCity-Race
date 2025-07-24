
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene01 : CutsceneController
{
    public override void OnLoadCutscene()
    {
        Debug.Log("Cena 01 carregada, tocando animação...");
        // StartCoroutine, animador, etc.
    }

    public override void OnClosingCutscene()
    {
        Debug.Log("Cena 01 encerrando, vai começar a corrida!");
        // SceneManager.LoadScene("Corrida"); ou ativa HUD etc.
    }
}