using Assets.Scripts.ScriptableObjets.Abilities;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controller.Combat.UI
{
    public class SelectedAbilityUIController : MonoBehaviour
    {
        Image _icon;
        void Awake()
        {
            _icon = transform.Find("Icon").GetComponentInChildren<Image>();
        }

        void OnEnable()
        {
            CustomEvents.CustomEvents.SelectAbilityEvent.AddListener(OnSelectAbility);
            CustomEvents.CustomEvents.UnselectAbilityEvent.AddListener(OnUnselectAbility);
        }

        void OnDisable()
        {
            CustomEvents.CustomEvents.SelectAbilityEvent.RemoveListener(OnSelectAbility);
            CustomEvents.CustomEvents.UnselectAbilityEvent.RemoveListener(OnUnselectAbility);
        }

        void OnSelectAbility(Ability ability)
        {
            _icon.gameObject.SetActive(true);
            _icon.sprite = ability.Icon;
        }

        void OnUnselectAbility()
        {
            _icon.gameObject.SetActive(false);
            _icon.sprite = null;
        }
    }
}