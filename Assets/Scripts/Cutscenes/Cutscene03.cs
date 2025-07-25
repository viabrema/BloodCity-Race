
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene03 : CutsceneController
{
    public override void OnLoadCutscene()
    {
        AudioSource music = GameObject.Find("Tension01")?.GetComponent<AudioSource>();
        if (music != null && !music.isPlaying)
        {
            music.volume = RaceManager.Instance.musicVolume;
            music.Play();
        }
        Debug.Log("Cena 03 carregada, tocando animação...");
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
        Cutscenes.Instance.SetCurrentScene("scene04");
    }
}

