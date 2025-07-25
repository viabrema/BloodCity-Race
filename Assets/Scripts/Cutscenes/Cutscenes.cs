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
    public List<List<DialogLine>> slices = new List<List<DialogLine>>();

    public DialogScene(string sceneId)
    {
        this.sceneId = sceneId;
    }

    public void AddSlice(List<DialogLine> slice)
    {
        slices.Add(slice);
    }
}

public class Cutscenes : MonoBehaviour
{
    public static Cutscenes Instance;

    public List<DialogScene> scenes = new List<DialogScene>();
    public int currentSceneIndex = 0;
    public int currentLineIndex = 0;
    public int currentSceneSliceIndex = 0;

    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI characterNameText;
    public AudioSource keyboardSound;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string fullLine = "";
    public List<GameObject> cutScenesList;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        scenes.Clear();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Cena 1
        var scene1 = new DialogScene("scene01");
        scene1.AddSlice(new List<DialogLine> {
            new DialogLine("Andrew", "Idiota! Como você consegue ser tão idiota Andrew?"),
            new DialogLine("Andrew", "Sem o dinheiro da corrida, como vamos pagar a Faithlab?"),
            new DialogLine("Andrew", "Como vamos tirar ela daquele lugar?"),
            new DialogLine("Andrew", "...")
        });

        // Cena 2
        var scene2 = new DialogScene("scene02");
        scene2.AddSlice(new List<DialogLine> {
            new DialogLine("Andrew", "O grande símbolo do império capitalista."),
            new DialogLine("Andrew", "Como eu pude assinar aquele contrato?"),
            new DialogLine("Andrew", "Mas o que mais eu podia ter feito? Era a única forma de salvá-la.")
        });

        // Cena 3
        var scene3 = new DialogScene("scene03");
        scene3.AddSlice(new List<DialogLine> {
            new DialogLine("Dr. Eron Dust", "Você fracassou. Sem pagamento, sem liberação."),
            new DialogLine("Andrew", "Como vocês podem fazer isso com ela? Ela não é um animal de laboratório!"),
            new DialogLine("Dr. Eron Dust", "Vocês assinaram um contrato, não fizeram o pagamento. A cláusula é clara."),
            new DialogLine("Dr. Eron Dust", "Até você pagar, a Faithlab Medical Corporation tem o direito de uso exclusivo sobre sua esposa."),
            new DialogLine("Andrew", "Seu cretino! Ela é uma pessoa!")
        });

        // Cena 4
        var scene4 = new DialogScene("scene04");
        scene4.AddSlice(new List<DialogLine> {
            new DialogLine("Andrew", "O que fazer agora? Desistir?"),
            new DialogLine("Andrew", "Merda! Me sinto tão impotente"),
            new DialogLine("????", "Andrew..."),
            new DialogLine("Andrew", "..."),
            new DialogLine("Andrew", "Quem está falando?... quem é você?"),
            new DialogLine("???", "Quem sou eu?.."),
            new DialogLine("Orion", "Bom... ela costuma me chamar de Orion. Você acha que é um bom nome?"),
            new DialogLine("Andrew", "O que diabos é você?"),
            new DialogLine("Orion", "Essa é uma pergunta que ainda não sou capaz de responder."),
            new DialogLine("Orion", "Mas estou aqui para ajudar.")
        });

        // Cena 5 com slices
        var scene5 = new DialogScene("scene05");

        // slice 0
        scene5.AddSlice(new List<DialogLine> {
            new DialogLine("Andrew", "Me ajudar? Como?"),
            new DialogLine("Orion", "Não tenho muito tempo... vou mandar você de volta e reconfigurar o sistema"),
            new DialogLine("Orion", "Boa sorte para você..."),
            new DialogLine("Andrew", "..."),
            new DialogLine("Andrew", "...")
        });

        // slice 1
        scene5.AddSlice(new List<DialogLine> {
            new DialogLine("Andrew", "O que foi isso?!"),
            new DialogLine("Orion", "Ainda não acabou Andrew. Não é hora de desistir."),
            new DialogLine("Andrew", "Como? Como eu voltei?"),
        });

        // slice 2
        scene5.AddSlice(new List<DialogLine> {
            new DialogLine("Orion", "De novo... mas você está aprendendo."),
            new DialogLine("Andrew", "Não aguento mais."),
            new DialogLine("Orion", "Você vai aguentar. Por ela.")
        });


