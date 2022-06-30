using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputNamePanel : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button confirmBtn;
    public static string DisplayName { get; private set; } = "";

    private void Start()
    {
        SetConfirmButtonState();
    }

    public void SetConfirmButtonState()
    {
        confirmBtn.interactable = !string.IsNullOrEmpty(nameInput.text);
    }

    public void ConfirmDisplayName()
    {
        DisplayName = nameInput.text;
    }
}
