using System;
using Level6;
using UnityEngine;
using UpdatedAudioSource = Level6.UpdatedAudioSource;

/// <summary>
///     Audio player class maintain a qeue to play the multiple  audio.
///     Qeue is maintained with the help of Linked List.
/// </summary>
public sealed class AudioPlayer
{
    //Audio node keep track of your Start Node.
    private AudioNode _startAudionode;

    //Check the AudioPlayer have completed it's all songs or yet to finish.
    public bool IsPlaylistPlaying;

    public AudioPlayer()
    {
        //Initial stage make it null .
        _startAudionode = null;
        //Connect to the event
        global::UpdatedAudioSource.AudioHasBeenPlayed += PlaySongOverList;
        //Default value is true .
        IsPlaylistPlaying = false;
    }

    /// <summary>
    ///     Create An Audio Player List.
    ///     Parameter Sequence is important .
    ///     Audio Clip will be played In same Sequence .
    /// </summary>
    public void CreatePlayList(params AudioClip[] clips)
    {
        foreach (var item in clips) AddSong(item);
    }

    /// <summary>
    ///     Delete the qeue .
    /// </summary>
    private void DeletePlaylist()
    {
        //just release the reference .
        _startAudionode = null;
    }

    /// <summary>
    ///     stop the play list .
    /// </summary>
    public void StopPlayingList()
    {
        //if start audio node is  empty
        if (_startAudionode == null) return;
        //check the song is playing or not .
        if (global::UpdatedAudioSource.Reference.IsAudioPlaying)
        {
            //if playing then stop it
            global::UpdatedAudioSource.Reference.Stop();
        }
        else
        {
            //empty the node
            DeletePlaylist();
            return;
        }

        //empty the node .
        DeletePlaylist();
    }

    /// <summary>
    /// Pauses the audio.
    /// </summary>
    /// <param name="pause">if set to <c>true</c> [pause].</param>
    public void  PauseAudio(bool pause)
    {
        if(pause)
            global::UpdatedAudioSource.Reference.PauseAudio();
        else
        {
            global::UpdatedAudioSource.Reference.ResumeAudio();
        }
    }

    /// <summary>
    ///     Add Node At the End of the list
    /// </summary>
    private void AddSong(AudioClip clip)
    {
        //Take a pointer.
        //Assign pointer rear to start node .
        var rear = _startAudionode;
        //Check start Node 
        if (_startAudionode == null)
        {
            //create a node
            var node = new AudioNode(clip);

            //Point start node to new node .
            _startAudionode = node;
            Debug.Log("First Song added Song name " + node.AudioNodeName);
        }
        else
        {
            //Iterate to the last node .
            while (rear.NextAudioNode != null) rear = rear.NextAudioNode;

            //create the node .
            var node = new AudioNode(clip);
            //pointer p now connecting the node .
            rear.NextAudioNode = node;
            Debug.Log(" Song added at the end " + node.AudioNodeName);
        }

        //Print Qeue
        //IterateOverNodes();
    }

    /// <summary>
    ///     Node will be removed only from the begining
    /// </summary>
    private void RemoveSong()
    {
        //If start node is null
        if (_startAudionode == null) throw new OperationCanceledException("No node is here to remove");

        //Take a pointer.
        //Assign pointer front to start node .
        var front = _startAudionode;
        //Get the reference of Next to Next Node .
        Debug.Log("Start node currently pointing to before deletion : " + _startAudionode.AudioNodeName);
        if (front.NextAudioNode != null)
        {
            _startAudionode = front.NextAudioNode;
            Debug.Log("Start node currently pointing to after deletion : " + _startAudionode.AudioNodeName);
        }
        else
        {
            _startAudionode = null;
            Debug.Log("Empty Play list");
            IsPlaylistPlaying = false;
        }

        //print Qeue
        //IterateOverNodes();
    }

    /// <summary>
    ///     Play the song.
    /// </summary>
    public void StartPlayingList()
    {
        //play list just started
        IsPlaylistPlaying = true;
        //if there is something to play .
        if (_startAudionode == null) return;
        //When Audio is not playing.
        if (global::UpdatedAudioSource.Reference.IsAudioPlaying) return;
        //Attch the clip.
        global::UpdatedAudioSource.Reference.AttachedAudioClip = _startAudionode.DataAudioClip;
        //play the clip.
        global::UpdatedAudioSource.Reference.Play();
        //return.
    }

    /// <summary>
    ///     Play song of the play list After first song is completed .
    /// </summary>
    private void PlaySongOverList(object source, AudioEventArgs audioEventArgs)
    {
        //Audio clip currently attached and just played must be same
        if (audioEventArgs.NameOfTheAudioClip != _startAudionode.DataAudioClip.name) return;
        //De attach clip.
        global::UpdatedAudioSource.Reference.DeAttachAudioClip(_startAudionode.DataAudioClip.name);
        //Delete first node
        RemoveSong();
        //if there is nothing to play .
        if (_startAudionode == null) return;
        //When Audio is  playing.
        if (global::UpdatedAudioSource.Reference.IsAudioPlaying) return;
        //Attch the clip.
        global::UpdatedAudioSource.Reference.AttachedAudioClip = _startAudionode.DataAudioClip;
        //play the clip.
        global::UpdatedAudioSource.Reference.Play();
    }

    /// <summary>
    ///     Ussed for debuging.
    /// </summary>
    private void IterateOverNodes()
    {
        var tracker = _startAudionode;
        Debug.Log("\n\n\n");
        if (tracker == null)
        {
            Debug.Log("<color=red> No Node present </color>");
            return;
        }

        while (tracker != null)
        {
            Debug.Log("<color=red>Node Name :</color> " + tracker.AudioNodeName);
            tracker = tracker.NextAudioNode;
        }

        Debug.Log("\n\n\n");
    }
}