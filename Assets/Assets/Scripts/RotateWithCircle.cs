using UnityEngine;

namespace Assets
{
    public class RotateWithCircle : MonoBehaviour
    {

        public GameObject follow;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update ()
        {
            this.transform.rotation = follow.transform.rotation;
        }
    }
}
