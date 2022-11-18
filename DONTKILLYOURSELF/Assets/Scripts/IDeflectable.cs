using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeflectable
{
    void shoot(float angle, float speed, Vector2 initialVel);
    void deflect(float angle);
    bool getDeflectsOffProjectiles();
}