using System.Collections;
using UnityEngine;

namespace audio_subtitle_system
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Load Method")]
        public bool useResourcesFolder = true;
        public string jsonFileName = "dialogue_data"; // Without .json extension
        
        [Header("Or Assign Directly")]
        public TextAsset[] dialogueJsonFiles; // Drag multiple .json.txt files here
        
        [Header("Selection")]
        public int selectedJsonIndex = 0; // Which JSON to play from the array
        
        public audio_subtitle_player subtitleManager;

        private DataWrapper dialogueData;
        public bool isPlaying = false;

        void Start()
        {
            Invoke(nameof(PlayFirstDialogue), 6f);
        }

        void PlayFirstDialogue()
        {
            PlayAllDialoguesFromFile(0);
        }


        // Load specific JSON file by index
        public void LoadDialogueData(int jsonIndex)
        {
            selectedJsonIndex = jsonIndex;
            LoadDialogueData();
        }

        // Load the currently selected JSON
        private void LoadDialogueData()
        {
            string jsonText = "";

            if (useResourcesFolder)
            {
                // Load from Resources folder
                TextAsset jsonAsset = Resources.Load<TextAsset>(jsonFileName);
                if (jsonAsset != null)
                {
                    jsonText = jsonAsset.text;
                }
                else
                {
                    Debug.LogError($"Could not find {jsonFileName} in Resources folder!");
                    return;
                }
            }
            else
            {
                // Load from assigned TextAsset array
                if (dialogueJsonFiles != null && dialogueJsonFiles.Length > 0)
                {
                    if (selectedJsonIndex >= 0 && selectedJsonIndex < dialogueJsonFiles.Length)
                    {
                        if (dialogueJsonFiles[selectedJsonIndex] != null)
                        {
                            jsonText = dialogueJsonFiles[selectedJsonIndex].text;
                            Debug.Log($"Loaded JSON file: {dialogueJsonFiles[selectedJsonIndex].name}");
                        }
                        else
                        {
                            Debug.LogError($"JSON file at index {selectedJsonIndex} is null!");
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Index {selectedJsonIndex} is out of range! Available files: {dialogueJsonFiles.Length}");
                        return;
                    }
                }
                else
                {
                    Debug.LogError("No JSON files assigned to DialogueTrigger!");
                    return;
                }
            }

            // Parse the JSON
            dialogueData = JsonUtility.FromJson<DataWrapper>(jsonText);
            
            if (dialogueData == null || dialogueData.items.Length == 0)
            {
                Debug.LogError("Failed to parse JSON or JSON is empty!");
            }
            else
            {
                Debug.Log($"Successfully loaded {dialogueData.items.Length} dialogue entries.");
            }
        }

        // Call this method to play a specific dialogue by key
        public void PlayDialogueByKey(string dialogueKey)
        {
            if (dialogueData == null)
            {
                Debug.LogWarning("Dialogue data not loaded! Loading now...");
                LoadDialogueData();
            }

            if (dialogueData == null) return;

            foreach (var entry in dialogueData.items)
            {
                if (entry.key == dialogueKey && !string.IsNullOrWhiteSpace(entry.value))
                {
                    subtitleManager.display(entry.value, entry.duration);
                    break;
                }
            }
        }

        // Call this method to play all dialogues in sequence from selected JSON
        public void PlayAllDialogues()
        {
            if (dialogueData == null)
            {
                Debug.LogWarning("Dialogue data not loaded! Loading now...");
                LoadDialogueData();
            }
            
            StartCoroutine(PlayDialoguesSequence());
        }

        // Play all dialogues from a specific JSON file
        public void PlayAllDialoguesFromFile(int jsonIndex)
        {
            LoadDialogueData(jsonIndex);
            PlayAllDialogues();
        }

        // Call this method to play a range of dialogues in sequence
        public void PlayDialoguesInRange(int startIndex, int endIndex)
        {
            if (dialogueData == null)
            {
                Debug.LogWarning("Dialogue data not loaded! Loading now...");
                LoadDialogueData();
            }
            
            StartCoroutine(PlayDialoguesSequenceRange(startIndex, endIndex));
        }

        private IEnumerator PlayDialoguesSequence()
        {
            if (dialogueData == null) yield break;

            foreach (var entry in dialogueData.items)
            {
                if (!string.IsNullOrWhiteSpace(entry.value))
                {
                    isPlaying = true;
                    subtitleManager.display(entry.value, entry.duration);
                    yield return new WaitForSeconds(entry.duration + 0.2f);
                    isPlaying = false;
                }
            }

            if (selectedJsonIndex == 0)
            {
                yield return new WaitForSeconds(1f);
                // After finishing the first JSON, automatically play the second one
                PlayAllDialoguesFromFile(1);
                Car car = FindObjectOfType<Car>();
                if (car != null)
                {
                    car.maxSpeed = 300f; // Increase max speed after first dialogue sequence
                }
            }
            else if (selectedJsonIndex == 1)
            {
                yield return null;
                // After finishing the second JSON, automatically play the third one
                phoneBehaviour phone = FindObjectOfType<phoneBehaviour>();
                if (phone != null)
                {
                    phone.StopPhoneSound();
                }
            }
        }

        private IEnumerator PlayDialoguesSequenceRange(int startIndex, int endIndex)
        {
            if (dialogueData == null) yield break;

            for (int i = startIndex; i <= endIndex && i < dialogueData.items.Length; i++)
            {
                var entry = dialogueData.items[i];
                if (!string.IsNullOrWhiteSpace(entry.value))
                {
                    isPlaying = true;
                    subtitleManager.display(entry.value, entry.duration);
                    yield return new WaitForSeconds(entry.duration + 0.2f);
                    isPlaying = false;
                }
            }
        }

        // Example: Trigger from another script or event
        private void Update()
        {
            // Press number keys to load and play different JSON files
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayAllDialoguesFromFile(0); // Play first JSON file
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayAllDialoguesFromFile(1); // Play second JSON file
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayAllDialoguesFromFile(2); // Play third JSON file
            }
            
            // Press Space to play all from currently selected JSON
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayAllDialogues();
            }
        }
    }
}