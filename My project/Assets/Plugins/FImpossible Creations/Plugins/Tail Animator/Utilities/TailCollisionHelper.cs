using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Simple class sending collision events to main script
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Hidden/Tail Collision Helper")]
    public class TailCollisionHelper : MonoBehaviour
    {
        public TailAnimator2 ParentTail;
        public Collider TailCollider;
        public int Index;

        internal Rigidbody RigBody { get; private set; }
        Transform previousCollision;

        internal TailCollisionHelper Init(bool addRigidbody = true, float mass = 1f, bool kinematic = false)
        {
            if (addRigidbody)
            {
                Rigidbody rig = GetComponent<Rigidbody>();
                if (!rig) rig = gameObject.AddComponent<Rigidbody>();
                rig.interpolation = RigidbodyInterpolation.Interpolate;
                rig.useGravity = false;
                rig.isKinematic = kinematic;
                rig.constraints = RigidbodyConstraints.FreezeAll;
                rig.mass = mass;
                RigBody = rig;
            }
            else
            {
                RigBody = GetComponent<Rigidbody>();
                if (RigBody) RigBody.mass = mass;
            }

            return this;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (ParentTail == null)
            {
                GameObject.Destroy(this);
                return;
            }

            TailCollisionHelper helper = collision.transform.GetComponent<TailCollisionHelper>();

            if (helper)
            {
                if (ParentTail.CollideWithOtherTails == false) return;
                if (helper.ParentTail == ParentTail) return;
            }

            if (ParentTail._TransformsGhostChain.Contains(collision.transform)) return;
            if (ParentTail.IgnoredColliders.Contains(collision.collider)) return;

            ParentTail.CollisionDetection(Index, collision);
            previousCollision = collision.transform;
        }


        //private void OnCollisionStay(Collision collision)
        //{
        //    OnCollisionEnter(collision);
        //}


        void OnCollisionExit(Collision collision)
        {
            if (collision.transform == previousCollision)
            {
                ParentTail.ExitCollision(Index);
                previousCollision = null;
            }
        }


        void OnTriggerEnter(Collider other)
        {
            TailCollisionHelper helper = other.transform.GetComponent<TailCollisionHelper>();

            if (other.isTrigger) return;

            if (helper)
            {
                if (ParentTail.CollideWithOtherTails == false) return;
                if (helper.ParentTail == ParentTail) return;
            }

            if (ParentTail._TransformsGhostChain.Contains(other.transform)) return;
            if (ParentTail.IgnoredColliders.Contains(other)) return;

            if (ParentTail.IgnoreMeshColliders)
                if (other is MeshCollider) return;

            ParentTail.AddCollider(other);
        }

        void OnTriggerExit(Collider other)
        {
            if (ParentTail.IncludedColliders.Contains(other))
            {
                if (!ParentTail.DynamicAlwaysInclude.Contains(other))
                    ParentTail.IncludedColliders.Remove(other);
            }
        }
    }
}