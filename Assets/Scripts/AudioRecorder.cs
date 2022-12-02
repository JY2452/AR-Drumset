/*
 * This is a script derived from the In-Game Audio Recorder available on the Unity Asset Store 
 * Link: https://assetstore.unity.com/packages/tools/audio/in-game-audio-recorder-228338
 * 
 * I edited the StartRecording(), StopRecording(), and CreateAudioFile() functions to
 * deal with updating the UI of the application & also so that the audio file would correctly
 * save to the proper location on an Android Device (previously it was set up to work just on PC)
 */

using System;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGameAudioRecorder
{
	[HelpURL("https://assetstore.unity.com/packages/slug/228338")]
	[RequireComponent(typeof(AudioListener))]
	public class AudioRecorder : MonoBehaviour
	{
		public static AudioRecorder Instance { get; private set; }

		public static string filePath, fileName;
		public static bool IsRecording { get; private set; }
		public static bool IsSaveAudioOnApplicationQuit = true;
		public const int OutputSampleRate = 48000;

		// UI Variables:
		public Button playButton;
		public Button stopButton;
		public TextMeshProUGUI successText;

		private const int headerSize = 44;

		private FileStream fileStream;

		/// <summary>
		/// Starts recording audio
		/// </summary>
		public void StartRecording()
		{
			if (IsRecording)
			{
				Debug.LogError("The recording cannot be started because it is already running");
				return;
			}
			IsRecording = true;
			if (string.IsNullOrEmpty(filePath))
            {
				Debug.LogError("You have not chosen a place to save the file!");
				return;
            }
			if (string.IsNullOrEmpty(fileName))
			{
				Debug.LogError("You didn't choose a file name!");
				return;
			}
			CreateAudioFile();

			// update state of UI components
			playButton.gameObject.SetActive(false);
			stopButton.gameObject.SetActive(true);
			successText.gameObject.SetActive(false);

			Debug.Log("Recording started");
		}

		/// <summary>
		/// Stops and saves recorded audio
		/// </summary>
		public void StopRecording()
		{
			if (!IsRecording)
            {
				Debug.LogError("The recording cannot be stopped because it has not started yet");
				return;
            }
			IsRecording = false;

			// update state of UI components
			playButton.gameObject.SetActive(true);
			stopButton.gameObject.SetActive(false);
			successText.gameObject.SetActive(true);

			SaveAudioFile();
			Debug.Log("Recording stopped. The file is saved in the directory \"" + filePath + "\"");
		}

		/// <summary>
		/// Creates an audio file at the specified path and fills it with empty bytes for now
		/// </summary>
		private void CreateAudioFile()
        {
			// save as wav file
			fileName = fileName + ".wav";

			// access file paths on android device
			var filepath = Path.Combine(Application.persistentDataPath, fileName);
			var dirName = Path.GetDirectoryName(filepath);

			if (!Directory.Exists(dirName))
				Directory.CreateDirectory(dirName);
			else if (File.Exists(filepath))
				File.Delete(filepath);
			fileStream = new FileStream(filepath, FileMode.Create);
			for (int i = 0; i < headerSize; i++)
				fileStream.WriteByte(new byte());
		}

		/// <summary>
		/// The algorithm for saving audio in the WAVE format
		/// </summary>
		private void SaveAudioFile()
		{
			fileStream.Seek(0, SeekOrigin.Begin);
			fileStream.Write(Encoding.UTF8.GetBytes("RIFF"), 0, 4);
			fileStream.Write(BitConverter.GetBytes(fileStream.Length - 8), 0, 4);
			fileStream.Write(Encoding.UTF8.GetBytes("WAVE"), 0, 4);
			fileStream.Write(Encoding.UTF8.GetBytes("fmt "), 0, 4);
			fileStream.Write(BitConverter.GetBytes(16), 0, 4);
			fileStream.Write(BitConverter.GetBytes(1), 0, 2);
			fileStream.Write(BitConverter.GetBytes(2), 0, 2);
			fileStream.Write(BitConverter.GetBytes(OutputSampleRate), 0, 4);
			fileStream.Write(BitConverter.GetBytes(OutputSampleRate * 4), 0, 4);
			fileStream.Write(BitConverter.GetBytes(4), 0, 2);
			fileStream.Write(BitConverter.GetBytes(16), 0, 2);
			fileStream.Write(Encoding.UTF8.GetBytes("data"), 0, 4);
			fileStream.Write(BitConverter.GetBytes(fileStream.Length - headerSize), 0, 4);
			fileStream.Close();
		}

		/// <summary>
		/// This is called by unity when bunch of audio samples gets accumulates
		/// </summary>
		/// <param name="data"></param>
		/// <param name="channels"></param>
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (!IsRecording)
				return;
			byte[] bytesData = new byte[data.Length * 2];
			for (int i = 0; i < data.Length; i++)
				BitConverter.GetBytes((short)(data[i] * 32767)).CopyTo(bytesData, i * 2);
			fileStream.Write(bytesData, 0, bytesData.Length);
		}

		/// <summary>
		/// Automatic file saving when exiting the application
		/// </summary>
		private void OnApplicationQuit()
        {
			if (!IsSaveAudioOnApplicationQuit || !IsRecording)
				return;
			StopRecording();
        }

		/// <summary>
		/// Preparing for this instance to work
		/// </summary>
		private void Start()
		{
			if (Instance == null)
				Instance = this;
			else
				Destroy(Instance.gameObject);
			AudioSettings.outputSampleRate = OutputSampleRate;

			// play sounds that are marked as PlayOnAwake
			AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
			for (int i = 0; i < allAudioSources.Length; i++)
				if (allAudioSources[i].playOnAwake)
					allAudioSources[i].Play();
		}
	}
}