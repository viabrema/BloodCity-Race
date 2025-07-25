using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cutscene07 : CutsceneController
{
    public GameObject pose01;
    public GameObject pose02;
    public GameObject pose03;
    public GameObject pose04;

    public GameObject choice;

    public Button button1;
    public Button button2;

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
        choice.SetActive(false);
    }


    void ApplyButtonsEvents()
    {
        button1.onClick.RemoveAllListeners();
        button1.onClick.AddListener(Choice1);
        button2.onClick.RemoveAllListeners();
        button2.onClick.AddListener(Choice2);

        // Adiciona eventos de hover para ambos os botões
        AddHoverEvents(button1);
        AddHoverEvents(button2);
    }

    void AddHoverEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Evento para quando o mouse entra
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => HoverButton(button));
        trigger.triggers.Add(pointerEnter);

        // Evento para quando o mouse sai
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => UnhoverButton(button));
        trigger.triggers.Add(pointerExit);
    }

    void HoverButton(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.DOKill(); // Cancela animações anteriores

        Vector2 target = rect.anchoredPosition + new Vector2(10f, 0f);
        rect.DOAnchorPos(target, 0.2f).SetEase(Ease.OutQuad);
    }

    void UnhoverButton(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.DOKill();

        Vector2 target = rect.anchoredPosition - new Vector2(10f, 0f);
        rect.DOAnchorPos(target, 0.2f).SetEase(Ease.OutQuad);
    }

    void Choice1()
    {
        Debug.Log("Escolha 1 selecionada");
        Cutscenes.Instance.SetCurrentScene("scene08");
    }

    void Choice2()
    {
        Debug.Log("Escolha 2 selecionada");
        Cutscenes.Instance.SetCurrentScene("scene08");
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

            case 19:
                choice.SetActive(true);
                ApplyButtonsEvents();
                break;
            default:
                Debug.LogWarning("Linha de diálogo desconhecida: " + index);
                break;
        }
    }
    public override void OnClosingCutscene()
    {

        // SceneManager.LoadScene("SampleScene"); // Volta para o menu inicial


    }
}

