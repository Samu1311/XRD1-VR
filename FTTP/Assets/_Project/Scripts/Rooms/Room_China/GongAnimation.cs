using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GongAnimation : MonoBehaviour
{
    public float wobbleAmount = 0.05f;
    public float wobbleSpeed = 8f;

    private Vector3 originalScale;
    private bool wobbling = false;
    private float wobbleTime = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void PlayWobble(SelectEnterEventArgs args)
    {
        wobbling = true;
        wobbleTime = 0f;
    }

    void Update()
    {
        if (wobbling)
        {
            wobbleTime += Time.deltaTime * wobbleSpeed;
            float wobble = Mathf.Sin(wobbleTime) * wobbleAmount;

            transform.localScale = originalScale + new Vector3(0, wobble, 0);

            if (wobbleTime > Mathf.PI * 2f)
            {
                wobbling = false;
                transform.localScale = originalScale;
            }
        }
    }
}
