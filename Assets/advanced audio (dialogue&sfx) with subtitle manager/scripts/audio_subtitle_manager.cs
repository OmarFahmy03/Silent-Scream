using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;
using UnityEngine.Networking;
namespace audio_subtitle_system
{
    public class audio_subtitle_manager : MonoBehaviour
    {
        public Language subtitle_language=Language.English;
        public AudioSource audio_source;
        public bool subtitle_active = true;
        public audio_subtitle_player audio_subtitle_player;
        private AudioClip audio_clip;
        private string txt_string;
        private Dictionary<string, string> current_subtitle_dictionary;
        private Dictionary<string, string> subtitle_dictionary_EN;
        private Dictionary<string, string> subtitle_dictionary_FR;
        public enum Language
        {
        English,
        French
        }
        void Awake()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "subtitles", "subtitles EN.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                DataWrapper dataWrapper = JsonUtility.FromJson<DataWrapper>(json);
                subtitle_dictionary_EN = new Dictionary<string, string>();
                foreach (KeyValueItem item in dataWrapper.items)
                {
                    subtitle_dictionary_EN[item.key] = item.value;
                }
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
            }
            filePath = Path.Combine(Application.streamingAssetsPath, "subtitles", "subtitles FR.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                DataWrapper dataWrapper = JsonUtility.FromJson<DataWrapper>(json);
                subtitle_dictionary_FR = new Dictionary<string, string>();
                foreach (KeyValueItem item in dataWrapper.items)
                {
                    subtitle_dictionary_FR[item.key] = item.value;
                }
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
            }
        }
        IEnumerator play_audio_file(string title)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "audios", title + ".wav");
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                audio_clip = DownloadHandlerAudioClip.GetContent(www);
            }
            audio_source.clip = audio_clip;
            audio_source.Play();
            if (subtitle_active)
                display_subtitle(title);
        }
        IEnumerator display_subtitle_text(string title)
        {
            switch (subtitle_language) 
        {
            case Language.English: 
            {
                current_subtitle_dictionary=subtitle_dictionary_EN;
                break;
            }
            case Language.French:
            {
                current_subtitle_dictionary=subtitle_dictionary_FR;
                break;
            }
            default :
            {
                current_subtitle_dictionary=subtitle_dictionary_EN;
                break;
            }
        }
            if (current_subtitle_dictionary.ContainsKey(title) && !string.IsNullOrWhiteSpace(current_subtitle_dictionary[title]))
            {
                audio_subtitle_player.display(current_subtitle_dictionary[title], audio_clip.length);
            }
            else
            {
                yield return null;
            }
        }
        public void play_audio(string title)
        {
            StartCoroutine(play_audio_file(title));
        }
        public void display_subtitle(string title)
        {
            StartCoroutine(display_subtitle_text(title));
        }
    }
}