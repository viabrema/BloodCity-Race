
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

    public override void OnChangeDialogLine(int index)
    {
        Debug.Log("Mudando linha de diálogo para: " + index);
        // Aqui você pode ativar/desativar poses ou animações específicas
        // Exemplo: Ativar uma pose específica com base no índice
        // switch (index) { ... }
    }
    public override void OnClosingCutscene()
    {
        SceneManager.LoadScene("Race01"); // Carrega a cena da corrida
    }
}

