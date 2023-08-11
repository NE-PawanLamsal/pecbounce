using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
   public AudioMixer audioMixture;
	
   public void SetVolume(float volume)
   {
	   audioMixture.SetFloat("volume",volume);
   }
  
}
