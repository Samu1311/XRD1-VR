using UnityEngine;

public class SceneIntro : MonoBehaviour
{
    public SphinxTalk sphinxTalk; 

    void Start()
    {
        // Play the intro audio automatically when the scene loads
        if (sphinxTalk != null)
        {
            sphinxTalk.PlayIntro();
        }
    }
}
