using UnityEngine;

public class SphinxTalk : MonoBehaviour
{
    public AudioSource audioSource;

    [TextArea] public string clue1Text;
    [TextArea] public string clue2Text;
    [TextArea] public string clue3Text;
    [TextArea] public string finalText;

    public TTSGenerator tts;  // Your text-to-speech component

    public void SayClue1() => Speak(clue1Text);

    public void SayClue2() => Speak(clue2Text);

    public void SayClue3() => Speak(clue3Text);

    public void SayFinalMessage() => Speak(finalText);

    void Speak(string text)
    {
        tts.GenerateAndPlay(text, audioSource);
    }
}
