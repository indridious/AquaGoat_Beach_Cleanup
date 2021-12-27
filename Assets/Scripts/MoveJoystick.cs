using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class MoveJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Image bgImage;
    private Image joystickImage;
    public Vector2 InputDir { set; get; }
    public float offset;
    

    public void Start()
    {
        bgImage = GetComponent<Image>();
        joystickImage = transform.GetChild(0).GetComponent<Image>();
        InputDir = Vector2.zero;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;
        float bgImageSizeX = bgImage.rectTransform.sizeDelta.x;
        float bgImageSizeY = bgImage.rectTransform.sizeDelta.y;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform,eventData.position,eventData.pressEventCamera,out pos))
        {
            pos.x /= bgImageSizeX;
            pos.y /= bgImageSizeY;
            InputDir = new Vector2(pos.x, pos.y);
            InputDir = InputDir.magnitude > 1 ? InputDir.normalized : InputDir;
             
            joystickImage.rectTransform.anchoredPosition = new Vector2(InputDir.x * (bgImageSizeX/offset), InputDir.y * (bgImageSizeY/offset));
            
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        InputDir = Vector2.zero;
        joystickImage.rectTransform.anchoredPosition = Vector2.zero;
        
    }    

}
