using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH3
{
    public class FacialController : MonoBehaviour
    {
        [SerializeField]
        private List<Element> elements;

        [SerializeField]
        private SimpleAnimation simpleAnimation;

        public void Play(string facialName)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            simpleAnimation.Play(facialName);
        }

        public void SetFacialFromAnimation(string facialName)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            foreach (var element in elements)
            {
                element.SetActive(facialName);
            }
        }

        public void SetFacial(string facialName)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            simpleAnimation.Stop();
            foreach (var element in elements)
            {
                element.SetActive(facialName);
            }
        }

        [Serializable]
        public class Element
        {
            [SerializeField]
            private GameObject facialObject;

            public void SetActive(string facialName)
            {
                facialObject.SetActive(facialObject.name == facialName);
            }
        }
    }
}
