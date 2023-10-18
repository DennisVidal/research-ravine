using UnityEngine;
using UnityEngine.UI;

public class ScrollMenu : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed;

    public void ScrollTop()
    {
        scrollRect.verticalNormalizedPosition = 1.0f;
    }
    public void ScrollBottom()
    {
        scrollRect.verticalNormalizedPosition = 0.0f;
    }
    public void ScrollUp()
    {
        ScrollVertical(scrollSpeed * Time.deltaTime);
    }
    public void ScrollDown()
    {
        ScrollVertical(-scrollSpeed * Time.deltaTime);
    }

    public void ScrollVertical(float delta)
    {
        scrollRect.verticalNormalizedPosition += delta / scrollRect.content.sizeDelta.y;
    }
    public void ScrollHorizontal(float delta)
    {
        scrollRect.horizontalNormalizedPosition += delta / scrollRect.content.sizeDelta.x;
    }
}
