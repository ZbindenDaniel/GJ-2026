using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuBootstrap : MonoBehaviour
{
    private const string CanvasName = "MainMenuCanvas";
    private const string EventSystemName = "EventSystem";

    [SerializeField] private string gameSceneName = "SampleScene";

    private Canvas menuCanvas;
    private TextMeshProUGUI testStatusLabel;

    private void Start()
    {
        GameObject existingCanvas = GameObject.Find(CanvasName);
        if (existingCanvas != null)
        {
            menuCanvas = existingCanvas.GetComponent<Canvas>();
            Transform statusTransform = existingCanvas.transform.Find("MenuPanel/TestStatusLabel");
            if (statusTransform != null)
            {
                testStatusLabel = statusTransform.GetComponent<TextMeshProUGUI>();
            }
            return;
        }

        EnsureEventSystem();
        menuCanvas = CreateCanvas();

        RectTransform panel = CreatePanel(menuCanvas.transform);
        Button startButton = CreateButton(panel, "I want to leave the party", new Vector2(0f, 80f));
        startButton.onClick.AddListener(StartGame);

        Button testButton = CreateButton(panel, "Run Test", new Vector2(0f, 0f));
        testButton.onClick.AddListener(RunTest);

        Button exitButton = CreateButton(panel, "I cant get out. Please just end it.", new Vector2(0f, -80f));
        exitButton.onClick.AddListener(ExitGame);

        testStatusLabel = CreateLabel(panel, "Test: Ready", new Vector2(0f, -140f), 18, "TestStatusLabel");
        testStatusLabel.enabled = false;
    }

    private void StartGame()
    {
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(false);
        }

        Debug.Log("Start Game pressed. Loading gameplay scene.");
        SceneManager.LoadScene(gameSceneName);
    }

    private void RunTest()
    {
        Debug.Log("Test button pressed.");

        if (testStatusLabel != null)
        {
            testStatusLabel.enabled = true;
            testStatusLabel.text = "Test: OK";
        }
    }

    private void ExitGame()
    {
        Debug.Log("Exit button pressed.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject(EventSystemName, typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        eventSystem.AddComponent<InputSystemUIInputModule>();
#else
        eventSystem.AddComponent<StandaloneInputModule>();
#endif
        eventSystem.transform.SetParent(null);
    }

    private static Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject(
            CanvasName,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform rectTransform = canvasObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        return canvas;
    }

    private static RectTransform CreatePanel(Transform parent)
    {
        GameObject panelObject = new GameObject("MenuPanel", typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        Image image = panelObject.GetComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        return rectTransform;
    }

    private static Button CreateButton(RectTransform parent, string label, Vector2 position)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(260f, 60f);
        rectTransform.anchoredPosition = position;

        TextMeshProUGUI text = CreateLabel(rectTransform, label, Vector2.zero, 22);
        text.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        return buttonObject.GetComponent<Button>();
    }

    private static TextMeshProUGUI CreateLabel(RectTransform parent, string textValue, Vector2 position, int fontSize, string name = "Text")
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = textValue;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchoredPosition = position;

        return text;
    }
}
