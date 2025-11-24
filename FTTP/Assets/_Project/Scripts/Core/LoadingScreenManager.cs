using UnityEngine;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    public GameObject panel;
    public TMP_Text messageText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Show(string message)
    {
        messageText.text = message;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}

