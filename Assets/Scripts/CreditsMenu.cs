using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackButton();
            }
        }
    }

    public void OpenCredits()
    {
        foreach (Transform b in transform.parent)
        {
            if (b.gameObject.tag == "button")
            {
                b.gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void BackButton()
    {
        foreach (Transform b in transform.parent)
        {
            if (b.gameObject.tag == "button")
            {
                b.gameObject.SetActive(true);
            }
        }
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}

