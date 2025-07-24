using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class DialogLine
{
    public string name;
    [TextArea]
    public string text;

    public DialogLine(string name, string text)
    {
        this.name = name;
        this.text = text;
    }
}

[Serializable]
public class DialogScene
{
    public string sceneId;
    public List<DialogLine> lines = new List<DialogLine>();

    public DialogScene(string sceneId)
    {
        this.sceneId = sceneId;
    }
}

public class Dialog : MonoBehaviour
{
    public List<DialogScene> scenes = new List<DialogScene>();
    public int currentSceneIndex = 0;
    public int currentLineIndex = 0;

    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI characterNameText;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string fullLine = "";

    void Awake()
    {
        // Cena 1
        var scene1 = new DialogScene("scene01");
        scene1.lines.Add(new DialogLine("Funcionário", "Você fracassou. Sem pagamento, sem liberação."));
        scene1.lines.Add(new DialogLine("Protagonista", "Eu não vou parar. Ela ainda está viva."));
        scene1.lines.Add(new DialogLine("Funcionário", "Então continue correndo. A FaithLab agradece."));

        // Cena 2
        var scene2 = new DialogScene("scene02");
        scene2.lines.Add(new DialogLine("Funcionário", "Você de novo? Isso está ficando cansativo."));
        scene2.lines.Add(new DialogLine("Protagonista", "Vocês não vão apagar a mente dela."));
        scene2.lines.Add(new DialogLine("Funcionário", "Ela já é parte do sistema. E você, uma variável irrelevante."));

        scenes.Add(scene1);
        scenes.Add(scene2);
    }

    void Start()
    {
        ShowCurrentLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogText.text = fullLine;
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void ShowCurrentLine()
    {
        if (currentSceneIndex < scenes.Count)
        {
            var currentScene = scenes[currentSceneIndex];

            if (currentLineIndex < currentScene.lines.Count)
            {
                DialogLine line = currentScene.lines[currentLineIndex];
                fullLine = line.text;
                characterNameText.text = line.name;

                // Inicia efeito de máquina de escrever
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);
                typingCoroutine = StartCoroutine(TypeText(fullLine));
            }
        }
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.02f); // Ajuste a velocidade aqui
        }

        isTyping = false;
    }

    public void NextLine()
    {
        var currentScene = scenes[currentSceneIndex];

        if (currentLineIndex < currentScene.lines.Count - 1)
        {
            currentLineIndex++;
            ShowCurrentLine();
        }
        else
        {
            Debug.Log("Fim do diálogo.");
            // Aqui você pode desativar o painel, ativar botão, chamar evento, etc.
        }
    }

    public List<DialogLine> GetDialog(string sceneId)
    {
        foreach (var scene in scenes)
        {
            if (scene.sceneId == sceneId)
                return scene.lines;
        }
        return null;
    }
}
