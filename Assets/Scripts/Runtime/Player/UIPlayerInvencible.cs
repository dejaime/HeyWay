using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HayWay.Runtime.Components
{
    public class UIPlayerInvencible : MonoBehaviour
    {
        [SerializeField] private RectTransform m_panelContent;
        [SerializeField] private Image m_imageInvencibleFill;

        IEnumerator corotine;

        float amount = 0;
        private void Awake()
        {
            m_panelContent.gameObject.SetActive(false);
            m_imageInvencibleFill.fillAmount = 0;
        }
        private void OnEnable()
        {
            PlayerController.OnInvencibleChanged += OnInvencibleChanged;
        }
        private void OnDisable()
        {
            PlayerController.OnInvencibleChanged -= OnInvencibleChanged;

        }

        IEnumerator IEUpdate(PlayerController controller)
        {
          
            bool runing = IsRuning(controller);
            m_panelContent.gameObject.SetActive(runing);
            while (runing)
            {
                m_imageInvencibleFill.fillAmount = controller.InvencibleAmount;
                runing = IsRuning(controller);
                yield return null;
            }

            m_imageInvencibleFill.fillAmount = 0;
            m_panelContent.gameObject.SetActive(false);
            corotine = null;

        }
        bool IsRuning(PlayerController controller)
        {
            if (controller == null) { return false; }
            if (controller.IsDead) { return false; }
            if (!controller.IsInvencible) { return false; }
            return true;
        }
        private void OnInvencibleChanged(PlayerController controller)
        {
            if (corotine != null) { return; }
            
            corotine = IEUpdate(controller);
            StartCoroutine(corotine);
        }
    }
}