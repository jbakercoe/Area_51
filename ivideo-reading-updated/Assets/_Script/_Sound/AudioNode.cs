using UnityEngine;

/// <summary>
///     Node of the linked list
/// </summary>
public sealed class AudioNode
{
    #region Public Field

    public readonly AudioClip DataAudioClip;
    public AudioNode NextAudioNode;
    public string AudioNodeName;

    #endregion

    #region Constructor

    /// <summary>
    ///     Constructor Derived
    /// </summary>
    /// <param name="clip"></param>
    public AudioNode(AudioClip clip)
    {
        DataAudioClip = clip;
        NextAudioNode = null;
        AudioNodeName = clip.name;
    }

    /// <summary>
    ///     Default constructor
    /// </summary>
    public AudioNode()
    {
        DataAudioClip = null;
        NextAudioNode = null;
    }

    #endregion
}