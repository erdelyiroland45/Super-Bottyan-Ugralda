using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ButtonSelecting : MonoBehaviour
{
    public Selectable firstSelectable; // Assign the first selectable UI element in the Inspector
    private GameObject lastSelected; // Keep track of the last selected button
    private float inputCooldown = 0.2f; // Cooldown to prevent fast scrolling
    private float lastInputTime = 0f;

    private void Start()
    {
        // Set the initial selected UI element
        EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
        lastSelected = firstSelectable.gameObject;
        UpdateSelectionVisual(lastSelected, true);
    }

    private void Update()
    {
        if (Gamepad.current != null) // Check if a controller is connected
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            // If the selection has changed, update the visual
            if (selected != lastSelected)
            {
                UpdateSelectionVisual(lastSelected, false); // Remove highlight from previous button
                UpdateSelectionVisual(selected, true); // Add highlight to new button
                lastSelected = selected;
            }

            // Handle Navigation Input (Left Stick or D-Pad)
            Vector2 moveInput = Gamepad.current.leftStick.ReadValue(); // Read left stick input
            bool dpadUp = Gamepad.current.dpad.up.wasPressedThisFrame;
            bool dpadDown = Gamepad.current.dpad.down.wasPressedThisFrame;

            if (Time.time - lastInputTime > inputCooldown) // Apply cooldown
            {
                if (moveInput.y > 0.5f || dpadUp)
                {
                    SelectNext(-1);
                    lastInputTime = Time.time;
                }
                else if (moveInput.y < -0.5f || dpadDown)
                {
                    SelectNext(1);
                    lastInputTime = Time.time;
                }
            }

            // Check for "A" button press
            if (Gamepad.current.buttonSouth.wasPressedThisFrame && selected != null) // "A" button
            {
                Button btn = selected.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.Invoke(); // Trigger button's OnClick
                }
            }
        }
    }

    private void SelectNext(int direction)
    {
        Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
        if (current != null)
        {
            Selectable next = (direction > 0) ? current.FindSelectableOnDown() : current.FindSelectableOnUp();
            if (next != null)
            {
                EventSystem.current.SetSelectedGameObject(next.gameObject);
            }
        }
    }

    private void UpdateSelectionVisual(GameObject button, bool isSelected)
    {
        if (button != null)
        {
            Image img = button.GetComponent<Image>();
            if (img != null)
            {
                img.color = isSelected ? Color.gray : Color.white; // Change border color
            }
        }
    }
}
