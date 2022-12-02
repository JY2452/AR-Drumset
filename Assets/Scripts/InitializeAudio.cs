/*
 * Initializes audio recorder used to record drum tracks
 */

using UnityEngine;
using InGameAudioRecorder;

public class InitializeAudio : MonoBehaviour
{
    void Start()
    {
        AudioRecorder.fileName = "drumtrack";
        AudioRecorder.filePath = Application.streamingAssetsPath;
    }
}
