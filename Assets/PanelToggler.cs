using UnityEngine;
using UnityEngine.UI;

public class PanelToggler : MonoBehaviour
{
    [SerializeField] GameObject targetObject;
    [SerializeField] Sprite hidePanelSprite;
    [SerializeField] Sprite showPanelSprite;
    [SerializeField] bool currentState;
    private Image Image;
    private void Start()
    {
        Image = GetComponent<Image>();
    }
    public void Toggle()
    {
        currentState = !currentState;
        Image.sprite = currentState ? hidePanelSprite : showPanelSprite;
        targetObject.SetActive(currentState);
    }

}
