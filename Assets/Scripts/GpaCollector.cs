using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GpaCollector : MonoBehaviour
{
	public float gpa = 0.0f;
	[SerializeField] private Text gpaText;
	[SerializeField] private AudioSource collectSound;

	
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("GPA"))
		{
			collectSound.Play();
			Destroy(collision.gameObject);
			gpa = gpa + 0.1f;
			gpaText.text = "GPA : "+gpa;
		}	
	}
	
	
}
