using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class DialogManager: MonoBehaviour 
{
    public string[] Lines;
    public float SpeedText;
    public float EraseTime;
    public TMP_Text DialogText;

    private int index;
    private bool typing;

    private void Start()
    {
        DialogText.text = string.Empty;
        typing = false;
    }


    public void StartDialog()
    {
        if (!typing)
        {
            index = 0;
            typing = true;
            StartCoroutine(TypeLine());
        }
    }
    IEnumerator TypeLine()
    {
        
        DialogText.text = string.Empty;
        foreach (char c in Lines[index].ToCharArray())
        {
            DialogText.text += c;
            yield return new WaitForSeconds(SpeedText);
        }
        typing = false;
        StartCoroutine (EraseLineInSeconds(EraseTime));
    }


    IEnumerator EraseLineInSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        DialogText.text = string.Empty;
    }
}
