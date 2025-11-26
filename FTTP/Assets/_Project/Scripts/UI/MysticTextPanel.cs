using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MysticTextPanel : MonoBehaviour
{
    public enum PanelTheme { Oracle, Vase }
    [Header("Theme")]
    [SerializeField] private PanelTheme theme = PanelTheme.Vase;

    [Header("Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI textComponent;

    private void Awake()
    {
        SetupPanel();
    }

    private void SetupPanel()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponentInChildren<Image>();
        if (textComponent == null)
            textComponent = GetComponentInChildren<TextMeshProUGUI>();

        if (theme == PanelTheme.Oracle)
        {
            backgroundImage.color = new Color(0.3f, 0.1f, 0.5f, 0.85f); // Purple with transparency
            textComponent.color = new Color(1f, 0.84f, 0f, 1f); // Gold
        }
        else // Vase
        {
            backgroundImage.color = new Color(1f, 0.84f, 0f, 0.75f); // Gold with transparency
            textComponent.color = new Color(0f, 0f, 0f, 1f); // Black
        }

        // Add blur/glow effect to background
        var shadow = backgroundImage.gameObject.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = backgroundImage.gameObject.AddComponent<Shadow>();
            shadow.effectDistance = new Vector2(5, -5);
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
        }

        // Text outline for improoved readability
        var outline = textComponent.GetComponent<Outline>();
        if (outline == null)
        {
            outline = textComponent.gameObject.AddComponent<Outline>();
            outline.effectDistance = new Vector2(2, -2);
            outline.effectColor = new Color(0, 0, 0, 0.5f);
        }
    }

    public void SetText(string text)
    {
        if (textComponent != null)
            textComponent.text = text;
    }
}
