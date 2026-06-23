using UnityEngine;

public static class CollisionResolver 
{
    static readonly Collider[] Colliders = new Collider[16];

    public static bool GetPenetrationInLayer(this CharacterController controller, LayerMask layerMask, out Vector3 totalCorrection)
    {
        totalCorrection = Vector3.zero;

        if (controller == null) return false;

        Vector3 position = controller.transform.position;

        int count = Physics.OverlapCapsuleNonAlloc(GetBottom(controller, position),GetTop(controller, position), controller.radius, Colliders, layerMask);

        if(count == 0) return false;

        bool collided = false;

        for (int i = 0; i < count; i++)
        {
            Collider other = Colliders[i];

            if (other == null) continue;

            if (Physics.ComputePenetration(controller, position, controller.transform.rotation, other, other.transform.position, other.transform.rotation,
                out Vector3 dir, out float dist))
            {
                collided = true;

                Vector3 away = controller.transform.position - other.transform.position;

                away.y = 0;

                if (away.sqrMagnitude > 0.001f)
                {
                    totalCorrection += away.normalized * (dist * 1.1f);
                }
            }
        }

        return collided;
    }

    public static bool GetPenetrationInLayer(this Collider coll, LayerMask layerMask, out Vector3 totalCorrection)
    {
        totalCorrection = Vector3.zero;

        if (coll == null) return false;

        Vector3 position = coll.transform.position;

        int count = Physics.OverlapBoxNonAlloc(coll.bounds.center, coll.bounds.extents, Colliders, Quaternion.identity, layerMask);

        if (count == 0) return false;

        bool collided = false;

        for (int i = 0; i < count; i++)
        {
            Collider other = Colliders[i];

            if (other == null) continue;

            if (Physics.ComputePenetration(coll, position, coll.transform.rotation, other, other.transform.position, other.transform.rotation,
                out Vector3 dir, out float dist))
            {
                collided = true;

                Vector3 horizontalDir = new Vector3(dir.x, 0, dir.z);

                totalCorrection += horizontalDir.normalized * dist;
            }
        }

        return collided;
    }

    static Vector3 GetBottom(CharacterController characterController, Vector3 pos)
    {
        return pos + Vector3.up * characterController.radius;
    }

    static Vector3 GetTop(CharacterController characterController, Vector3 pos)
    {
        return pos + Vector3.up * (characterController.height - characterController.radius);
    }
}
