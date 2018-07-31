using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Reference to the sound manager .
    public static SoundManager reference;

    //Audio clips
    public AudioClip TraceTheAlphabet;
    public AudioClip NowYouDoIt;
    public AudioClip PenDrawClip;
    public AudioClip TraceOnly;
    public AudioClip NowYouSayit;
    public AudioClip[] RewardAudio = new AudioClip[3];

    private bool IsAudioPlaying
    {
        get
        {
            if (gameObject.GetComponent<AudioSource>() != null)
                return gameObject.GetComponent<AudioSource>().isPlaying;
            return false;
        }
    }

    //Unity referenciate single tone 
    private void Awake()
    {
        if (reference == null)
            reference = this;
        else
            Destroy(this);
    }

    //play any audio clip 
    public void Play(AudioClip clipThatwillBeplayed, bool loop, bool isImmediate)
    {
        //if audio is not playing .
        //There is no question of playing audio immidiately .
        if (!IsAudioPlaying )
        {
            //check is there any audio listener attached 
            if (Camera.main.GetComponent<AudioListener>() != null)
            {
            }
            else
            {
                //Add a Audio listener 
                gameObject.AddComponent<AudioListener>();
            }

            //create A audio source At runtime 
            //It will be Autometically Attached with the Gameobject .
            //check if any AudioSource exist or not .
            if (gameObject.GetComponent<AudioSource>() == null)
            {
                //create Audio Source
                //AudioSource attachedaudioSource = new AudioSource ();
                //Attch the Audiosource 
                var attachedaudioSource = gameObject.AddComponent<AudioSource>();
                //Play on Awake 
                attachedaudioSource.playOnAwake = false;
                //Updated enabled 
                attachedaudioSource.enabled = true;
                //Attched clip
                attachedaudioSource.clip = clipThatwillBeplayed;
                //Play the sound
                attachedaudioSource.Play();

                //no loop 
                attachedaudioSource.loop = loop;
            }
            else
            {
                //no need to add audio source
                gameObject.GetComponent<AudioSource>().enabled = true;
                gameObject.GetComponent<AudioSource>().clip = clipThatwillBeplayed;
                gameObject.GetComponent<AudioSource>().Play();
                gameObject.GetComponent<AudioSource>().loop = loop;
            }
        }

        //When Audio is playing and you want to stop that and play something else .
        if (IsAudioPlaying && isImmediate)
        {
            gameObject.GetComponent<AudioSource>().enabled = true;
            gameObject.GetComponent<AudioSource>().clip = clipThatwillBeplayed;
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<AudioSource>().loop = loop;
        }
    }

    //play reward audio
    public void PlayRewardAudio()
    {
        //Genarate a random number 
        var num = Random.Range(0, 1000);
        //Make a modulo diision to make it less predictable 
        num = num % (RewardAudio.Length - 1);
        //Get the audio clip with array index .
        var getAudioClip = RewardAudio[num];

        //if audioclip is not null 
        if (getAudioClip != null)
        {
            //play it 
            Play(getAudioClip, false, false);
            PatchyAnimationController.ReferenceAnimationController.StartAnimation(
                PatchyAnimationController.TypeOfAnimation.Super, gameObject.GetComponent<AudioSource>().clip.length);
        }
        else
        {
            //play a audioclip from the array
            //prediction is low
            Play(RewardAudio[Random.Range(0, RewardAudio.Length - 1)], false, false);
            PatchyAnimationController.ReferenceAnimationController.StartAnimation(
                PatchyAnimationController.TypeOfAnimation.Super, gameObject.GetComponent<AudioSource>().clip.length);
        }
    }

    //when user draw that time sound will be played
    public void PlayDrawAudio()
    {
        Play(PenDrawClip, true, true);
    }

    public void StopDrawAudio()
    {
        //Get attched audio source
        var attachedaudioSource = gameObject.GetComponent<AudioSource>();
        //check null
        if (attachedaudioSource != null)
        {
            //Stop the audio clip 
            attachedaudioSource.Stop();
            //Remove the audio clip from the source
            attachedaudioSource.clip = null;
        }
    }
}