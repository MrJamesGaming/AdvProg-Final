using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target 
{

    private float delay;
    private Vector3 position;
    private float comboNum;
    private float nextComboNum;
    
    
    public Target(float delay, Vector3 position, int comboNum, int nextComboNum)
    {
        this.delay = delay;
        this.position = position;
        this.comboNum = comboNum;
        this.nextComboNum = nextComboNum;
    }
    public Target(float delay, Vector3 position, int comboNum)
    {
        this.delay = delay;
        this.position = position;
        this.comboNum = comboNum;
    }
    public float getDelay()
    {
        return delay;
    }
    public Vector3 getPosition()
    {
        return position;
    }
    public float getComboNum()
    {
        return comboNum;
    }
    public float getNextComboNum()
    {
        return nextComboNum;
    }
    public void setNextComboNum(int value)
    {
        nextComboNum = value;
    }
}
