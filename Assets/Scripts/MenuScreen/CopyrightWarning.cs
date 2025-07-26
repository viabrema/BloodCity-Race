using UnityEngine;
using UnityEngine.SceneManagement;

public class CopyrightWarning : MonoBehaviour
{
    void Start()
    {
        Invoke("LoadNextScene", 4f); // mostra por 4 segundos
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("SampleScene"); // ou o nome da sua próxima cena
    }
}
