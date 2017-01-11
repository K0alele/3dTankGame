using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScoreManager))]
public class DropObject : MonoBehaviour
{
    [SerializeField]
    private float dropDistance = 1.0f;
    [SerializeField]
    private int groupID = 0;
    [SerializeField]
    private float resetDelay = 0.5f;
    [SerializeField]
    private static List<DropObject> dropTargets = new List<DropObject>();
    [SerializeField]
    private bool isDropped = false;
    [SerializeField]
    private int value = 100;
    [SerializeField]
    private int AllValue = 1000;

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

            ScoreManager.score += value;

            //Se os seus "parceiros" cairam
            bool resetGroup = true;
            foreach (DropObject target in dropTargets)
                if (target.returnId() == groupID)
                    if (!target.returnState())
                        resetGroup = false;

            //Delayed reset
            if (resetGroup)
            {
                ScoreManager.score += AllValue;
                Invoke("ResetGroup", resetDelay);
            }         
        }
    }

    void ResetGroup()
    {
        foreach (DropObject target in dropTargets)
        {
            if (target.returnId() == groupID)
            {
                target.transform.position += Vector3.up * dropDistance;
                target.isDropped = false;
            }
        }
    }
    public int returnId()
    {
        return groupID;
    }
    public bool returnState()
    {
        return isDropped;
    }
}