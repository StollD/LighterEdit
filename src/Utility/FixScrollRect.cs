using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LighterEdit.Utility
{
    // https://forum.unity.com/threads/child-objects-blocking-scrollrect-from-scrolling.311555/#post-2351789
    public class FixScrollRect: MonoBehaviour, IBeginDragHandler,  IDragHandler, IEndDragHandler, IScrollHandler
    {
        public ScrollRect MainScroll;
 
        public void OnBeginDrag(PointerEventData eventData)
        {
            MainScroll.OnBeginDrag(eventData);
        }
 
        public void OnDrag(PointerEventData eventData)
        {
            MainScroll.OnDrag(eventData);
        }
 
        public void OnEndDrag(PointerEventData eventData)
        {
            MainScroll.OnEndDrag(eventData);
        }
 
 
        public void OnScroll(PointerEventData data)
        {
            MainScroll.OnScroll(data);
        }
 
    }
}