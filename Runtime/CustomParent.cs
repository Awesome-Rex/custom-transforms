using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.REXCore;

namespace REXTools.CustomTransforms
{
    public class CustomParent : MonoBehaviourPRO
    {
        [SerializeField]
        public Dictionary<CustomTransform<dynamic>, Transform> parents;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            foreach (KeyValuePair<CustomTransform<dynamic>, Transform> parent in parents)
            {
                ((CustomTransform<dynamic>)GetComponent(parent.Key.GetType())).parent = parent.Value;
            }
        }

        private void OnDrawGizmos()
        {
            //Works!!!
            //Debug.Log((CustomTransform<Vector3>)GetComponent(typeof(CustomTransform<Vector3>)) != null);
        }
    }
}