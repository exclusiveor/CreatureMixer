using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
	private const string SOUNDS_DIRECTORY = "Sounds/";
	private const string TRACKS_DIRECTORY = "music/";

	public static float				CROSSFADE_TIME = 1.0f;
	// The music player game object
	private static GameObject 		curr_me = null;				// current played music game object
	private static MusicPlayer 		curr_mp = null;				// current played music player
	private static GameObject 		free_me = null;				// free music game object
	private static MusicPlayer 		free_mp = null;				// free music player

	//
	private static string			lastGameTrack;
	private static bool				isGameScene;
	private static List<string>		AvailableGameTracks;

	// sound
	private static AudioSource 		SoundSource = null;			// free music game object

	private static Dictionary<string, GameObject> SoundsPlayed;
	private static Dictionary<string, AudioClip> Sounds;
	private static Dictionary<string, AudioClip> Tracks;

	
	// Retreive or create the current music emitter
	public static GameObject getCurrentMusicEmitter()
	{
		if(curr_me == null)
		{
			curr_me = new GameObject();
			curr_me.name = "Current Music Emitter";
			curr_me.AddComponent<AudioSource>();
			curr_me.GetComponent<AudioSource>().loop = true;
			DontDestroyOnLoad(curr_me);
			curr_mp = curr_me.AddComponent<MusicPlayer>();

			curr_mp.transform.parent = GameManager.Instance.transform;
		}
		return curr_me;
	}
	
	// Retreive or create the current music player component of the emitter
	public static MusicPlayer getCurrentMusicPlayer()
	{
		if(curr_mp == null)
		{
			curr_mp = getCurrentMusicEmitter().GetComponent<MusicPlayer>();
			if(curr_mp == null)
			{
				curr_mp = curr_me.AddComponent<MusicPlayer>();
			}
		}
		return curr_mp;
	}

	// Retreive or create the free music emitter
	public static GameObject getFreeMusicEmitter()
	{
		if(free_me == null)
		{
			free_me = new GameObject();
			free_me.name = "Free Music Emitter";
			free_me.AddComponent<AudioSource>();
			free_me.GetComponent<AudioSource>().loop = true;
			DontDestroyOnLoad(free_me);
			free_mp = free_me.AddComponent<MusicPlayer>();

			free_me.transform.parent = GameManager.Instance.transform;
		}
		return free_me;
	}
	
	// Retreive or create the free music player component of the emitter
	public static MusicPlayer getFreeMusicPlayer()
	{
		if(free_mp == null)
		{
			free_mp = getFreeMusicEmitter().GetComponent<MusicPlayer>();
			if(free_mp == null)
			{
				free_mp = free_me.AddComponent<MusicPlayer>();
			}
		}
		return free_mp;
	}
	
	// Play a music
	public static void play(AudioClip clip, float fadeOut = 0f, float fadeIn = 0f)
	{
		getCurrentMusicPlayer().playMusic(clip, fadeOut, fadeIn);
	}

	// Play a music with a filename in a Resources folder
	public static void play(string name, float fadeOut = 0f, float fadeIn = 0f)
	{
		AudioClip a = null;
		Tracks.TryGetValue(name, out a);
		if (a == null)
		{
			a = (AudioClip)Resources.Load(TRACKS_DIRECTORY + name, typeof(AudioClip));
		}
		if(a != null){
			play(a, fadeOut, fadeIn);
		}
		else
		{
			Debug.Log("Could not find music \""+name+"\" in Resources folder.");
		}
	}

	// Play a music crossfade
	public static float playCrossfade(AudioClip clip, float fadeOut = 0f, float fadeIn = 0f)
	{
		getCurrentMusicPlayer().stopMusic(fadeOut);
		getFreeMusicPlayer().playMusic(clip, 0, fadeIn);

		GameObject temp_me = curr_me;
		MusicPlayer temp_mp = curr_mp;
		curr_me = free_me;
		curr_mp = free_mp;
		free_me = temp_me;
		free_mp = temp_mp;
		return clip.length;
	}
	
	// Play a music with a filename in a Resources folder
	public static float playCrossfade(string name, float fadeOut = 0f, float fadeIn = 0f)
	{
		AudioClip a = null;
		Tracks.TryGetValue(name, out a);
		if (a == null)
		{
			a = (AudioClip)Resources.Load(TRACKS_DIRECTORY + name, typeof(AudioClip));
		}
		if(a != null){
			playCrossfade(a, fadeOut, fadeIn);
		}
		else
		{
			Debug.Log("Could not find music \""+name+"\" in Resources folder.");
		}
		return a.length;
	}
	
	// Set if the music should loop or not
	public static void setLoop(bool t)
	{
		getCurrentMusicEmitter().GetComponent<AudioSource>().loop = t;
		getFreeMusicEmitter().GetComponent<AudioSource>().loop = t;
	}
	
	// Pause the music
	public static void pause()
	{
		getCurrentMusicPlayer().pauseMusic();
		getFreeMusicPlayer().pauseMusic();
	}
	
	// Unpause the music
	public static void unpause()
	{
		getCurrentMusicPlayer().unpauseMusic();
		getFreeMusicPlayer().unpauseMusic();
	}

	// Stop the music
	public static void stop(float fadeOut = 0f)
	{
		getCurrentMusicPlayer().stopMusic(fadeOut);
		getFreeMusicPlayer().stopMusic(fadeOut);
	}

	// Set the volume of the music
	public static void setMusicVolume(float volume = 1.0f, float duration = 0f)
	{
		getCurrentMusicPlayer().setMusicVolume(volume, duration);
		getFreeMusicPlayer().setMusicVolume(volume, duration);
	}

	// EXTENSION
	public static void InitMusicManager()
	{
		lastGameTrack = "";
		setLoop(true);
		setMusicVolume(0.5f);
		AvailableGameTracks = new List<string>();
		SoundsPlayed = new Dictionary<string, GameObject>();
		isGameScene = true;
		LoadSounds();
		LoadTracks();
	}

	private static void LoadSounds()
	{
		//		// load only some clips
		//		SingleSoundsNeeded = new Dictionary<string, bool>();
		//		List<string> soundsToLoad = new List<string>()
		//		{
		//			"DemoSound1", 
		//			"DemoSound2",
		//			"DemoSound3"
		//		};
		//		Sounds = new Dictionary<string, AudioClip>();
		//		for (int i = 0; i < soundsToLoad.Count; ++i)
		//		{
		//			AudioClip clip = (AudioClip)Resources.Load(MusicManager.SOUNDS_DIRECTORY + soundsToLoad[i], typeof(AudioClip));
		//			Debug.Log ("             " + clip.name);
		//			Sounds.Add(soundsToLoad[i], clip);
		//		}
		
		// load all sounds in folder
		Sounds = new Dictionary<string, AudioClip>();
		AudioClip[] sounds = Resources.LoadAll<AudioClip>(SOUNDS_DIRECTORY);
		for (int i = 0; i < sounds.Length; ++i)
		{
			Sounds.Add(sounds[i].name, sounds[i]);
		}
	}

	private static void LoadTracks()
	{
		// load all tracks in folder
		Tracks = new Dictionary<string, AudioClip>();
/*		AudioClip[] sounds = Resources.LoadAll<AudioClip>(TRACKS_DIRECTORY);
		for (int i = 0; i < sounds.Length; ++i)
		{
			Tracks.Add(sounds[i].name, sounds[i]);
			//Debug.Log(sounds[i].name);
		}
*/	}

	public static void PlayTrack(string name, float crossfade)
	{
		isGameScene = true;
		LeanTween.cancel(curr_me);
		LeanTween.cancel(free_me);
		playCrossfade(name, crossfade, crossfade);
	}

	public static void PlayMainMenuTrack()
	{
		if (isGameScene)
		{
            isGameScene = false;
			LeanTween.cancel(curr_me);
			LeanTween.cancel(free_me);
			playCrossfade("MainMenuTrack", CROSSFADE_TIME, CROSSFADE_TIME);
		}
	}

	public static void PlayGameTracks()
	{
		lastGameTrack = GameManager.Instance.Settings.User.LastGameTrack;
		isGameScene = true;
		AvailableGameTracks.Clear();
		LeanTween.cancel(curr_me);
		for (int i = 1; i <= 1; ++i)
		{
			AvailableGameTracks.Add("GameTrack_" + i.ToString());
		}
		PlayNextGameTrack();
		GameManager.Instance.Settings.User.LastGameTrack = lastGameTrack;
	}

	private static void PlayNextGameTrack()
	{
		if (!isGameScene)
		{
			return;
		}
		LeanTween.cancel(curr_me);
		string prev = lastGameTrack;
        if (AvailableGameTracks.Count > 1)
        {
            if (prev != "") { AvailableGameTracks.Remove(prev); }
            lastGameTrack = AvailableGameTracks[UnityEngine.Random.Range(0, AvailableGameTracks.Count)];
            if (prev != "") { AvailableGameTracks.Add(prev); }
        }
        else
        {
            lastGameTrack = AvailableGameTracks[0];
        }
		
		float clipLength = playCrossfade(lastGameTrack, CROSSFADE_TIME, CROSSFADE_TIME);

        ////>float adelay = clipLength * UnityEngine.Random.Range(2,4) - CROSSFADE_TIME; // if each track plays 2-3 times CROSSFADE_TIME seconds for crossfade
        float adelay = clipLength - CROSSFADE_TIME;

        //float adelay = clipLength / 10 - 2.0f; // TEST

        LeanTween.delayedCall(curr_me, adelay, PlayNextGameTrack);
	}

	///// SOUNDS
	public static AudioSource getSoundSource()
	{
		if(SoundSource == null)
		{
			GameObject obj = new GameObject();
			obj.name = "Sound Emitter";
			SoundSource = obj.AddComponent<AudioSource>();
			SoundSource.loop = false;
			SoundSource.spatialBlend = 0;
			SoundSource.bypassEffects = false;
			SoundSource.bypassListenerEffects = false;
			SoundSource.dopplerLevel = 0;
			//SoundSource.ignoreListenerVolume = true;
			DontDestroyOnLoad(obj);

			obj.transform.parent = GameManager.Instance.transform;
		}
		return SoundSource;
	}

	public static void setSoundVolume(float volume = 1.0f)
	{
		getSoundSource().GetComponent<AudioSource>().volume = volume;
	}

	public static void playSound(AudioClip clip)
	{
		string name = clip.name;
		OnSoundEnded(name);
		getSoundSource().GetComponent<AudioSource>().PlayOneShot(clip);
		AddSoundToPlayed(name, clip.length);
	}

	public static void playSoundByPath(string name)
	{
		OnSoundEnded(name);
		AudioClip clip = (AudioClip)Resources.Load(name, typeof(AudioClip));
		getSoundSource().GetComponent<AudioSource>().PlayOneShot(clip);
		AddSoundToPlayed(name, clip.length);
	}

	public static AudioClip GetSound(string name)
	{
		return Sounds[name];
	}

	public static void playSound(string name)
	{
        //Debug.Log(name);
        playSound(Sounds[name]);
	}

	public static float GetSoundLength(string name)
	{
		return Sounds[name].length;
	}

	public static void StopAllSounds()
	{
		foreach (var s in SoundsPlayed)
		{
			LeanTween.cancel(s.Value);
			GameObject.Destroy(s.Value);
		}
		SoundsPlayed.Clear();
		SoundSource.Stop();
	}

	public static void playSingleSound(string name)
	{
		if (!SoundsPlayed.ContainsKey(name) || SoundsPlayed[name] == null)
		{
			AudioClip clip = Sounds[name];
			OnSoundEnded(name);
			AddSoundToPlayed(name, clip.length);
			getSoundSource().GetComponent<AudioSource>().PlayOneShot(clip);
		}
	}    
	
	public static void StopAll()
	{
		stop();
		SoundSource.Stop();
	}

    public static bool isSoundPlaying(string name)
    {
        return SoundsPlayed.ContainsKey(name);
    }

	private static void AddSoundToPlayed(string name, float delay)
	{
		GameObject obj = new GameObject();
		obj.name = "SOUND_" + name;
		SoundsPlayed.Add(name, obj);
        LeanTween.delayedCall(obj, delay, () => { OnSoundEnded(name); });
	}

	private static void OnSoundEnded(string name)
	{
		if (SoundsPlayed.ContainsKey(name))
		{
			if (SoundsPlayed[name] != null)
			{
				LeanTween.cancel(SoundsPlayed[name]);
			}
            GameObject.Destroy(SoundsPlayed[name]);
			SoundsPlayed.Remove(name);
		}
	}

    public static AudioClip getSoundClip(string clipName)
    {
        if (Sounds.ContainsKey(clipName))
        {
            return Sounds[clipName];
        }

        return null;
    }    
}
