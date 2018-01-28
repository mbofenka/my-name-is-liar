﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicTracks {None, MenuTheme, Overworld, DDR}

public class MusicManager : MonoBehaviour {

	public static MusicManager instance;

	[SerializeField]
	private AudioClip MenuTheme, Overworld, DDR;

	private AudioSource AS;

	// Use this for initialization
	void Start () {
		AS = GetComponent<AudioSource> ();

		if (instance) {
			if (AS.clip) {
				instance.AS.clip = AS.clip;
				if (!instance.AS.isPlaying && AS.playOnAwake) {
					instance.AS.Play ();
				}
			}
			if (!AS.playOnAwake) {
				instance.AS.Stop ();
			}
			instance.AS.loop = AS.loop;
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (this);
		}
	}

	public void SwitchMusic(MusicTracks newTrack) {
		if (newTrack == MusicTracks.None) {
			AS.Stop ();
		} else if (newTrack == MusicTracks.MenuTheme) {
			AS.clip = MenuTheme;
			AS.Play ();
		} else if (newTrack == MusicTracks.Overworld) {
			AS.clip = Overworld;
			AS.Play ();
		} else if (newTrack == MusicTracks.DDR) {
			AS.clip = DDR;
			AS.Play ();
		}
	}
}