using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropObject : MonoBehaviour
{
    [SerializeField]
    private float dropDistance = 1.0f;
    [SerializeField]
    private int bankID = 0;
    [SerializeField]
    private float resetDelay = 0.5f;
    [SerializeField]
    private static List<DropObject> dropTargets = new List<DropObject>();
    [SerializeField]
    private bool isDropped = false;

    void Start()
    {
        dropTargets.Add(this);
    }

    void Update()
    {

    }

    void OnCollisionEnter()
    {
        if (!isDropped)
        {
            //Fazer descer
            transform.position += Vector3.down * dropDistance;
            isDropped = true;

            //Se os seus "parceiros" cairam
            bool resetBank = true;
            foreach (DropObject target in dropTargets)
                if (target.returnId() == bankID)
                    if (!target.returnState())
                        resetBank = false;

            //Delayed reset
            if (resetBank)            
                Invoke("ResetBank", resetDelay);            
        }
    }

    void ResetBank()
    {
        foreach (DropObject target in dropTargets)
        {
            if (target.returnId() == bankID)
            {
                target.transform.position += Vector3.up * dropDistance;
                target.isDropped = false;
            }
        }
    }
    public int returnId()
    {
        return bankID;
    }
    public bool returnState()
    {
        return isDropped;
    }
}