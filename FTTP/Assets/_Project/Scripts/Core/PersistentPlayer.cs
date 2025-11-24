using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentPlayer : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
