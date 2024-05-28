using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.UI
{
    public class UnitInfoUIController : MonoBehaviour
    {
        private TextMeshProUGUI _nameTMP;
        private TextMeshProUGUI _healthTMP;

        private int _maxHealth;

        private void Awake()
        {
            _nameTMP = transform.Find("Name").GetComponent<TextMeshProUGUI>();   
            _healthTMP = transform.Find("Health").GetComponent<TextMeshProUGUI>();
        }

        public void Init(int maxHealth, int currHealth, string name)
        {
            _maxHealth = maxHealth;

            _nameTMP.SetText(name);
            _healthTMP.SetText($"{currHealth}/{_maxHealth}HP");
        }

        public void UpdateCurrHp(int currHealth)
        {
            _healthTMP.SetText($"{currHealth}/{_maxHealth}HP");
        }
    }
}