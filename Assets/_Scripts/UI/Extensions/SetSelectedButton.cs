using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Extensions
{
    public class SetSelectedButton : MonoBehaviour
    {
        public void SelectButton(GameObject go) 
        {
            FindObjectOfType<EventSystem>().SetSelectedGameObject(go);
        }
    }
}
