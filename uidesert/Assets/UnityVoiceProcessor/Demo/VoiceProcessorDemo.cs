//
// Copyright 2021-2023 Picovoice Inc.
//
// You may not use this file except in compliance with the license. A copy of the license is located in the "LICENSE"
// file accompanying this source.
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
// an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
//
/*
using System.Collections.Generic;
using UnityEngine;

using Pv.Unity;

using System.Collections; 
using System;
using System.IO;// ✅ Required for Unity coroutines
using Google.Cloud.Speech.V1;


public class VoiceProcessorDemo : MonoBehaviour
{
    
    readonly int FrameLength = 512;
    readonly int SampleRate = 16000;

    private bool _dumpAudio = true;
    private bool _wasPositive = false; // Track if scale was positive before
    private Coroutine _saveCoroutine = null; // Store coroutine reference
    private List<short[]> _audioData = new List<short[]>();

    private SpeechClient speechClient;
    //private string googleCredentialsPath = Application.dataPath + "/StreamingAssets/google_credentials.json";
    

    void Start()
    {
        Debug.Log("Available Devices: " + string.Join(",", VoiceProcessor.Instance.Devices.ToArray()));

        VoiceProcessor.Instance.AddFrameListener(_onFrameCaptured);

        // Initialize Google Cloud Speech client
        /*speechClient = new SpeechClientBuilder
        {
            CredentialsPath = googleCredentialsPath
        }.Build();*/
        // Set the environment variable for Google credentials
   /*     string credentialsPath = Path.Combine(Application.streamingAssetsPath, "google_credentials.json");
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        Debug.Log($"Google credentials set from: {credentialsPath}");

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (VoiceProcessor.Instance.IsRecording)
            {

                VoiceProcessor.Instance.StopRecording();
            }
            else
            {
                if (_dumpAudio)
                {
                    _audioData.Clear();
                }
                VoiceProcessor.Instance.StartRecording(FrameLength, SampleRate);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            VoiceProcessor.Instance.ChangeDevice(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            VoiceProcessor.Instance.ChangeDevice(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            VoiceProcessor.Instance.ChangeDevice(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            VoiceProcessor.Instance.ChangeDevice(3);
        }
    }

    private void _onFrameCaptured(short[] frame)
    {
        if (_dumpAudio)
        {
            _audioData.Add(frame);
        }

        float rmsSum = 0;
        for (int i = 0; i < frame.Length; i++)
        {
            rmsSum += Mathf.Pow(frame[i], 2);
        }
        float rms = Mathf.Sqrt(rmsSum / frame.Length);

        float dBFS = 20 * Mathf.Log10(rms);
        if (float.IsInfinity(dBFS) || float.IsNaN(dBFS))
        {
            return;
        }
        float scale = (dBFS - 30) / 5;
        //Debug.Log($" Scale value is : {scale}");
        if (scale < 0)
        {
            // ✅ If scale was positive before and now it's negative, start coroutine
            if (_wasPositive && _saveCoroutine == null)
            {
                _saveCoroutine = StartCoroutine(DelayedSave());
            }
            _wasPositive = false;
            return;
        }

        _wasPositive = true; // Mark that we are in a positive scale range
        gameObject.transform.localScale = new Vector3(1, scale, 1);
    }

    private IEnumerator DelayedSave()
    {
        Debug.Log("⏳ Waiting 1 second before saving...");
        yield return new WaitForSeconds(1f); // ✅ Waits for 1 second

        SaveRecordedAudio();
        _saveCoroutine = null; // Reset coroutine reference
    }

    private void SaveRecordedAudio()
    {
        if (_audioData.Count > 0)
        {
            string audioFilePath = Path.Combine(Application.persistentDataPath + "/", "unity_voice_processor_auto.wav");
            var wavFileWriter = new WavFileWriter();
            wavFileWriter.Save("unity_voice_processor_auto.wav", _audioData);
            Debug.Log("✅ Audio saved: unity_voice_processor_auto.wav");

            _audioData.Clear(); // ✅ Clear buffer after saving, but keep recording

            TranscribeAudio(audioFilePath);
        }
        else
        {
            Debug.Log("⚠️ No audio recorded, skipping save.");
        }
    }

    private void TranscribeAudio(string audioFilePath)
    {
        try
        {
            SpeechClient speechClient = SpeechClient.Create();
            var response = speechClient.Recognize(new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = SampleRate,
                //EnableAutomaticPunctuation = true, // ✅ Adds punctuation
                Model = "latest_long",
                LanguageCode = "en-US"
            },
            RecognitionAudio.FromFile(audioFilePath));

            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Debug.Log($"Transcript: {alternative.Transcript}");
                }
            }
            Debug.Log("Speech-to-Text processing complete.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Google STT Error: {e.Message}");
        }
    }
}
*/