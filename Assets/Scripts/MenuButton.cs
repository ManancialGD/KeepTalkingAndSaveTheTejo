using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClickEvent;

    private Animator anim;

    public bool isSelected = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    public void SetSelection(bool select)
    {
        isSelected = select;
        if (anim != null)
        {
            anim.SetBool("isSelected", isSelected);
        }
        else
        {
            Debug.LogError("Animator component is not assigned on " + gameObject.name);
        }
    }

    public void OnClick()
    {
        onClickEvent?.Invoke();
    }
}
