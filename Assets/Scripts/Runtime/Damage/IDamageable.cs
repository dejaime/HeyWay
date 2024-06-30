using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HayWay.Runtime.Components
{
    public interface IDamageable
    {
        public void TakeDamage(int value);
        public void RestoreHealth(int value);
        public void RestoreFullHealth();
        public int GetHealth();
        public int GetMaxHealth();

    }
}