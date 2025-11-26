using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class GreekVase : MonoBehaviour
{
    [Header("Myth Selection")]
    [SerializeField] private GreekMythDatabase mythDatabase;
    [SerializeField] private int mythIndex = 0;

    [Header("Display Settings")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI mythTextUI;
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 1.5f, 0);

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    private GameObject activeTextPanel;
    private float hideTimer;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }

        interactable.selectEntered.AddListener(OnVaseClicked);
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnVaseClicked);
        }
    }

    private void Update()
    {
        if (activeTextPanel != null && hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
            {
                HideText();
            }
        }
    }

    private void OnVaseClicked(SelectEnterEventArgs args)
    {
        ShowText();
    }

    private void ShowText()
    {
        string mythText = mythDatabase != null ? mythDatabase.GetMythText(mythIndex) : "No myth database assigned.";

        if (textPanel != null)
        {
            if (activeTextPanel == null)
            {
                activeTextPanel = Instantiate(textPanel, transform.position + textOffset, Quaternion.identity);
                activeTextPanel.transform.SetParent(transform);
            }

            // Try to update the text by using the mystictextpanel first, fallback to direct textmeshpro if not found
            var mysticPanel = activeTextPanel.GetComponent<MysticTextPanel>();
            if (mysticPanel != null)
            {
                mysticPanel.SetText(mythText);
            }
            else if (mythTextUI != null)
            {
                mythTextUI.text = mythText;
            }
            else
            {
                // Get textmeshprogui from children if not assigned
                var textInChildren = activeTextPanel.GetComponentInChildren<TextMeshProUGUI>();
                if (textInChildren != null)
                {
                    textInChildren.text = mythText;
                }
            }

            activeTextPanel.SetActive(true);
            hideTimer = displayDuration;
        }
        else
        {
            Debug.Log($"Greek Vase: {mythText}");
        }
    }

    private void HideText()
    {
        if (activeTextPanel != null)
        {
            activeTextPanel.SetActive(false);
        }
    }
}
