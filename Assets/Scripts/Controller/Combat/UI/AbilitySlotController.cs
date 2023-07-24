using Assets.Scripts.ScriptableObjets.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controller.Combat.UI
{

    [RequireComponent(typeof(Button))]
    public class AbilitySlotController : MonoBehaviour
    {
        Image _icon;
        TextMeshProUGUI _keyBind;
        Ability _ability;
        Button _button;

        public void Init(Ability ability, string keyBindText)
        {
            _icon = transform.Find("Icon").GetComponent<Image>();
            _keyBind = transform.Find("KeyBind").GetComponent<TextMeshProUGUI>();
            _button = GetComponent<Button>();
            _ability = ability;

            _icon.sprite = ability.Icon;
            _keyBind.text = keyBindText;
            _button.onClick.AddListener(OnClick);
        }

        void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);    
        }

        void OnClick()
        {
            CustomEvents.CustomEvents.SelectAbilityEvent.Invoke(_ability);
        }
    }
}