using UnityEngine;

namespace Assets.Scripts
{

	namespace DiceyTiles.Scripts
	{
		public class Health : MonoBehaviour
		{
			public OnHealthChangedEventHandler OnHealthChanged;
			public OnDeathEventHandler OnDeath;
			public OnHurtEventHandler OnHurt;

			public delegate void OnHealthChangedEventHandler(float healthDelta);
			public delegate void OnDeathEventHandler();
			public delegate void OnHurtEventHandler(bool fatal);

			[SerializeField] public float maxHP;
			[SerializeField] public float currentHP;
			[SerializeField] public bool isDead;
			[SerializeField] public Transform healthIndicator;

			public float GetHealthPercent() => currentHP / maxHP;

			public bool IsFullHealth() => currentHP >= maxHP;

			public float GetTotalHealth() => currentHP;

			public void Heal(float hp)
			{
				if (IsFullHealth())
					return;

				var delta = AddHP(hp);
			}

			public float SetHP(float hp, bool triggerEvent = true)
			{
				var previousHP = currentHP;

				currentHP = hp;
				currentHP = Mathf.Min(currentHP, maxHP);

				if (triggerEvent)
				{
					OnHealthChanged?.Invoke(currentHP - previousHP);

					if (currentHP <= 0)
					{
						isDead = true;
						OnDeath?.Invoke();
					}
				}

				return currentHP - previousHP;
			}

			private float AddHP(float hp) => SetHP(currentHP + hp);

			public void Damage(float damage)
			{
				var damageReduction = (damage);
				damage -= damageReduction;

				if (damage == 0)
					return;

				if (healthIndicator != null && IsFullHealth())
					healthIndicator.parent.gameObject.SetActive(true);

				AddHP(-damage);

				OnHurt?.Invoke(GetTotalHealth() <= 0);

				healthIndicator.localScale = new Vector3(GetHealthPercent() * 10.0f, 1.0f, 1.0f);
			}

			public void SetMaxHP(int health, bool triggerEvent = true)
			{
				maxHP = health;
				SetHP(maxHP, triggerEvent);
			}
		}
	}
}
