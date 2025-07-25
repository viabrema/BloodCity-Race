
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene06 : CutsceneController
{
    public GameObject pose01;
    public GameObject pose02;

    public override void OnLoadCutscene()
    {
        AudioSource music = GameObject.Find("Tension01")?.GetComponent<AudioSource>();
        if (music != null && !music.isPlaying)
        {
            music.volume = RaceManager.Instance.musicVolume;
            music.Play();
        }
        Debug.Log("Cena 06 carregada, tocando animação...");
        pose01.SetActive(true);
        pose02.SetActive(true);
    }

    public override void OnChangeDiaNextLine(int index)
    {
        Debug.Log("Mudando linha de diálogo para: " + index);
        // Aqui você pode ativar/desativar poses ou animações específicas
        switch (index)
        {
            case 0:
                pose01.SetActive(true);
                pose02.SetActive(true);
                break;
            default:
                Debug.LogWarning("Linha de diálogo desconhecida: " + index);
                break;
        }
    }
    public override void OnClosingCutscene()
    {
        SceneManager.LoadScene("Race01"); // Carrega a cena da corrida
    }
}

