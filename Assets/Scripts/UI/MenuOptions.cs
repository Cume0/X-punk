using UnityEngine;
using UnityEngine.Audio;

public class MenuOptions : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void setVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
}
