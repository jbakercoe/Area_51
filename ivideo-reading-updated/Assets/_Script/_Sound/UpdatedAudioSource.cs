using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;


/// <summary>
///     Updated Audio Source .
///     Updated Audio Source is constructed with init method.
///     Updated Audio source attach a audio source in a desire gameobject.
///     Or in the same game object the class ,
///     attached with .
/// </summary>
public class UpdatedAudioSource : MonoBehaviour
{
    #region for public field

    /// <summary>
    ///     Audio Source Delegate Handeler.
    /// </summary>
    public delegate void AudioSourceEventHandeler(Object publisher, AudioEventArgs detailsArgs);

    /// <summary>
    ///     Static event that informed subscriber that audio has been completed.
    /// </summary>
    public static event AudioSourceEventHandeler AudioHasBeenPlayed;

    /// <summary>
    ///     Static reference of the class .
    /// </summary>
    public static UpdatedAudioSource Reference;

    /// <summary>
    /// Audio mixer
    /// </summary>
    [Space(10f)] [Header("Clint's Audio Mixer")]
    public AudioMixer Mixer;

    /// <summary>
    /// Out of Audio Mixer.
    /// </summary>
    public enum OutPutOfAudioMixer : int
    {
        Ambience = 0,
        Argentinosaurus = 1,
        Footsteps = 2,
        Master = 3,
        Music = 4,
        Pachy = 5,
        SoundFX = 6,
        Therizinosaurus = 7,
        Triceratops = 8,
        VoiceOver = 9
    }

    /// <summary>
    /// Current output.
    /// </summary>
    [Space(10f)] [Header("Clint's Audio Mixer Current Out Put")]
    public OutPutOfAudioMixer CurrentOutput;

    #endregion

    #region Public property

    /// <summary>
    ///     Audio source connected.
    /// </summary>
    private AudioSource AudioSource { get; set; }

    /// <summary>
    ///     Is Audio playing .
    /// </summary>
    public bool IsAudioPlaying => AudioSource.isPlaying;

    /// <summary>
    ///     Return the Audio clip attached with .
    /// </summary>
    public AudioClip AttachedAudioClip
    {
        private get { return AudioSource.clip; }
        set { AudioSource.clip = value; }
    }

    #endregion

    #region Unity Functions

    /// <summary>
    ///     Awake the instatnce.
    /// </summary>
    private void Awake()
    {
        Reference = this;
        Debug.Log(Reference.name);
        Init();
    }

    #endregion

    #region Normal functions

    /// <summary>
    ///     initialize the Updated Audio Source.
    /// </summary>
    private void Init()
    {
        AudioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    ///     Audio source setting.
    ///     No loop , No play on Awake .
    /// </summary>
    public void AudioSourceSetting()
    {
        AudioSource.loop = false;
        AudioSource.playOnAwake = false;
        //Use Audio mixer group.
        var outputMixer = CurrentOutput;
        //set group.
        AudioSource.outputAudioMixerGroup = Mixer.FindMatchingGroups(outputMixer.ToString())[0];
    }

    /// <summary>
    ///     Play the Audio Source.
    /// </summary>
    public void Play()
    {
        AudioSource.Play();
        AudioStarted();
    }

    /// <summary>
    ///     Stop the Audio .
    ///     Remove the sound track.
    /// </summary>
    public void Stop()
    {
        //stop playing.
        AudioSource.Stop();
        //stop follow up coroutine.
        StopAllCoroutines();
        //make audio clip null .
        AttachedAudioClip = null;
    }

    /// <summary>
    /// Pauses the Audio.
    /// </summary>
    public void PauseAudio()
    {
        AudioSource.Pause();
    }
    /// <summary>
    /// Resumes the audio.
    /// </summary>
    public void ResumeAudio()
    {
        AudioSource.UnPause();
    }
    /// <summary>
    ///     Get Length Of the Audio clip .
    /// </summary>
    /// <returns></returns>
    public float CurrentAudioClipLength()
    {
        return AudioSource.clip.length;
    }

    /// <summary>
    ///     Attach the Audio Clip.
    /// </summary>
    /// <param name="clip"></param>
    public void AttachClip(AudioClip clip)
    {
        AttachedAudioClip = clip;
    }

    /// <summary>
    ///     Remove the Audio clip from the Audio Source .
    /// </summary>
    /// <returns></returns>
    public void DeAttachAudioClip(string clipname)
    {
        if (clipname.Contains(AttachedAudioClip.name)) AttachedAudioClip = null;
    }

    /// <summary>
    ///     Co routine keep track of Audio start .
    /// </summary>
    private void AudioStarted()
    {
        var audioStartEnumerator = AudioTrackCoroutine();
        StartCoroutine(audioStartEnumerator);
    }

    private IEnumerator AudioTrackCoroutine()
    {
        while (IsAudioPlaying) yield return null;
        //TODO:next line error.
        Debug.Log("Audio playing is compleet for clip : " + AttachedAudioClip.name);
        var eventargs = new AudioEventArgs {NameOfTheAudioClip = AttachedAudioClip.name};
        OnAudioCompleted(this, eventargs);
    }

    /// <summary>
    ///     On Audio Complete.
    ///     Connected to event Audio Played .
    /// </summary>
    private void OnAudioCompleted(Object publisher, AudioEventArgs details)
    {
        AudioHasBeenPlayed?.Invoke(publisher, details);
    }

    #endregion
}