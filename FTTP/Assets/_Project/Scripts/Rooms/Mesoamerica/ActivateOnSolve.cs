using UnityEngine;

public class ActivateOnSolve : MonoBehaviour
{
    public GameObject target;

    public void Activate()
    {
        if (target != null)
            target.SetActive(true);
    }
}
