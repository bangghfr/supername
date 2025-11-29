using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potato : MonoBehaviour, IFood
{
    void Eat()
    {

    }

    void IFood.Eat()
    {
        Eat();
    }

    void Share()
    {

    }

    void IFood.Share()
    {
        Share();
    }
}
