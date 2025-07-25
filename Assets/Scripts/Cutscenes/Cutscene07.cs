
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene07 : CutsceneController
{
    public GameObject pose01;
    public GameObject pose02;
    public GameObject pose03;
    public GameObject pose04;

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
        pose03.SetActive(true);
        pose04.SetActive(false);
    }

    public override void OnChangeDiaNextLine(int index)
    {
        Debug.Log("Mudando linha de diálogo para: " + index);
        // Aqui você pode ativar/desativar poses ou animações específicas
        switch (index)
        {
            case 5:
                pose04.SetActive(true);
                break;
            default:
                Debug.LogWarning("Linha de diálogo desconhecida: " + index);
                break;
        }
    }
    public override void OnClosingCutscene()
    {
        SceneManager.LoadScene("SampleScene"); // Volta para o menu inicial
    }
}

