using UnityEngine;

public interface IDamage 
{
    void takeDamage(int amount, Vector3? knockbackDirection = null);
}