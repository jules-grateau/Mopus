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

        public int MinRange => _minRange;
        [SerializeField]
        int _minRange;

        public int MaxRange => _maxRange;
        [SerializeField]
        int _maxRange;

        public int Damage => _damage;
        [SerializeField]
        int _damage;

    }
}