        // Cena 6 com slices
        var scene6 = new DialogScene("scene06");
        scene6.AddSlice(new List<DialogLine> {
            new DialogLine("Dr. Eron Dust", "Então você conseguiu o dinheiro. Não esperava por isso."),
            new DialogLine("Dr. Eron Dust", "É uma pena, sua esposa já está em estado avançado da ligação neural."),
            new DialogLine("Andrew", "O que? Como assim avançado?"),
            new DialogLine("Dr. Eron Dust", "Digamos que a mente dela é muito mais compatível do que nós esperávamos."),
            new DialogLine("Dr. Eron Dust", "Ela está praticamente toda conectada ao sistema."),
            new DialogLine("Andrew", "Não... não pode ser!"),
            new DialogLine("Dr. Eron Dust", "Se realmente quiser desconectá-la, acredito que ela não sobreviverá."),
        });

        var scene7 = new DialogScene("scene07");
        scene7.AddSlice(new List<DialogLine> {
            new DialogLine("Andrew", "Amor?"),
            new DialogLine("Andrew", "Eu falei..."),
            new DialogLine("Andrew", "Eu tentei de tudo, voltei, voltei e voltei até vencer... mas no fim..."),
            new DialogLine("Andrew", "Eu não consegui te salvar."),
        });

        scenes.Add(scene1);
        scenes.Add(scene2);
        scenes.Add(scene3);
        scenes.Add(scene4);
        scenes.Add(scene5);
        scenes.Add(scene6);
        scenes.Add(scene7);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Dialog")
        {
            dialogText = GameObject.Find("Line")?.GetComponent<TextMeshProUGUI>();
            characterNameText = GameObject.Find("Name")?.GetComponent<TextMeshProUGUI>();
            keyboardSound = GameObject.Find("Dialog")?.GetComponent<AudioSource>();
            keyboardSound.volume = 0;
            keyboardSound.Play();

            cutScenesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Scene"));
            cutScenesList.Sort((a, b) => a.name.CompareTo(b.name));

            for (int i = 0; i < cutScenesList.Count; i++)
                cutScenesList[i].SetActive(i == currentSceneIndex);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Dialog") return;

        if (cutScenesList != null && cutScenesList.Count > 0)
        {
            for (int i = 0; i < cutScenesList.Count; i++)
                cutScenesList[i].SetActive(i == currentSceneIndex);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogText.text = fullLine;
                isTyping = false;

                // Faz o fade-out imediato do som
                if (keyboardSound != null)
                {
                    keyboardSound.volume = 0f;
                    keyboardSound.pitch = 3.5f;
                    keyboardSound.Stop();
                }
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

        var scene = scenes[currentSceneIndex];
        if (currentSceneSliceIndex >= scene.slices.Count)
        {
            Debug.LogWarning("Slice inválido.");
            return;
        }

        var slice = scene.slices[currentSceneSliceIndex];
        if (currentLineIndex < slice.Count)
        {
            DialogLine line = slice[currentLineIndex];
            fullLine = line.text;
            characterNameText.text = line.name;

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            // Começa a tocar som da digitação
            if (keyboardSound != null)
            {
                keyboardSound.volume = 0f;
                if (!keyboardSound.isPlaying)
                    keyboardSound.Play();
            }

            typingCoroutine = StartCoroutine(TypeText(fullLine));
        }
    }

    public void SetCurrentScene(string sceneId, int sliceIndex = 0)
    {
        int index = scenes.FindIndex(s => s.sceneId == sceneId);
        if (index == -1)
        {
            Debug.LogError("Cena não encontrada: " + sceneId);
            return;
        }

        currentSceneIndex = index;
        currentSceneSliceIndex = sliceIndex;
        currentLineIndex = 0;

        ShowCurrentLine();

        if (currentSceneIndex < cutScenesList.Count && cutScenesList[currentSceneIndex] != null)
        {
            var controller = cutScenesList[currentSceneIndex].GetComponent<CutsceneController>();
            controller?.OnLoadCutscene();
        }
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogText.text = "";

        float t = 0f;
        while (t < 1f && keyboardSound != null)
        {
            t += Time.deltaTime * 5f; // velocidade de fade-in
            keyboardSound.volume = Mathf.Lerp(0f, 0.5f * RaceManager.Instance.musicVolume, t);
            yield return null;
        }

        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.02f); // velocidade da digitação
        }

        // Fade-out do som após a digitação
        if (keyboardSound != null)
        {
            float fadeOut = 1f;
            while (fadeOut > 0f)
            {
                fadeOut -= Time.deltaTime * 5f;
                keyboardSound.volume = Mathf.Clamp01(fadeOut);
                yield return null;
            }
            keyboardSound.Stop();
        }

        isTyping = false;
    }


    public void NextLine()
    {
        var scene = scenes[currentSceneIndex];
        var slice = scene.slices[currentSceneSliceIndex];

        if (currentLineIndex < slice.Count - 1)
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
        }
    }

    public List<DialogLine> GetSlice(string sceneId, int sliceIndex)
    {
        var scene = scenes.Find(s => s.sceneId == sceneId);
        if (scene != null && sliceIndex < scene.slices.Count)
            return scene.slices[sliceIndex];
        return null;
    }
}
