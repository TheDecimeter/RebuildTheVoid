using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnEscape : MonoBehaviour
{
    public GameObject[] toShow;


    private void Start()
    {
        StartCoroutine(show(5));
    }

    private IEnumerator show(float sec)
    {
        foreach(GameObject g in toShow)
            g.SetActive(true);

        yield return new WaitForSeconds(sec);

        foreach (GameObject g in toShow)
            g.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(show(5));
        }
    }
}
