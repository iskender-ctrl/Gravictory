using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateGravitationalForce
{

    private const float GravitionalConstant = 6.67430e-11f; // Evrensel yerçekimi sabiti

    // İki gökcisminin çekim kuvvetini hesaplayan fonksiyon
    public static Vector3 ComputeForce(Vector3 position1, float mass1, Vector3 position2, float mass2)
    {
        // İki cisim arasındaki mesafeyi hesapla
        Vector3 direction = position2 - position1;
        float distance = direction.magnitude;

        //Yerçekimi kuvvetini hesapla
        float forceMagnitude = (GravitionalConstant * mass1 * mass2) / (distance * distance);
        Vector3 force = direction.normalized*forceMagnitude;
        return force;
    }

}
