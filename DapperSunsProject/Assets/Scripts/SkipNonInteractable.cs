using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkipNonInteractable : MonoBehaviour, ISelectHandler
{
    private Selectable m_Selectable;

    // Use this for initialization
    void Awake()
    {
        m_Selectable = GetComponent<Selectable>();
    }

    public void OnSelect(BaseEventData evData)
    {
        // Don't apply skipping unless we are not interactable.
        if (m_Selectable.interactable) return;

        // Check if the user navigated to this selectable.
        if (Input.GetAxis("Horizontal") < 0)
        {
            Selectable select = m_Selectable.FindSelectableOnLeft();
            if (select == null || !select.gameObject.activeInHierarchy)
                select = m_Selectable.FindSelectableOnRight();
            StartCoroutine(DelaySelect(select));
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            Selectable select = m_Selectable.FindSelectableOnRight();
            if (select == null || !select.gameObject.activeInHierarchy)
                select = m_Selectable.FindSelectableOnLeft();
            StartCoroutine(DelaySelect(select));
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            Selectable select = m_Selectable.FindSelectableOnDown();
            if (select == null || !select.gameObject.activeInHierarchy)
                select = m_Selectable.FindSelectableOnUp();
            StartCoroutine(DelaySelect(select));
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            Selectable select = m_Selectable.FindSelectableOnUp();
            if (select == null || !select.gameObject.activeInHierarchy)
                select = m_Selectable.FindSelectableOnDown();
            StartCoroutine(DelaySelect(select));
        }
    }

    // Delay the select until the end of the frame.
    // If we do not, the current object will be selected instead.
    private IEnumerator DelaySelect(Selectable select)
    {
        yield return new WaitForEndOfFrame();

        if (select != null || !select.gameObject.activeInHierarchy)
            select.Select();
        else
            Debug.LogWarning("Please make sure your explicit navigation is configured correctly.");
    }
}