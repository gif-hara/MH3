using MH3.ActorControllers;
using UnityEngine;

namespace MH3
{
    public static partial class Extensions
    {
        public static int GetAttackLayer(this Actor actor)
        {
            return (Layer)actor.gameObject.layer switch
            {
                Layer.ActorPlayer => (int)Layer.ActorPlayerAttack,
                Layer.ActorEnemy => (int)Layer.ActorEnemyAttack,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
