using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public int numberOfHearts = 3;
	public float health = 3f;

	[SerializeField] private Image[] hearts;
	[SerializeField] private Sprite HeartSprite;
	[SerializeField] private Sprite NonHeartSprite;
	[SerializeField] private GameObject player;

	public bool useOffSpecial; // Специальный режим и регинерация

	public bool isRegeneratingHealth;

	private Coroutine regenCoroutine;
	public TextMeshProUGUI regenTimerText;


	public float calculatedSpeed;
	private bool isInvulnerable;
	private Vector2 lastPosition;
	private bool inDamageWhenStationaryZone;
	private bool inDamageWhenMovingZone;
	private float damageTimer;
	private float damageInterval = 1f;
	private float movementThreshold = 0.1f;

	private void Start()
	{
		LoadSettings();
		ResetHealth();
		UpdateHealthDisplay();
		if (regenTimerText != null)
		{
			regenTimerText.gameObject.SetActive(value: false);
		}
		lastPosition = base.transform.position;
		if (useOffSpecial)
		{
			ApplySpecialMode();
		}
	}

	private void Update()
	{
		calculatedSpeed = Vector2.Distance(base.transform.position, lastPosition) / Time.deltaTime;
		lastPosition = base.transform.position;
		if (health <= 0f)
		{
			Time.timeScale = 0f;
		}
		if (inDamageWhenStationaryZone && calculatedSpeed <= movementThreshold)
		{
			damageTimer += Time.deltaTime;
			if (damageTimer >= damageInterval)
			{
				TakeDamage(1);
				damageTimer = 0f;
			}
		}
		else if (inDamageWhenMovingZone && calculatedSpeed > movementThreshold)
		{
			damageTimer += Time.deltaTime;
			if (damageTimer >= damageInterval)
			{
				TakeDamage(1);
				damageTimer = 0f;
			}
		}
		else
		{
			damageTimer = 0f;
		}
	}

	public void TakeDamage(int damage)
	{
		if (!isInvulnerable)
		{
			float num = health;
			health -= damage;
			health = Mathf.Max(health, 0f);
			UpdateHealthDisplay();
			if (isRegeneratingHealth && health < num && health < (float)numberOfHearts && regenCoroutine == null)
			{
				regenTimerText.gameObject.SetActive(value: true);
				StartRegen();
			}
			StartCoroutine(TemporaryInvulnerability(1.5f));
		}
	}

	private IEnumerator TemporaryInvulnerability(float duration)
	{
		isInvulnerable = true;
		Collider2D col = GetComponent<Collider2D>();
		if (col != null)
		{
			col.enabled = false;
		}
		yield return new WaitForSeconds(duration);
		if (col != null)
		{
			col.enabled = true;
		}
		isInvulnerable = false;
	}

	private IEnumerator RegenerateHealth()
	{
		for (float timer = 30f; timer > 0f; timer--)
		{
			regenTimerText.text = "Регенерация: " + Mathf.Ceil(timer);
			yield return new WaitForSeconds(1f);
		}
		if (health < (float)numberOfHearts)
		{
			health++;
			SaveHealth();
			UpdateHealthDisplay();
		}
		regenTimerText.gameObject.SetActive(value: false);
		regenCoroutine = null;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.isTrigger)
		{
			return;
		}
		float num = 0.1f;
		if (other.CompareTag("DamageWhenMoving"))
		{
			if (calculatedSpeed > num)
			{
				TakeDamage(1);
			}
		}
		else if (other.CompareTag("DamageWhenStationary"))
		{
			if (calculatedSpeed <= num)
			{
				TakeDamage(1);
			}
		}
		else
		{
			TakeDamage(1);
		}
	}

	private void UpdateHealthDisplay()
	{
		for (int i = 0; i < hearts.Length; i++)
		{
			hearts[i].sprite = ((i < Mathf.RoundToInt(health)) ? HeartSprite : NonHeartSprite);
			hearts[i].enabled = i < numberOfHearts;
		}
	}

	public void ApplySpecialMode()
	{
		numberOfHearts = 6;
		health = 6f;
		SaveHealth();
		UpdateHealthDisplay();
	}

	public void RemoveSpecialMode()
	{
		numberOfHearts = 3;
		health = Mathf.Min(health, numberOfHearts);
		SaveHealth();
		UpdateHealthDisplay();
	}

	public void SaveHealth()
	{
		PlayerPrefs.SetInt("NumberOfHearts", numberOfHearts);
		PlayerPrefs.SetFloat("Health", health);
		PlayerPrefs.SetInt("UseOffSpecial", useOffSpecial ? 1 : 0);
		PlayerPrefs.SetInt("IsRegeneratingHealth", isRegeneratingHealth ? 1 : 0);
		PlayerPrefs.Save();
	}

	public void LoadSettings()
	{
		if (PlayerPrefs.HasKey("UseOffSpecial"))
		{
			useOffSpecial = PlayerPrefs.GetInt("UseOffSpecial") == 1;
		}
		if (PlayerPrefs.HasKey("IsRegeneratingHealth"))
		{
			isRegeneratingHealth = PlayerPrefs.GetInt("IsRegeneratingHealth") == 1;
		}
	}

	public void ToggRegen(bool enable)
	{
		isRegeneratingHealth = enable;
		SaveHealth();
		if (!enable)
		{
			StopRegen();
		}
	}

	private void StartRegen()
	{
		if (regenCoroutine == null)
		{
			regenCoroutine = StartCoroutine(RegenerateHealth());
		}
	}

	private void StopRegen()
	{
		if (regenCoroutine != null)
		{
			StopCoroutine(regenCoroutine);
			regenCoroutine = null;
		}
		regenTimerText.gameObject.SetActive(value: false);
	}

	private void ResetHealth()
	{
		health = numberOfHearts;
	}

	public void FullResetPlayer()
	{
		health = numberOfHearts;
		UpdateHealthDisplay();
		PlayerPrefs.SetFloat("Health", health);
		PlayerPrefs.Save();
	}
}
