using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cucumber : MonoBehaviour, IFood
{
    public void Share()
    {
        throw new System.NotImplementedException();
    }

    void Eat()
        {

        }

    void IFood.Eat()
    {
        Eat();
    }
}