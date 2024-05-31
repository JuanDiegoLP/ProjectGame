using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is missing from the GameObject");
            return;
        }

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
        else
        {
            Debug.LogError("Button component is missing from the GameObject");
        }
    }

    void PlaySound()
    {
        if (clickSound == null)
        {
            Debug.LogError("No click sound assigned");
            return;
        }

        audioSource.PlayOneShot(clickSound);
        Debug.Log("Playing sound: " + clickSound.name);
    }
}
