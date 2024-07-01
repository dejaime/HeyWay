using HayWay.Runtime.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HayWay.Runtime
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private DataCharactersCatalogue m_CharCatalogue;
        [SerializeField] private Transform m_CharacterContent;
        [SerializeField] private GameObject m_defaultTestChar;
        private void Awake()
        {
            if (LocalDataBase.IsLoaded)
            {
                Destroy(m_defaultTestChar);
            }

        }
        private void Start()
        {
            if (!LocalDataBase.IsLoaded)
            {
                return;
            }

            DataCharacter character = m_CharCatalogue.GetCharacter(LocalDataBase.PlayerData.Character);
            GameObject go = Instantiate(character.Model, Vector3.zero, Quaternion.identity, m_CharacterContent);
            go.transform.localPosition = Vector3.zero;


        }
    }
}