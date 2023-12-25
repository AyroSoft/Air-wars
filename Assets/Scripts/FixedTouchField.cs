using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsRightPressed { get; private set; }
    public Vector2 TouchDelta { get; private set; }

    public Vector2 PointerOld { get; private set; }

    protected int PointerId { get; private set; }

    public bool IsPressed { get; private set; }

    [SerializeField] private Vector2 rightPressPosition;
    [SerializeField] private float sensitive = 0.1f;
    [SerializeField] private float swipeThreshold = 2.5f;

    private Touch touch;
    private bool isSwipe;

    private void Update()
    {
        if (IsPressed)
        {
#if UNITY_EDITOR
            ExecuteMouse();
#else
ExecuteTouchs();
#endif
        }
        else
        {
            TouchDelta = Vector2.zero;
        }
    }

    private void ExecuteMouse()
    {
        if (isSwipe == false)
        {
            var delta = Vector2.Distance(Input.mousePosition, PointerOld);
            if (delta > swipeThreshold)
                isSwipe = true;
        }
        else
            TouchDelta = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld) * sensitive;
        PointerOld = Input.mousePosition;
    }

    private void ExecuteTouchs()
    {
        if (Input.touches.Length >= 0)
        {
            var id = -1;
            for (int i = 0; i < Input.touchCount; i++)
                if (Input.touches[i].fingerId == PointerId)
                    id = i;

            if (id < 0)
                return;

            if (isSwipe == false)
            {
                var delta = Vector2.Distance(Input.touches[id].position, PointerOld);
                if (delta > swipeThreshold)
                    isSwipe = true;
            }
            else
                TouchDelta = (Input.touches[id].position - PointerOld) * sensitive;
            PointerOld = Input.touches[id].position;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsPressed)
            return;

        IsPressed = true;
        PointerId = Input.touches[Input.touchCount - 1].fingerId;
        PointerOld = Input.touches[Input.touchCount - 1].position;

        var inputPosition = new Vector2(Screen.width - eventData.position.x, eventData.position.y);
        if (inputPosition.x < rightPressPosition.x && inputPosition.y < rightPressPosition.y)
            IsRightPressed = true;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        IsRightPressed = false;
    }
}
