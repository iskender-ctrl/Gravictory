using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateGravitationalForce
{
    private const float GravitionalConstant = 6.67430e-11f; // Evrensel yerçekimi sabiti

    // İki gökcisminin çekim kuvvetini ve ikincil nesnenin son hızını hesaplayan fonksiyon
    public static Vector3 ComputePostVelocity(Vector3 position1, Vector3 velocity0, float mass1, float deltaTime, Vector3 position2, float mass2)
    {
        // İki cisim arasındaki mesafeyi hesapla
        Vector3 direction = position2 - position1;
        float distance = direction.magnitude;

        //Yerçekimi kuvvetini hesapla
        float forceMagnitude = (GravitionalConstant * mass1 * mass2) / (distance * distance);
        Vector3 gravitionalForce = direction.normalized * forceMagnitude;

        //mass2 objesinin son hızı vektörünü hesaplayıp geri döndür
        Vector3 gravitionalAcceleration = gravitionalForce / mass2;
        Vector3 gravitionalVelocity = gravitionalAcceleration * deltaTime;
        Vector3 postVelocity = gravitionalVelocity + velocity0;
        return postVelocity;
    }

}
