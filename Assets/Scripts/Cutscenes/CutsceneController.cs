
using UnityEngine;

public abstract class CutsceneController : MonoBehaviour
{
    public virtual void OnLoadCutscene() { }
    public virtual void OnClosingCutscene() { }
    public virtual void OnChangeDialogLine(int index) { }
}