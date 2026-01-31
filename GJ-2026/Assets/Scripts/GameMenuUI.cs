using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [Header("Menu Root")]
    [SerializeField] private GameObject gameMenuCanvas;

    [Header("Buttons")]
    [SerializeField] private Button openMenuIconButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private Button mainMenuButton;

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
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }

    private void CloseMenu()
    {
        if (gameMenuCanvas != null)
        {
            gameMenuCanvas.SetActive(false);
        }
    }

    private void ToggleMenu()
    {
        if (gameMenuCanvas == null)
        {
            return;
        }

        gameMenuCanvas.SetActive(!gameMenuCanvas.activeSelf);
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
