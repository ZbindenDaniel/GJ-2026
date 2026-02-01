using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

public class GameMenuUI : MonoBehaviour
{
    [Header("Menu Root")]
    [SerializeField] private GameObject gameMenuCanvas;

    [Header("Buttons")]
    [SerializeField] private Button openMenuIconButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button firstSelectedButton;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        ResolveReferences();

        if (gameMenuCanvas != null)
        {
            gameMenuCanvas.SetActive(false);
        }

        WireButton(openMenuIconButton, OpenMenu);
        WireButton(backButton, CloseMenu);
        WireButton(quitGameButton, QuitGame);
        WireButton(mainMenuButton, GoToMainMenu);
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
#else
        if (Input.GetKeyDown(KeyCode.Escape))
#endif
        {
            ToggleMenu();
        }
    }

    private void ResolveReferences()
    {
        if (gameMenuCanvas == null)
        {
            GameObject canvasObject = FindInactiveObject("GameMenuCanvas");
            if (canvasObject != null)
            {
                gameMenuCanvas = canvasObject;
            }
        }

        if (openMenuIconButton == null)
        {
            openMenuIconButton = FindButton("OpenMenuIcon");
        }

        if (backButton == null)
        {
            backButton = FindButton("Back");
        }

        if (quitGameButton == null)
        {
            quitGameButton = FindButton("QuitGame");
        }

        if (mainMenuButton == null)
        {
            mainMenuButton = FindButton("MenuButton (1)") ?? FindButton("MainMenuButton");
        }
    }

    private void OpenMenu()
    {
        if (gameMenuCanvas != null)
        {
            gameMenuCanvas.SetActive(true);
        }
        EnsureEventSystem();
        SelectFirstButton();
    }

    private void CloseMenu()
    {
        if (gameMenuCanvas != null)
        {
            gameMenuCanvas.SetActive(false);
        }
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void ToggleMenu()
    {
        if (gameMenuCanvas == null)
        {
            return;
        }

        gameMenuCanvas.SetActive(!gameMenuCanvas.activeSelf);
        if (gameMenuCanvas.activeSelf)
        {
            EnsureEventSystem();
            SelectFirstButton();
        }
        else if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void GoToMainMenu()
    {
        if (!string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private static void WireButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.AddListener(action);
    }

    private static Button FindButton(string name)
    {
        GameObject target = FindInactiveObject(name);
        if (target == null)
        {
            return null;
        }

        Button button = target.GetComponent<Button>();
        if (button != null)
        {
            return button;
        }

        return target.GetComponentInChildren<Button>(true);
    }

    private void SelectFirstButton()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        Button target = firstSelectedButton != null ? firstSelectedButton : backButton;
        if (target != null)
        {
            EventSystem.current.SetSelectedGameObject(target.gameObject);
        }
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
        eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
    }

    private static GameObject FindInactiveObject(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            GameObject candidate = allObjects[i];
            if (!candidate.scene.IsValid() || !candidate.scene.isLoaded)
            {
                continue;
            }

            if (candidate.hideFlags != HideFlags.None)
            {
                continue;
            }

            if (candidate.name == name)
            {
                return candidate;
            }
        }

        return null;
    }
}
