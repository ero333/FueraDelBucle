using UnityEngine;

public class mudoefectos : MonoBehaviour
{
    [Header("Audio Sources to Mute")]
    [Tooltip("Drag the GameObjects with AudioSources into this list.")]
    public AudioSource[] audioSources;

    // Keeps track of whether the audio is currently muted
    private bool isMuted = false;

    // The UI Button will call this method
    public void ToggleMute()
    {
        // Flip the state (if it was false, make it true, and vice versa)
        isMuted = !isMuted;

        // Apply the mute state to every AudioSource in the list
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
            {
                source.mute = isMuted;
            }
        }

        Debug.Log(isMuted ? "Audio Muted" : "Audio Unmuted");
    }
}