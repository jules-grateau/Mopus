using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjets.Abilities
{
    [CreateAssetMenu(fileName="Ability",menuName ="Scriptable/Ability",order =1)]
    public class Ability : ScriptableObject
    {
        public Sprite Icon => _icon;

        [SerializeField]
        Sprite _icon;
        public string Name => _name;
        [SerializeField]
        string _name;

        public Range Range => _range;
        [SerializeField]
        Range _range;

        public int Damage => _damage;
        [SerializeField]
        int _damage;
    }
}