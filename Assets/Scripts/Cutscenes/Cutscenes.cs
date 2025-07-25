using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

public class Cutscenes : MonoBehaviour
{
    public static Cutscenes Instance;

    public List<DialogScene> scenes = new List<DialogScene>();
    public int currentSceneIndex = 0;
    public int currentLineIndex = 0;

    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI characterNameText;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string fullLine = "";
    public List<GameObject> cutScenesList;

    void Awake()
    {

        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Suas cenas
        scenes.Clear(); // Evita duplicatas se voltar de alguma cena


        SceneManager.sceneLoaded += OnSceneLoaded;

        // Cena 1
        var scene1 = new DialogScene("scene01");
        scene1.lines.Add(new DialogLine("Andrew", "Idiota! Como você consegue ser tão idiota Andrew?"));
        scene1.lines.Add(new DialogLine("Andrew", "Sem o dinheiro da corrida, como vamos pagar a Faithlab?"));
        scene1.lines.Add(new DialogLine("Andrew", "Como vamos tirar ela daquele lugar?"));
        scene1.lines.Add(new DialogLine("Andrew", "..."));

        // Cena 2
        var scene2 = new DialogScene("scene02");
        scene2.lines.Add(new DialogLine("Andrew", "Lá está ela, o grande símbolo do império capitalista. A Faithlab."));
        scene2.lines.Add(new DialogLine("Andrew", "Como eu pude assinar aquele contrato?"));
        scene2.lines.Add(new DialogLine("Andrew", "Mas o que mais eu podia fazer? Era a única forma de salvá-la."));

        var scene3 = new DialogScene("scene03");
        scene3.lines.Add(new DialogLine("Dr. Eron Dust", "Você fracassou. Sem pagamento, sem liberação."));
        scene3.lines.Add(new DialogLine("Andrew", "Como vocês podem fazer isso com ela? Ela não é um animal de laboratório!"));
        scene3.lines.Add(new DialogLine("Dr. Eron Dust", "Vocês assinaram um contrato, não fizeram o pagamento. A cláusula é clara."));
        scene3.lines.Add(new DialogLine("Dr. Eron Dust", "Até você pagar, sua esposa é propriedade da Faithlab Medical Corporation."));
        scene3.lines.Add(new DialogLine("Andrew", "Seu cretino! Ela é uma pessoa!"));

        var scene4 = new DialogScene("scene04");
        scene4.lines.Add(new DialogLine("Andrew", "O que fazer agora? Desistir?"));
        scene4.lines.Add(new DialogLine("Andrew", "Merda! Me sinto tão impotente quanto uma criança."));
        scene4.lines.Add(new DialogLine("????", "Andrew..."));
        scene4.lines.Add(new DialogLine("Andrew", "..."));


        scenes.Add(scene1);
        scenes.Add(scene2);
        scenes.Add(scene3);
        scenes.Add(scene4);
    }

    void Start()
    {
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Dialog")
        {
            dialogText = GameObject.Find("Line")?.GetComponent<TextMeshProUGUI>();
            characterNameText = GameObject.Find("Name")?.GetComponent<TextMeshProUGUI>();

            // Busca todos os GameObjects com tag "Scene"
            cutScenesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Scene"));

            // Ordena por nome (alfabética → respeita Scene_01, Scene_02, etc.)
            cutScenesList.Sort((a, b) => a.name.CompareTo(b.name));

            // Reinicia visual para garantir estado inicial
            for (int i = 0; i < cutScenesList.Count; i++)
                cutScenesList[i].SetActive(i == currentSceneIndex);

            SetCurrentScene(0);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Dialog") return;

        if (cutScenesList != null && cutScenesList.Count > 0)
        {
            for (int i = 0; i < cutScenesList.Count; i++)
            {

                cutScenesList[i].SetActive(i == currentSceneIndex);

            }
        }

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
        if (SceneManager.GetActiveScene().name != "Dialog") return;
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

    public void SetCurrentScene(int index)
    {
        if (index < 0 || index >= scenes.Count)
        {
            Debug.LogError("Índice de cena inválido: " + index);
            return;
        }

        if (currentSceneIndex < cutScenesList.Count && cutScenesList[currentSceneIndex] != null)
        {
            var controller = cutScenesList[currentSceneIndex].GetComponent<CutsceneController>();
            controller?.OnLoadCutscene();
        }

        currentSceneIndex = index;
        currentLineIndex = 0;
        ShowCurrentLine();
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
            if (currentSceneIndex < cutScenesList.Count && cutScenesList[currentSceneIndex] != null)
            {
                var controller = cutScenesList[currentSceneIndex].GetComponent<CutsceneController>();
                controller?.OnChangeDiaNextLine(currentLineIndex);
            }
        }
        else
        {
            if (currentSceneIndex < cutScenesList.Count && cutScenesList[currentSceneIndex] != null)
            {
                var controller = cutScenesList[currentSceneIndex].GetComponent<CutsceneController>();
                controller?.OnClosingCutscene();
            }
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
