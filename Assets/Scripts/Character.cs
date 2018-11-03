using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    private float deltaX;
    private Quaternion spineRotation;
    private bool m_aim;

    public UnityEvent OnFire;

    private Animator m_animator;
    private bool m_picked;

    private bool m_enableIK;
    private float m_weightIK;
    private Vector3 m_positionIK;

    // Use this for initialization
    void Start()
    {
        // Initialize Animator
        m_animator = GetComponent<Animator>();
    }

    public void Move(float turn, float forward, bool jump, bool picked)
    {
        m_animator.SetFloat("Turn", turn);
        m_animator.SetFloat("Forward", forward);

        m_picked = picked;

        if (jump)
        {
            m_animator.SetTrigger("Jump");
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "pickable")
        {
            var pickable = other.GetComponent<Pickable>();

            if (m_picked && pickable != null && !pickable.picked)
            {
                //Debug.Log("Picking");
                Transform rightHand = m_animator.GetBoneTransform(HumanBodyBones.RightHand);
                pickable.bePicked(rightHand);

                m_animator.SetTrigger("Pick");
                StartCoroutine(UpdateIK(other));
            }
        }
        
    }

    private IEnumerator UpdateIK(Collider other)
    {
        m_enableIK = true;

        while(m_enableIK)
        {
            m_positionIK = other.transform.position;
            m_weightIK = m_animator.GetFloat("IK");
            yield return null;
        }
    }

    public void DisableIK()
    {
        m_enableIK = false;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(m_enableIK)
        {
            m_animator.SetIKPosition(AvatarIKGoal.RightHand, m_positionIK);
            m_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_weightIK);
        }

        if(m_aim)
        {
            //do spine rotation
            Vector3 rotationEular = new Vector3(0, deltaX, 0);
            Quaternion rotationOffset = Quaternion.Euler(rotationEular);

            spineRotation = Quaternion.Lerp(spineRotation, spineRotation * rotationOffset, Time.deltaTime * 50.0f);

            rotationEular = spineRotation.eulerAngles;
            if (rotationEular.y > 180)
            {
                rotationEular.y -= 360;
            }
            if (rotationEular.y < 180)
            {
                rotationEular.y += 360;
            }
            rotationEular.y = Mathf.Clamp(rotationEular.y, -60.0f, +60.0f);

            m_animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(rotationEular));
        }
    }

    public void AimFire(bool aimDown, bool aimHold, bool fire)
    {
        m_animator.SetBool("Aim", aimHold);

        if(aimHold && fire)
        {
            m_animator.SetTrigger("Fire");

            if(OnFire!= null)
            {
                OnFire.Invoke();
            }
        }
    }
}

