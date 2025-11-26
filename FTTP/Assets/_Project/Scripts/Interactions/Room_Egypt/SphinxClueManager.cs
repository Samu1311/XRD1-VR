using UnityEngine;

public class SphinxClueManager : MonoBehaviour
{
    private int collected = 0;

    public SphinxTalk sphinxTalk;

    public void ItemCollected(string itemName)
    {
        collected++;

        if (collected == 1)
            sphinxTalk.SayClue2();  // after first pickup

        else if (collected == 2)
            sphinxTalk.SayClue3();  // after second pickup

        else if (collected == 3)
            sphinxTalk.SayFinalMessage(); // all done
    }

    public void StartFirstRiddle()
    {
        sphinxTalk.SayClue1();
    }
}
