using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputBinder : MonoBehaviour
{
    public PlayerControls controls;

    public TextMeshProUGUI forwardText;
    public TextMeshProUGUI backwardText;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI crouchText;
    public TextMeshProUGUI dashText;

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void Start()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
            controls.Enable();
        }

        LoadBindingOverrides();
        UpdateAllBindingDisplays();
    }

    void StartRebind(string actionName, TextMeshProUGUI displayText)
    {
        var action = controls.asset.FindAction(actionName);
        if (action == null || displayText == null) return;

        action.Disable();

        if (rebindOperation != null)
        {
            rebindOperation.Dispose();
        }

        displayText.text = "Press a key...";

        rebindOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                op.Dispose();
                rebindOperation = null;
                action.Enable();
                displayText.text = action.GetBindingDisplayString();
                SaveBindingOverrides();
            })
            .OnCancel(op =>
            {
                op.Dispose();
                rebindOperation = null;
                action.Enable();
                displayText.text = action.GetBindingDisplayString();
            })
            .Start();
    }

    void UpdateAllBindingDisplays()
    {
        UpdateBindingDisplay("Forward", forwardText);
        UpdateBindingDisplay("Backward", backwardText);
        UpdateBindingDisplay("Left", leftText);
        UpdateBindingDisplay("Right", rightText);
        UpdateBindingDisplay("Jump", jumpText);
        UpdateBindingDisplay("Crouch", crouchText);
        UpdateBindingDisplay("Dash", dashText);
    }

    void UpdateBindingDisplay(string actionName, TextMeshProUGUI displayText)
    {
        var action = controls.asset.FindAction(actionName);
        if (action != null && displayText != null)
        {
            displayText.text = action.GetBindingDisplayString();
        }
    }

    public void RebindForward() => StartRebind("Forward", forwardText);
    public void RebindBackward() => StartRebind("Backward", backwardText);
    public void RebindLeft() => StartRebind("Left", leftText);
    public void RebindRight() => StartRebind("Right", rightText);
    public void RebindJump() => StartRebind("Jump", jumpText);
    public void RebindCrouch() => StartRebind("Crouch", crouchText);
    public void RebindDash() => StartRebind("Dash", dashText);

    void SaveBindingOverrides()
    {
        string rebinds = controls.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("inputRebinds", rebinds);
    }

    void LoadBindingOverrides()
    {
        string rebinds = PlayerPrefs.GetString("inputRebinds", string.Empty);
        if (!string.IsNullOrEmpty(rebinds))
        {
            controls.LoadBindingOverridesFromJson(rebinds);
        }
    }
    public void ResetBindings()
    {
        PlayerPrefs.DeleteKey("inputRebinds"); // or DeleteAll()
        PlayerPrefs.Save();

        controls.RemoveAllBindingOverrides();
        UpdateAllBindingDisplays();
    }
}
