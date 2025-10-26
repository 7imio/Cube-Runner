using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-100)]
public class CoreInput : MonoBehaviour
{
    public static CoreInput Instance { get; private set; }

    // ===== Visu Inspector =====
    [Header("Axes (lecture continue)")]
    [Tooltip("Déplacement latéral : A/D, ←/→, stick gauche X")]
    public float moveX;

    [Header("Boutons (WasPerformedThisFrame)")]
    public bool startPressedThisFrame;
    public bool pausePressedThisFrame;
    public bool restartPressedThisFrame;
    public bool jumpPressedThisFrame;

    [Header("Dernière source d'entrée")]
    public string lastControlPath;
    public string lastDevice;
    public string lastAction;

    [Header("Journal (dernier en haut)")]
    [TextArea(5, 12)] public string log;

    // ===== API (1-shot flags) =====
    private bool _startFlag;
    private bool _pauseFlag;
    private bool _restartFlag;
    private bool _jumpFlag;

    public bool ConsumeStart() { if (_startFlag) { _startFlag = false; return true; } return false; }
    public bool ConsumePause() { if (_pauseFlag) { _pauseFlag = false; return true; } return false; }
    public bool ConsumeRestart() { if (_restartFlag) { _restartFlag = false; return true; } return false; }
    public bool ConsumeJump() { if (_jumpFlag) { _jumpFlag = false; return true; } return false; }

    // ===== InputActions (code-only) =====
    private InputAction _move;
    private InputAction _start;
    private InputAction _pause;
    private InputAction _restart;
    private InputAction _jump;

    private const int MaxLogLines = 12;
    private readonly System.Collections.Generic.Queue<string> _lines = new();

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- MOVE (1D Axis) ---
        _move = new InputAction("Move", InputActionType.Value);
        _move.AddBinding("<Gamepad>/leftStick/x");
        var axis = _move.AddCompositeBinding("1DAxis");
        axis.With("Negative", "<Keyboard>/a");
        axis.With("Negative", "<Keyboard>/leftArrow");
        axis.With("Positive", "<Keyboard>/d");
        axis.With("Positive", "<Keyboard>/rightArrow");

        // --- START ---
        _start = new InputAction("Start", InputActionType.Button);
        _start.AddBinding("<Keyboard>/space");
        _start.AddBinding("<Gamepad>/buttonSouth");

        // --- PAUSE ---
        _pause = new InputAction("Pause", InputActionType.Button);
        _pause.AddBinding("<Keyboard>/escape");
        _pause.AddBinding("<Gamepad>/select");

        // --- RESTART ---
        _restart = new InputAction("Restart", InputActionType.Button);
        _restart.AddBinding("<Keyboard>/r");
        _restart.AddBinding("<Gamepad>/start");

        // --- JUMP ---
        _jump = new InputAction("Jump", InputActionType.Button);
        _jump.AddBinding("<Keyboard>/space");
        _jump.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
        _move.Enable();
        _start.Enable();
        _pause.Enable();
        _restart.Enable();
        _jump.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _start.Disable();
        _pause.Disable();
        _restart.Disable();
        _jump.Disable();
    }

    private void Update()
    {
        // Axe continu
        moveX = _move.ReadValue<float>();

        // Boutons (visu + flags consommables)
        startPressedThisFrame = _start.WasPerformedThisFrame();
        pausePressedThisFrame = _pause.WasPerformedThisFrame();
        restartPressedThisFrame = _restart.WasPerformedThisFrame();
        jumpPressedThisFrame = _jump.WasPerformedThisFrame();

        if (startPressedThisFrame) { _startFlag = true; CaptureSource(_start, "Start"); }
        if (pausePressedThisFrame) { _pauseFlag = true; CaptureSource(_pause, "Pause"); }
        if (restartPressedThisFrame) { _restartFlag = true; CaptureSource(_restart, "Restart"); }
        if (jumpPressedThisFrame) { _jumpFlag = true; CaptureSource(_jump, "Jump"); }
    }

    private void CaptureSource(InputAction action, string label)
    {
        var ctrl = action.activeControl;
        lastControlPath = ctrl != null ? ctrl.path : "(unknown)";
        lastDevice = ctrl?.device?.displayName ?? ctrl?.device?.name ?? "(unknown)";
        lastAction = label;
        PushLog($"{label} — {lastControlPath} [{lastDevice}]");
    }

    private void PushLog(string line)
    {
        var ts = System.DateTime.Now.ToString("HH:mm:ss.fff");
        _lines.Enqueue($"{ts}  {line}");
        while (_lines.Count > MaxLogLines) _lines.Dequeue();

        var arr = _lines.ToArray();
        System.Array.Reverse(arr);
        log = string.Join("\n", arr);
    }
}
