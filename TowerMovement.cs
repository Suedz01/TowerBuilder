using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMovement : MonoBehaviour
{
    [Range(1, 10)]
    public int speed;
    public bool isActive, hitGround, hitTower, inAir, freezePositionY;
    GameManager gm = GameManager.Instance;
    public float TowerHeight;
    Rigidbody rb;
    Collider col;


    public GameObject aGO;
    public GameObject bGO;
    public GameObject baseGO;
    Transform aTransform;
    Transform bTransform;
    Transform baseTransform;
    Vector3 aPos;
    Vector3 bPos;
    Vector3 basePos;

    public float lookness, preciseness;
    public float var1, var2;
    public bool triggerHorizontal;

    private void Awake()
    {
        aGO = GameObject.Find("aPos");
        if (aGO != null)
        {
            // Faça algo com playerObject, como obter um componente.
        }
        else
        {
            Debug.LogWarning("Objeto com nome 'aPos' não foi encontrado na cena!");
        }

        bGO = GameObject.Find("bPos");
        if (bGO != null)
        {
            // Faça algo com playerObject, como obter um componente.
        }
        else
        {
            Debug.LogWarning("Objeto com nome 'bPos' não foi encontrado na cena!");
        }

        baseGO = GameObject.Find("basePos");
        if (baseGO != null)
        {
            // Faça algo com playerObject, como obter um componente.
        }
        else
        {
            Debug.LogWarning("Objeto com nome 'basePos' não foi encontrado na cena!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
        hitGround = false;
        hitTower = false;
        inAir = true;

        rb = GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();
        aTransform = aGO.transform;
        bTransform = bGO.transform;
        baseTransform = baseGO.transform;

        aPos = aTransform.transform.position;
        bPos = bTransform.transform.position;
        basePos = baseTransform.transform.position;

        triggerHorizontal = true;
        lookness = 0.001f;
        speed = 4;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVectors();
        movTower();
        TowerHeight = col.bounds.size.y;
    }

    void movTower()
    {
        if (Input.GetKey(KeyCode.Space) && isActive)
        {
            freezePositionY = true;
            //Vai andar nas horizontais aqui
            checktriggerHorizontal();
        }
        else
        {
            isActive = false;
        }

        holdTower();
    }

    void UpdateVectors()
    {
        /*
            L�gica:
                Fazer o bloco se locomover de -6 at� +6
                Quanto mais pr�ximo das extremidades mais devagas ele vai ficar
                Ser� utilizado o mesmo vetor.x, mas com velocidades que ir�o variar
                Uma velocidade positiva ir� para +6
                Um velocidade negativa ir� para -6
         */

        preciseness = 1 - lookness;

        Vector3 triggerTopDir = ( transform.position - basePos ).normalized;
        Vector3 aPosBase = ( aPos - basePos ).normalized;
        Vector3 bPosBase = ( bPos - basePos ).normalized;

        var1 = Mathf.Abs( Vector3.Dot(triggerTopDir, aPosBase) );
        var2 = Mathf.Abs( Vector3.Dot(triggerTopDir, bPosBase) );
    }

    void checktriggerHorizontal()
    {
        if (triggerHorizontal)
        {
            Debug.Log("Quase Lá..");
            transform.position += (-transform.right) * speed * Time.deltaTime;
            if (var2 > preciseness)
            {
                Debug.Log("Cheguei..");
                triggerHorizontal = false;
            }
        }
        else
        { 
            Debug.Log("Quase Lá aqui...");
            transform.position += transform.right * speed * Time.deltaTime;
            if (var1 > preciseness)
            {
                Debug.Log("Quase Lá aqui tb...");
                triggerHorizontal = true;
            }
        }
    }

    void holdTower()
    {
        //Caso o item estiver ativo, ele estará sendo suspenso e não pode cair
        if (isActive) rb.constraints |= RigidbodyConstraints.FreezePositionY;
        else rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
    }

    #region Colisoes
    private void OnCollisionEnter(Collision collision)
    {
        if (inAir)
        {
            if (collision.gameObject.tag == "Ground")
            {
                Debug.Log("Encostou no chão");
                gm.ScoreDown();
                gm.StartCoroutine(gm.NewTower());
                inAir = false;
                rb.isKinematic = true;
            }
            else if (collision.gameObject.tag == "Tower")
            {
                //Caso o paciente acertou o alvo,
                //Atualize as alturas e crie uma nova torre!
                gm.ScoreUp();
                Debug.Log("Encostou em outra torre");
                gm.HeightUpdate(TowerHeight);
                gm.StartCoroutine(gm.NewTower());
                inAir = false;
                rb.isKinematic = true;
            }
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(basePos, transform.position);

        Gizmos.color = var1 <= preciseness ? Color.blue : Color.green;
        Gizmos.DrawLine(basePos, aPos);

        Gizmos.color = var2 <= preciseness ? Color.grey : Color.green;
        Gizmos.DrawLine(basePos, bPos);
    }
}
