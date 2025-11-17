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

    public Image fadeImage;         // Assign the black fullscreen Image here
    public float fadeDuration = 0.5f;
    public bool isTransitioning = false;
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
                DoorNameWindow.SetActive(false);
                DoorDescriptionWindow.SetActive(false);
                DoorName.text = "";
                DoorDescription.text = "";
                canEnterDoor = false;
                isLevel1ToCenter = false;
            }
        }
        else if (Center1.activeInHierarchy)
        {
            distance = Vector3.Distance(Player.position, CenterRoomtoLevel1Door.position);
            

            if (distance <= 3f && !isTransitioning)
            {
                DoorNameWindow.SetActive(true);
                DoorName.text = "First Room";
                DoorDescriptionWindow.SetActive(true);
                DoorDescription.text = "Press X or Enter to go in";
                canEnterDoor = true;
                isCenterToLevel1 = true;
            }


            distance2 = Vector3.Distance(Player.position, CenterRoomtoFireWallDoor.position);

            if (distance2 <= 3f && !isTransitioning)
            {
                DoorNameWindow.SetActive(true);
                DoorName.text = "Firewall Room";
                DoorDescriptionWindow.SetActive(true);
                DoorDescription.text = "Press X or Enter to go in";
                canEnterDoor = true;
                isCenterToLevel2 = true;
            }
            
            if (distance2 > 3 && distance > 3)
            {
                DoorNameWindow.SetActive(false);
                DoorDescriptionWindow.SetActive(false);
                DoorName.text = "";
                DoorDescription.text = "";
                canEnterDoor = false;
                isCenterToLevel1 = false;
                isCenterToLevel2 = false;
            }
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

    }

    public void Transition()
    {
        if (Level1.activeInHierarchy)
        {
            Level1.SetActive(false);
            Center1.SetActive(true);
            Player.position = new Vector3(-218.1322f, Player.position.y, 2.186936f);
            DoorNameWindow.SetActive(false);
            DoorDescriptionWindow.SetActive(false);
            DoorName.text = "";
            DoorDescription.text = "";
            canEnterDoor = false;
            isCenterToLevel1 = false;
        }
        else if (Level2.activeInHierarchy)
        {
            Level2.SetActive(false);
            Center1.SetActive(true);
            Player.position = new Vector3(-204.322f, Player.position.y, -13.74279f);
            DoorNameWindow.SetActive(false);
            DoorDescriptionWindow.SetActive(false);
            DoorName.text = "";
            DoorDescription.text = "";
            canEnterDoor = false;
            isCenterToLevel2 = false;
        }

        else if (Center1.activeInHierarchy && isCenterToLevel1)
        {
            Center1.SetActive(false);
            Level1.SetActive(true);
            DoorNameWindow.SetActive(false);
            DoorDescriptionWindow.SetActive(false);
            DoorName.text = "";
            DoorDescription.text = "";
            canEnterDoor = false;
            isCenterToLevel1 = false;
            Player.position = new Vector3(-225.863f, Player.position.y, 1.521716f);
        }

        else if (Center1.activeInHierarchy && isCenterToLevel2)
        {
            Center1.SetActive(false);
            Level2.SetActive(true);
            DoorNameWindow.SetActive(false);
            DoorDescriptionWindow.SetActive(false);
            DoorName.text = "";
            DoorDescription.text = "";
            canEnterDoor = false;
            isCenterToLevel2 = false;
            Player.position = new Vector3(-202.3167f, Player.position.y, -19.5631f);
        }
    }

    public IEnumerator DoorTransitionRoutine()
    {
        isTransitioning = true;
        canEnterDoor = false;
        player.enabled = false;
        // Hide door UI immediately
        DoorNameWindow.SetActive(false);
        DoorDescriptionWindow.SetActive(false);
        DoorName.text = "";
        DoorDescription.text = "";
        

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
            // If no fadeImage assigned, just wait a bit
            yield return new WaitForSeconds(fadeDuration);
        }

        Transition();
        

        // (If you later add more doors/levels, you can extend this logic.)

        // Small pause if you want, otherwise remove this line
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
        player.enabled = true;
        isTransitioning = false;
    }

    public void Resume()
    {
        HUD.SetActive(true);
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
