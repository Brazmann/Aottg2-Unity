using Effects;
using log4net.Util;
using Projectiles;
using Settings;
using System.Collections;
using UnityEngine;
using Utility;

namespace Characters
{
    class ReverseGasSpecial : SimpleUseable
    {
        float Speed = 150f;
        float gasCost = 5f;
        float radius = 15f;
        Vector3 Gravity = Vector3.down * 15f;

        public ReverseGasSpecial(BaseCharacter owner) : base(owner)
        {
            Cooldown = 1f;
        }

        protected override void Activate()
        {
            var human = (Human)_owner;
            if(human.Stats.CurrentGas < gasCost)
            {
                human.PlaySound(HumanSounds.NoGas);
                return;
            }
            Vector3 target = human.GetAimPoint();
            Vector3 start = human.Cache.Transform.position + human.Cache.Transform.up * 2f;
            Vector3 direction = (target - start).normalized;
            /*ProjectileSpawner.Spawn(ProjectilePrefabs.ReverseGas, start, Quaternion.identity, direction * Speed, Gravity, 6.5f, _owner.Cache.PhotonView.ViewID,
                "", new object[0]);*/
            Collider[] cols = Physics.OverlapSphere(start, radius, PhysicsLayer.GetMask(PhysicsLayer.Hurtbox));
            foreach (var collider in cols)
            {
                var root = collider.transform.root.gameObject;
                var titan = root.GetComponent<BasicTitan>();
                if (titan != null)
                {
                    if (collider == titan.BaseTitanCache.EyesHurtbox)
                    {
                        EffectSpawner.Spawn(EffectPrefabs.Boom1, start, Quaternion.Euler(270f, 0f, 0f));
                        float dist = Vector3.Distance(start, collider.transform.position);
                        if(dist < radius / 2)
                        {
                            titan.Attack("AttackHitFace");
                        } else
                        {
                            titan.Attack("AttackGrabAirR");
                        }
                        break; //Only want this to affect one titan. Remove this break to affect all in range.
                    }
                }
            }
            human.Dash(180, 0f, gasCost);
            human.Cache.Rigidbody.AddForce(-direction * 80f, ForceMode.VelocityChange);
            //human.PlaySound(HumanSounds.FlareLaunch);
        }
    }
}
