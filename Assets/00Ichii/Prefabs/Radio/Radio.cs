using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : Interactable
{
    public AudioClip[] radioClips;
    public int ch = 0;
    AudioSource audioSource;
    public AudioSource switchSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRadio(ch);
    }

    public override void Interact()
    {
        if (player.GetComponent<Player>().InventoryContains(Item.ItemType.Arm))
        {
            ch = ++ch % radioClips.Length;
            PlayRadio(ch);
        }
        else
        {
            Debug.Log("You can't play radio!");
        }
    }

    private void PlayRadio(int ch)
    {
        audioSource.Stop();
        switchSource.PlayOneShot(switchSource.clip);

        AudioClip playClip = radioClips[ch];
        if(playClip != null)
        {
            audioSource.clip = playClip;
            audioSource.Play();
        }
        return;
    }
}
