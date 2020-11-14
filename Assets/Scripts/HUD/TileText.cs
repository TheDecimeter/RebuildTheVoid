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
        StopAllCoroutines();
        text.text = message;
        //print("setting message " + message);
    }

    public void Show(string message, float time)
    {
        //print("setting temp message " + message);
        StopAllCoroutines();
        StartCoroutine(ShowTempMessageCoroutine(message, time));
    }

    private IEnumerator ShowTempMessageCoroutine(string message, float time)
    {
        string oldMessage = text.text;
        text.text = message;
        yield return new WaitForSeconds(time);
        text.text = oldMessage;

        //print("resetting message " + oldMessage);
    }
}
