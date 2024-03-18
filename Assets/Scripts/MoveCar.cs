using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveCar : MonoBehaviour
{
    public Rigidbody rigid;

    private float currentAccel;
    private float currentTurnSpeed;

    [Header("Move")]
    public float forwardAccel = 6f;
    public float reverseAccel = 4f;
    public float turnStrength = 45f;
    public float gravityForce = 10f;
    public float dragOnGround = 3f;
    private float speedInput, turnInput;

    [Header("Draft")]
    public bool isDrift = false;
    public float driftSpeed = 4f;
    public float driftTurn = 90f;

    [Header("Ground Check")]
    private bool grounded;
    public LayerMask whatIsGround;
    public float groundRayLength = .5f;
    public Transform groundRayPoint;

    [Header("Wheel Anim")]
    public Transform leftFrontWheel;
    public Transform rightFrontWheel;
    public float maxWheelTurn= 25f;

    [Header("Trail Particle")]
    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f;
    private float emissionRate;

    private void Start()
    {
        rigid.transform.parent = null;
    }
    private void Update()
    {
        // Input
        speedInput = 0f;
        if(Input.GetAxis("Vertical") > 0)
                {
            speedInput = Input.GetAxis("Vertical") * currentAccel * 1000f;

        } else if(Input.GetAxis("Vertical") < 0)
                {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
        }
        turnInput = Input.GetAxis("Horizontal");

        
        // Draft
        if (Input.GetKeyDown(KeyCode.LeftShift) && grounded)
        {
            isDrift = true;
            rigid.AddForce(Vector3.up * 50f, ForceMode.Impulse);
            StartDrift();
        }

        // �帮��Ʈ ����
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isDrift = false;
            EndDrift();
        }

        // Rotation
        if (grounded)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, turnInput *
                currentTurnSpeed * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }

        // Animation
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);

        // rigidbody�� ������ ���󰡱�
        transform.position = rigid.gameObject.transform.localPosition - new Vector3(0, 0.5f, 0) ;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Trail();
        Draft();
    }

    private void GroundCheck()
    {
        // ���� �پ��ִ��� Ȯ��
        grounded = false;
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

    }

    private void Draft()
    {
        if(isDrift)
        {
            currentAccel = driftSpeed;
            currentTurnSpeed = driftTurn;
        }
        else
        {
            currentAccel = forwardAccel;
            currentTurnSpeed = turnStrength;
        }
    }

    private void Trail()
    {
        emissionRate = 0;

        if (grounded)
        {
            rigid.drag = dragOnGround;
            if (Mathf.Abs(speedInput) > 0)
            {
                rigid.AddForce(transform.forward * speedInput);
                emissionRate = maxEmission;
            }
        }
        else
        {
            rigid.drag = 0.1f;
            rigid.AddForce(Vector3.up * -gravityForce * 100f);
        }

        foreach (ParticleSystem part in dustTrail)
        {
            var emissionModule = part.emission;
            emissionModule.rateOverTime = emissionRate;
        }
    }

    // �帮��Ʈ ���� �� ȣ��� �Լ�
    void StartDrift()
    {
        rigid.AddForce(Vector3.up * 250f, ForceMode.Impulse);

        // �帮��Ʈ ���� �� �̵� �������� ��� ȸ���ϴ� ���� �߰�
        if (speedInput > 0) // ���� ���� ��
        {
            StartCoroutine(TurnDuringDrift(turnInput > 0 ? 15f : -15f)); // ���� �Է¿� ���� ȸ��
        }
        else if (speedInput < 0) // ���� ���� ��
        {
            StartCoroutine(TurnDuringDrift(turnInput > 0 ? -15f : 15f)); // ���� �� ���� ����
        }
    }

    // �帮��Ʈ ���� �� ȣ��� �Լ�
    void EndDrift()
    {
        // �帮��Ʈ ���� �� �ʿ��� ���� �߰� (����� ��� ����)
    }

    // �帮��Ʈ �� ȸ�� ������ ���� �ڷ�ƾ
    IEnumerator TurnDuringDrift(float turnAngle)
    {
        float turnTime = 0.5f; // ȸ���� ���ӵ� �ð� (��)
        float timer = 0f;
        while (timer < turnTime)
        {
            // �̵� ���������� ��� ȸ��
            transform.Rotate(0f, turnAngle * Time.deltaTime / turnTime, 0f);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}