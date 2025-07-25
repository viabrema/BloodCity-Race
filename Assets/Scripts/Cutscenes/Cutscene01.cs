
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene01 : CutsceneController
{
    public GameObject pose01;
    public GameObject pose02;
    public GameObject pose03;
    public GameObject pose04;
    public override void OnLoadCutscene()
    {
        Debug.Log("Cena 01 carregada, tocando animação...");
        // StartCoroutine, animador, etc.
    }

    public override void OnChangeDialogLine(int index)
    {
        Debug.Log("Mudando linha de diálogo para: " + index);
        // Aqui você pode ativar/desativar poses ou animações específicas
        switch (index)
        {
            case 0:
                pose01.SetActive(true);
                pose02.SetActive(false);
                pose03.SetActive(false);
                pose04.SetActive(false);
                break;
            case 1:
                pose01.SetActive(false);
                pose02.SetActive(true);
                pose03.SetActive(false);
                pose04.SetActive(false);
                break;
            case 2:
                pose01.SetActive(false);
                pose02.SetActive(false);
                pose03.SetActive(true);
                pose04.SetActive(false);
                break;
            case 3:
                pose01.SetActive(false);
                pose02.SetActive(false);
                pose03.SetActive(false);
                pose04.SetActive(true);
                break;
            default:
                Debug.LogWarning("Linha de diálogo desconhecida: " + index);
                break;
        }
    }

    public override void OnClosingCutscene()
    {
        Debug.Log("Cena 01 encerrando");
        Cutscenes.Instance.SetCurrentScene(1);
        // SceneManager.LoadScene("Corrida"); ou ativa HUD etc.
    }
}