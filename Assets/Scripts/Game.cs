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

    void Awake()
    {
        Level1.SetActive(true);
        Level2.SetActive(false);
        Level3.SetActive(false);
        Level4.SetActive(false);
        Center1.SetActive(false);
        Center2.SetActive(false);
    }

    void Update()
    {
        // --- 1. Door Distance Checks ---

        // Level 1 Door Check
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
                // Only reset if we are not transitioning
                if (!isTransitioning)
                {
                    ResetDoorState(true);
                    isLevel1ToCenter = false;
                }
            }
        }

        // Center Room Door Checks
        else if (Center1.activeInHierarchy)
        {
            distance = Vector3.Distance(Player.position, CenterRoomtoLevel1Door.position);
            distance2 = Vector3.Distance(Player.position, CenterRoomtoFireWallDoor.position);

            bool nearDoor1 = distance <= 3f;
            bool nearDoor2 = distance2 <= 3f;

            // Default reset for Center Room doors
            ResetDoorState(true);
            isCenterToLevel1 = false;
            isCenterToLevel2 = false;

            if (nearDoor1 && !isTransitioning)
            {
                DoorNameWindow.SetActive(true);
                DoorName.text = "First Room";
                DoorDescriptionWindow.SetActive(true);
                DoorDescription.text = "Press X or Enter to go in";
                canEnterDoor = true;
                isCenterToLevel1 = true;
            }
            else if (nearDoor2 && !isTransitioning)
            {
                DoorNameWindow.SetActive(true);
                DoorName.text = "Firewall Room";
                DoorDescriptionWindow.SetActive(true);
                DoorDescription.text = "Press X or Enter to go in";
                canEnterDoor = true;
                isCenterToLevel2 = true;
            }
            else if (distance2 > 3 && distance > 3)
            {
                // UI already reset by ResetDoorState(true) above
            }
        }

        // Level 2 Door Check
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
                if (!isTransitioning)
                {
                    ResetDoorState(true);
                    isLevel2ToCenter = false;
                }
            }
        }

        // --- 2. Input Check to Trigger Transition (The essential fix) ---
        if (canEnterDoor && !isTransitioning && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Return)))
        {
            StartCoroutine(DoorTransitionRoutine());
        }
    }

    public void Transition()
    {
        // 1. Level 1 to Center
        if (isLevel1ToCenter)
        {
            Level1.SetActive(false);
            Center1.SetActive(true);
            Player.position = new Vector3(-218.1322f, Player.position.y, 2.186936f);
        }
        // 2. Level 2 to Center
        else if (isLevel2ToCenter)
        {
            Level2.SetActive(false);
            Center1.SetActive(true);
            Player.position = new Vector3(-204.322f, Player.position.y, -13.74279f);
        }
        // 3. Center to Level 1
        else if (isCenterToLevel1)
        {
            Center1.SetActive(false);
            Level1.SetActive(true);
            Player.position = new Vector3(-225.863f, Player.position.y, 1.521716f);
        }
        // 4. Center to Level 2
        else if (isCenterToLevel2)
        {
            Center1.SetActive(false);
            Level2.SetActive(true);
            Player.position = new Vector3(-202.3167f, Player.position.y, -19.5631f);
        }

        // Reset all door state flags after transition is complete
        ResetDoorState(false);
    }

    private void ResetDoorState(bool onlyUI = true)
    {
        // Always reset UI when called during Update checks
        DoorNameWindow.SetActive(false);
        DoorDescriptionWindow.SetActive(false);
        DoorName.text = "";
        DoorDescription.text = "";
        canEnterDoor = false;

        if (!onlyUI)
        {
            // Fully reset all specific door flags after a successful transition
            isLevel1ToCenter = false;
            isLevel2ToCenter = false;
            isLevel3ToCenter = false;
            isLevel4ToCenter = false;
            isCenterToLevel1 = false;
            isCenterToLevel2 = false;
            isCenterToLevel3 = false;
            isCenterToLevel4 = false;
        }
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
        ResetDoorState(true);

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
            c.a = 1f;
            fadeImage.color = c;
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        // 2) Perform the Scene/Level Change and Player Repositioning
        Transition();

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