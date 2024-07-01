using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    [CreateAssetMenu(menuName = "HayWay/Characters Catalogue")]
    public class DataCharactersCatalogue : ScriptableObject
    {
        [SerializeField] List<DataCharacter> m_characters = new List<DataCharacter>();

        public List<DataCharacter> GetCharacters()
        {
            return m_characters.ToList();
        }
        public DataCharacter GetCharacter(int index)
        {
            return m_characters.ElementAt(index);
        }
        public int GetCharactersCount()
        {
            return m_characters.Count;
        }
    }
}