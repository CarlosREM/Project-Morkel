using UnityEngine;
using System;

public class CharacterHealth : MonoBehaviour
{
   [SerializeField] private int maxHealth;
   public int MaxHealth => maxHealth;
   
   public int CurrentHealth { get; private set; }
   
   public Action<int> OnHurt;
   public Action<int> OnHeal;
   public Action OnDeath;

   public bool IsDead => CurrentHealth <= 0;
   public bool IsFull => CurrentHealth >= maxHealth;
   
   private void Start()
   {
      CurrentHealth = maxHealth;
   }

   public void Hurt(int amount)
   { 
      CurrentHealth -= amount;
      
      if (CurrentHealth <= 0)
      {
         CurrentHealth = 0;
         OnDeath?.Invoke();
      }
      else
      {
         OnHurt?.Invoke(amount);
      }
   }

   public void Heal(int amount)
   {
      CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
      OnHeal?.Invoke(amount);
   }
}
