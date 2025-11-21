using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Game : MonoBehaviour
{
    public GameObject Level1;
    public GameObject Level2;
    public GameObject Level3;
    public GameObject Level4;
    public GameObject Center1;
    public GameObject Center2;

    public Transform Player;
    public Transform Level1Door;
    public Transform CenterRoomtoLevel1Door;
    public Transform CenterRoomtoFireWallDoor;
    public Transform FireWalltoCenterDoor;

    public GameObject DoorNameWindow;
    public TextMeshProUGUI DoorName;
    public TextMeshProUGUI DoorDescription;
    public GameObject DoorDescriptionWindow;

    public float distance;
    public float distance2;
    public float distance3;
    public float distance4;
    public bool canEnterDoor;

    public bool isLevel1ToCenter;
    public bool isLevel2ToCenter;
    public bool isLevel3ToCenter;
    public bool isLevel4ToCenter;
    public bool isCenterToLevel1;
    public bool isCenterToLevel2;
    public bool isCenterToLevel3;
    public bool isCenterToLevel4;

    public Image fadeImage;
    public float fadeDuration = 0.5f;
    public bool isTransitioning = false;

    // NOTE: Ensure you have a script named 'Player' with an 'enabled' property
    public Player player;

    public GameObject PauseMenu;
    public GameObject HUD;

    public AudioSource pause;
    public AudioSource shoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Level1.SetActive(true);
        Level2.SetActive(false);
        Level3.SetActive(false);
        Level4.SetActive(false);
        Center1.SetActive(false);
        Center2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // --- 1. Door Distance Checks ---

        if (Level1.activeInHierarchy)
        {
            distance = Vector3.Distance(Player.position, Level1Door.position);

            if (distance <= 3f && !isTransitioning)
            {
                DoorNameWindow.SetActive(true);
                DoorName.text = "Center Room";
                DoorDescriptionWindow.SetActive(true);
                DoorDescription.text = "Press X or Enter to go in";
                canEnterDoor = true;
                isLevel1ToCenter = true;
            }
            else if (distance > 3)
            {
                // Reset UI and flags only if we are the only active check that failed
                if (!isTransitioning)
                {
                    DoorNameWindow.SetActive(false);
                    DoorDescriptionWindow.SetActive(false);
                    DoorName.text = "";
                    DoorDescription.text = "";
                    canEnterDoor = false;
                    isLevel1ToCenter = false;
                }
            }
        }
        else if (Center1.activeInHierarchy)
        {
            distance = Vector3.Distance(Player.position, CenterRoomtoLevel1Door.position);
            distance2 = Vector3.Distance(Player.position, CenterRoomtoFireWallDoor.position);

            // Reset flags and UI first to handle the two-door case correctly
            bool nearDoor1 = distance <= 3f;
            bool nearDoor2 = distance2 <= 3f;

            if (nearDoor1 && !isTransitioning)
            {
                DoorNameWindow.SetActive(true);
                DoorName.text = "First Room";
                DoorDescriptionWindow.SetActive(true);
                DoorDescription.text = "Press X or Enter to go in";
                canEnterDoor = true;
                isCenterToLevel1 = true;
                isCenterToLevel2 = false; // Ensure only one is true
            }
            else if (nearDoor2 && !isTransitioning)
            {
                DoorNameWindow.SetActive(true);
                DoorName.text = "Firewall Room";
                DoorDescriptionWindow.SetActive(true);
                DoorDescription.text = "Press X or Enter to go in";
                canEnterDoor = true;
                isCenterToLevel2 = true;
                isCenterToLevel1 = false; // Ensure only one is true
            }
            else if (distance2 > 3 && distance > 3)
            {
                DoorNameWindow.SetActive(false);
                DoorDescriptionWindow.SetActive(false);
                DoorName.text = "";
                DoorDescription.text = "";
                canEnterDoor = false;
                isCenterToLevel1 = false;
                isCenterToLevel2 = false;
            }
            // A potential issue here: If you are near DOOR 1, the UI will show DOOR 1. If you move from DOOR 1 to be near DOOR 2, the logic above correctly updates. If you are near BOTH, only the last one checked (DOOR 2 in this case) will display its text, but `canEnterDoor` will be true, and the correct `isCenterToLevelX` flag will be set in the `Transition()` function.
        }

        else if (Level2.activeInHierarchy)
        {
            distance = Vector3.Distance(Player.position, FireWalltoCenterDoor.position);


            if (distance <= 3f && !isTransitioning)
            {
                DoorNameWindow.SetActive(true);
                DoorName.text = "Center Room";
                DoorDescriptionWindow.SetActive(true);
                DoorDescription.text = "Press X or Enter to go in";
                canEnterDoor = true;
                isLevel2ToCenter = true;
            }

            else if (distance > 3)
            {
                DoorNameWindow.SetActive(false);
                DoorDescriptionWindow.SetActive(false);
                DoorName.text = "";
                DoorDescription.text = "";
                canEnterDoor = false;
                isLevel2ToCenter = false;
            }
        }

        // --- 2. Input Check to Trigger Transition (The Fix) ---

        if (canEnterDoor && !isTransitioning && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Return)))
        {
            // Start the coroutine to handle fading, transition, and player movement
            StartCoroutine(DoorTransitionRoutine());
        }
    }

    public void Transition()
    {
        // Level 1 to Center
        if (Level1.activeInHierarchy && isLevel1ToCenter)
        {
            Level1.SetActive(false);
            Center1.SetActive(true);
            // Move player to Center door exit
            Player.position = new Vector3(-218.1322f, Player.position.y, 2.186936f);

            // Clear flags/UI regardless of where we came from
            ResetDoorState();
        }
        // Level 2 to Center
        else if (Level2.activeInHierarchy && isLevel2ToCenter)
        {
            Level2.SetActive(false);
            Center1.SetActive(true);
            // Move player to Center door exit
            Player.position = new Vector3(-204.322f, Player.position.y, -13.74279f);

            // Clear flags/UI regardless of where we came from
            ResetDoorState();
        }
        // Center to Level 1
        else if (Center1.activeInHierarchy && isCenterToLevel1)
        {
            Center1.SetActive(false);
            Level1.SetActive(true);
            // Move player to Level 1 door exit
            Player.position = new Vector3(-225.863f, Player.position.y, 1.521716f);

            // Clear flags/UI regardless of where we came from
            ResetDoorState();
        }
        // Center to Level 2
        else if (Center1.activeInHierarchy && isCenterToLevel2)
        {
            Center1.SetActive(false);
            Level2.SetActive(true);
            // Move player to Level 2 door exit
            Player.position = new Vector3(-202.3167f, Player.position.y, -19.5631f);

            // Clear flags/UI regardless of where we came from
            ResetDoorState();
        }
    }

    private void ResetDoorState()
    {
        DoorNameWindow.SetActive(false);
        DoorDescriptionWindow.SetActive(false);
        DoorName.text = "";
        DoorDescription.text = "";
        canEnterDoor = false;

        // Reset all specific door flags to ensure clean state
        isLevel1ToCenter = false;
        isLevel2ToCenter = false;
        isLevel3ToCenter = false;
        isLevel4ToCenter = false;
        isCenterToLevel1 = false;
        isCenterToLevel2 = false;
        isCenterToLevel3 = false;
        isCenterToLevel4 = false;
    }

    public IEnumerator DoorTransitionRoutine()
    {
        isTransitioning = true;
        canEnterDoor = false;
        // Temporarily disable player movement script
        if (player != null)
        {
            player.enabled = false;
        }

        // Hide door UI immediately
        DoorNameWindow.SetActive(false);
        DoorDescriptionWindow.SetActive(false);

        // 1) Fade OUT to black
        if (fadeImage != null)
        {
            float t = 0f;
            Color c = fadeImage.color;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Clamp01(t / fadeDuration);
                c.a = alpha;
                fadeImage.color = c;
                yield return null;
            }

            // Ensure fully black
            c.a = 1f;
            fadeImage.color = c;
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        // 2) Perform the Scene/Level Change and Player Repositioning
        Transition();


        // Small pause 
        yield return new WaitForSeconds(0.1f);

        // 3) Fade IN from black
        if (fadeImage != null)
        {
            float t = 0f;
            Color c = fadeImage.color;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = 1f - Mathf.Clamp01(t / fadeDuration);
                c.a = alpha;
                fadeImage.color = c;
                yield return null;
            }

            // Ensure fully transparent again
            c.a = 0f;
            fadeImage.color = c;
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        // Re-enable player movement and set transition complete
        if (player != null)
        {
            player.enabled = true;
        }
        isTransitioning = false;
    }

    public void Resume()
    {
        HUD.SetActive(true);
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}