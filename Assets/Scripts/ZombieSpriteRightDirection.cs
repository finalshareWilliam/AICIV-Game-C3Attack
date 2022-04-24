using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpriteRightDirection : MonoBehaviour
{
    [SerializeField] private AIPath aiPath;

    void Update()
    {
        transform.right = aiPath.desiredVelocity;
    }
}
