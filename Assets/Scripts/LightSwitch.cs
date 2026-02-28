using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    private Dictionary<GameObject, Light[]> roomLights = new Dictionary<GameObject, Light[]>();
    private Dictionary<GameObject, bool[]> roomLightStates = new Dictionary<GameObject, bool[]>();
    private Queue<int> correctOrder = new Queue<int>(new int[] { 7, 5, 4, 8, 2, 3, 6, 1 });
    private Queue<int> playerOrder = new Queue<int>();
    
    private bool room1FirstToggleOff = false;
    private bool room4FirstToggleOff = true;

    public GameObject firstDoor;
    public AudioSource switchSound;
    public GameObject DoorUnlock;
    private Vector3 doorDesiredPosition;
    private Quaternion doorDesiredRotation;
    public AudioSource DoorUnlockSound;
    public AudioClip DoorUnlockClip;
    public AudioClip MonsterChase;

    public Transform SpawnMonsterPoint; // Assign in inspector
    public GameObject MonsterPrefab; // Assign in inspector
    public AudioSource KnockingSound;
    
    void Start()
    {
        doorDesiredPosition = new Vector3(14.35f, 10.284f, 16.369f);
        doorDesiredRotation = Quaternion.Euler(-107, 0, 0);
        // Find all rooms and set their initial light states
        for (int i = 1; i <= 8; i++)
        {
            GameObject room = GameObject.Find("Room" + i);
            if (room != null)
            {
                Light[] lights = room.GetComponentsInChildren<Light>();
                roomLights[room] = lights;
                
                bool[] states = new bool[lights.Length];
                
                // Room1 start with lights ON, all others OFF
                bool shouldBeOn = (i == 1);
                
                for (int j = 0; j < lights.Length; j++)
                {
                    lights[j].enabled = shouldBeOn;
                    states[j] = shouldBeOn;
                }
                
                roomLightStates[room] = states;
            }
        }
    }
    
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 3f, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("LightSwitch") && Input.GetKeyDown(KeyCode.E))
            {
                // Find the room parent (Room1, Room2, etc.)
                Transform roomParent = hit.collider.transform.parent;
                
                if (roomParent != null)
                {
                    ToggleLights(roomParent.gameObject);
                    if (switchSound != null)
                    {
                        switchSound.Play();
                    }
                }
                else
                {
                    Debug.LogWarning("Light switch has no parent room!");
                }
            }
        }
    }

    void ToggleLights(GameObject room)
    {
        if (!roomLights.ContainsKey(room))
        {
            Debug.LogWarning("Room not found in dictionary: " + room.name);
            return;
        }

        Light[] lightsToToggle = roomLights[room];
        bool[] isOn = roomLightStates[room];

        // Check if lights are currently on before toggling
        bool wereOn = isOn[0]; // Check first light state

        // Toggle lights normally
        for (int i = 0; i < lightsToToggle.Length; i++)
        {
            isOn[i] = !isOn[i];
            lightsToToggle[i].enabled = isOn[i];
        }

        // Extract room number from room name (e.g., "Room7" -> 7)
        string roomName = room.name;
        if (roomName.StartsWith("Room"))
        {
            string numberStr = roomName.Substring(4);
            if (int.TryParse(numberStr, out int roomNumber))
            {
                // Check for Room1 first time turning off
                if (roomNumber == 1 && wereOn && !room1FirstToggleOff)
                {
                    room1FirstToggleOff = true;
                    Room1FirstTimeOff();
                }

                // Check for Room4 first time turning off
                if (roomNumber == 4 && !wereOn && room4FirstToggleOff)
                {
                    room4FirstToggleOff = false;
                    Room4FirstTimeOff();
                }

                // Add to player's order
                playerOrder.Enqueue(roomNumber);
                CheckArrengment();
                Debug.Log("Player toggled Room" + roomNumber);

                // Check if player has toggled 8 switches
                if (playerOrder.Count == 8)
                {
                    CheckPuzzleSolution();
                }
            }
        }
    }
    void CheckArrengment()
    {
        Debug.Log("Current Player Order: " + string.Join(", ", playerOrder.ToArray()));
        Debug.Log("Correct Order: " + string.Join(", ", correctOrder.ToArray()));
        for (int i = 0; i < playerOrder.Count; i++)
        {
            if (playerOrder.ElementAt(i) != correctOrder.ElementAt(i))
            {
                Debug.Log("Player order does not match the correct order.");
                Invoke(nameof(ResetLights), 2f);
                return;
            }
        }
    }
    void ResetLights()
    {
        //add sound effect here
        switchSound.Play();
        Debug.Log("Resetting lights to initial states.");
        playerOrder.Clear();
        for (int i = 0; i <= 8; i++)
        {
            GameObject room = GameObject.Find("Room" + i);
            if (room != null)
            {
                Light[] lights = room.GetComponentsInChildren<Light>();
                roomLights[room] = lights;

                bool[] states = new bool[lights.Length];

                for (int j = 0; j < lights.Length; j++)
                {
                    lights[j].enabled = false;
                    states[j] = false;
                }

                
                roomLightStates[room] = states;
            }
        }
    }
    
    void Room1FirstTimeOff()
    {
        Debug.Log("🔦 Room1 lights turned off for the first time!");
        DoorUnlockSound.PlayOneShot(DoorUnlockClip);
        // Add your special event here
        // For example: spawn an item, play a sound, reveal a clue, etc.
        Animator doorAnimator = firstDoor.GetComponent<Animator>();
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
            Debug.Log("🚪 First door opening!");
        }
        else
        {
            Debug.LogWarning("No Animator found on the first door!");
        }
    }
    
    void Room4FirstTimeOff()
    {
        Debug.Log("🔦 Room4 lights turned off for the first time!");
        // Add your special event here
        // For example: open a secret door, spawn enemy, play spooky sound, etc.
    }
    
    void CheckPuzzleSolution()
    {
        bool isCorrect = true;
        Queue<int> correctCopy = new Queue<int>(correctOrder);
        Queue<int> playerCopy = new Queue<int>(playerOrder);
        
        // Compare both queues
        while (correctCopy.Count > 0 && playerCopy.Count > 0)
        {
            if (correctCopy.Dequeue() != playerCopy.Dequeue())
            {
                isCorrect = false;
                break;
            }
        }
        
        if (isCorrect)
        {
            Debug.Log("PUZZLE SOLVED! Correct order!");
            // Trigger your event here (open door, unlock something, etc.)
            PuzzleSolved();
        }
        else
        {
            Debug.Log("Wrong order! Resetting...");
            // Clear the player's order to try again
            playerOrder.Clear();
            for (int i = 0; i <= 8; i++)
            {
                GameObject room = GameObject.Find("Room" + i);
                if (room != null)
                {
                    Light[] lights = room.GetComponentsInChildren<Light>();
                    roomLights[room] = lights;

                    bool[] states = new bool[lights.Length];

                    for (int j = 0; j < lights.Length; j++)
                    {
                        lights[j].enabled = false;
                        states[j] = false;
                    }

                    
                    roomLightStates[room] = states;
                }
            }
        }
    }
    
    void PuzzleSolved()
    {
        DoorUnlockSound.PlayOneShot(MonsterChase);
        KnockingSound.Stop();
        // Add your puzzle completion logic here
        // For example: open a door, play a sound, unlock next area, etc.
        Debug.Log("🎉 You can now proceed!");
        if (DoorUnlock != null)
        {
            DoorUnlock.transform.position = doorDesiredPosition;
            DoorUnlock.transform.rotation = doorDesiredRotation;
        }

        // Spawn the monster at the designated spawn point
        if (MonsterPrefab != null && SpawnMonsterPoint != null)
        {
            Instantiate(MonsterPrefab, SpawnMonsterPoint.position, SpawnMonsterPoint.rotation);
            Debug.Log("👹 Monster spawned!");
        }
    }
}