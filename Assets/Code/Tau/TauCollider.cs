using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TauCollider : MonoBehaviour
{
	public TauBody body;
	
	public void OnTriggerEnter(Collider other)
	{
		if (body.HandleCollide != null)
		{
			body.HandleCollide(other.gameObject);
		}
	}
}