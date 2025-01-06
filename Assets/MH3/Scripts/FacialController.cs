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
        private string defaultFacialName;

        void Start()
        {
            SetFacial(defaultFacialName);
            Debug.Log("FacialController.Start() called");
        }

        public void SetFacial(string facialName)
        {
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
