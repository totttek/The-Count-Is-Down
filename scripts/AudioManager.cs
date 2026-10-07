using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	[Export] private AudioStream[] musicTracks;

	[Export] private int audioPlayersAmmount = 8;
	private readonly List<AudioStreamPlayer> audioPlayers = [];
	private AudioStreamPlayer musicPlayer;

	private readonly Random rng = new();

	public override void _Ready()
	{
		Instance = this;

		for(int i = 0; i < audioPlayersAmmount; i++)
		{
			var audioPlayer = new AudioStreamPlayer() { Bus = "SFX" };
			audioPlayers.Add(audioPlayer);
			AddChild(audioPlayer);
		}

		musicPlayer = new AudioStreamPlayer() { Bus = "Music" };
		AddChild(musicPlayer);
		musicPlayer.Finished += PlayRandomMusic;

		PlayRandomMusic();
	}

	public void PlaySound(AudioStream sound)
	{
		foreach (var audioPlayer in audioPlayers)
		{
			if (!audioPlayer.Playing)
			{
				audioPlayer.Stream = sound;
				audioPlayer.Play();
				return;
			}
		}

		GD.PrintErr("All audio players are currently in use. Consider increasing the number of audio players.");
	}

	public void PlayRandomMusic()
	{
		AudioStream randomTrack = musicTracks[rng.Next(0, musicTracks.Length)];
		musicPlayer.Stream = randomTrack;
		musicPlayer.Play();
	}
}
