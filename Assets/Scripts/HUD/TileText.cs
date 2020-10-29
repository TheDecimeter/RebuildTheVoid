using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileText : MonoBehaviour
{
    public TextMeshPro text;

    private void Awake()
    {
        text.text = "";
    }

    public void LookAt(Transform t)
    {
        transform.LookAt(t);
        transform.forward = -transform.forward;
    }

    public void Show(string message)
    {
        text.text = message;
    }

    public void Show(string message, float time)
    {
        StartCoroutine(ShowTempMessageCoroutine(message, time));
    }

    private IEnumerator ShowTempMessageCoroutine(string message, float time)
    {
        string oldMessage = text.text;
        text.text = message;
        yield return new WaitForSeconds(time);
        text.text = oldMessage;
    }
}
