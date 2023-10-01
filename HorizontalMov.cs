using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMov : MonoBehaviour
{
    public Transform aTrans;
    public Transform bTrans;

    Vector3 aPos;
    Vector3 bPos;

    // Start is called before the first frame update
    void Start()
    {
        aPos = aTrans.position;
        bPos = bTrans.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, aPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, bPos);
    }
}
