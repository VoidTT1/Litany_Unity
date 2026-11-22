using System.Collections;
using UnityEngine;

public class ErrorAttack : MonoBehaviour
{
	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private GameObject preEnemyPrefab;
	[SerializeField] private int maxEnemy = 10000;
	[SerializeField] private float timeSpawn = 0.3f;
	[SerializeField] private float enemyLifetime = 1f;
	[SerializeField] private float width = 22f;
	[SerializeField] private float height = 10f;
	[SerializeField] private bool isActive = true;

	private float timer;

	private void Start()
	{
		timer = timeSpawn;
		InvokeRepeating("AdjustSpawnParameters", 5000000f, 5000000f);
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			timer = timeSpawn;
			if (base.transform.childCount < maxEnemy)
			{
				StartCoroutine(SpawnWithDelay());
			}
		}
	}

	private IEnumerator SpawnWithDelay()
	{
		Vector2 spawnPosition = new Vector2(Random.Range((0f - width) / 2f, width / 2f), Random.Range((0f - height) / 2f, height / 2f));
		Object.Destroy(Object.Instantiate(preEnemyPrefab, spawnPosition, Quaternion.identity, base.transform), 2f);
		yield return new WaitForSeconds(2f);
		Object.Destroy(Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, base.transform), enemyLifetime);
	}

	private void AdjustSpawnParameters()
	{
		timeSpawn = Mathf.Max(0.3f, timeSpawn - 0.2f);
		enemyLifetime += 0.5f;
	}
}
