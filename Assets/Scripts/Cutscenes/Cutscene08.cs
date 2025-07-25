
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Cutscene08 : CutsceneController
{

    public GameObject dialog;
    public override void OnLoadCutscene()
    {
        AudioSource music = GameObject.Find("Tension01")?.GetComponent<AudioSource>();
        music.Stop();
        Debug.Log("Cena 03 carregada, tocando animação...");
        dialog.SetActive(false);
        // StartCoroutine, animador, etc.
    }

    public override void OnChangeDiaNextLine(int index)
    {
        Debug.Log("Mudando linha de diálogo para: " + index);
        // Aqui você pode ativar/desativar poses ou animações específicas
        // Exemplo: Ativar uma pose específica com base no índice
        // switch (index) { ... }
    }
    public override void OnClosingCutscene()
    {
        SceneManager.LoadScene("SampleScene"); // Carrega a cena da corrida
    }
}

