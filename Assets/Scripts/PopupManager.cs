using System.Collections;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public GameObject dialogPopUpWindow;
    public TMP_Text errorMessageText;

    public GameObject customDialogPopUpWindow;
    public TMP_Text customErrorMessageText;
    private Animator popupAnimator;
    private Animator customPopupAnimator;

    void Start()
    {
        // Get the Animator component attached to the popup
        popupAnimator = dialogPopUpWindow.GetComponent<Animator>();
        customPopupAnimator = customDialogPopUpWindow.GetComponent<Animator>();

        // Ensure the popup starts hidden
        dialogPopUpWindow.SetActive(false);
        customDialogPopUpWindow.SetActive(false);
    }

    // Method to show the popup with an error message
    public void ShowError(string message)
    {
        errorMessageText.text = message;

        dialogPopUpWindow.SetActive(true);
        popupAnimator.Play("Popup");
    }

    public void HidePopup()
    {
        popupAnimator.Play("HidePopup");
        StartCoroutine(HideAfterAnimation());
    }

    public void ShowCustomError(string message)
    {
        customErrorMessageText.text = message;

        customDialogPopUpWindow.SetActive(true);
        customPopupAnimator.Play("Popup");
    }

    public void HideCustomPopup()
    {
        customPopupAnimator.Play("HidePopup");
        StartCoroutine(HideCustomAfterAnimation());
    }

    private IEnumerator HideAfterAnimation()
    {
        // Wait until the end of the current animation
        yield return new WaitForSeconds(popupAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Deactivate the popup after the animation
        dialogPopUpWindow.SetActive(false);
    }

    private IEnumerator HideCustomAfterAnimation()
    {
        // Wait until the end of the current animation
        yield return new WaitForSeconds(customPopupAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Deactivate the popup after the animation
        customDialogPopUpWindow.SetActive(false);
    }
}
