using System;

/// <summary>
///     Keep track of details passed through event .
/// </summary>
public class AudioEventArgs : EventArgs
{
    /// <summary>
    ///     Name of the Audio Clip.
    /// </summary>
    public string NameOfTheAudioClip { get; set; }
}