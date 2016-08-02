using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Spin : MonoBehaviour {

    [SerializeField]
    [Range(0f,20f)]
    public float spinRate = 5f;

    [SerializeField]
    [Range(0f, 200f)]
    public float spikeRadius = 25f;
    public enum SpinDirection {CLOCKWISE, ANTICLOCKWISE};
    public SpinDirection spinDir;
    private GameObject spike1, spike2;

    void Start()
    {
        spike1 = transform.FindChild("CollisionObject1").gameObject;
        spike2 = transform.FindChild("CollisionObject2").gameObject;
    }

    void Update()
    {
        spike1.transform.localScale = new Vector3(spike1.transform.localScale.x, spikeRadius, spike1.transform.localScale.z);
        spike2.transform.localScale = new Vector3(spikeRadius, spike2.transform.localScale.y, spike2.transform.localScale.z);

        if(spinDir == SpinDirection.CLOCKWISE)
        {
            transform.Rotate(new Vector3(0,0,-1 * spinRate * 50 * Time.deltaTime));
        }
        else
        {
            transform.Rotate(new Vector3(0,0,spinRate * 50 * Time.deltaTime));
        }
    }
}
