using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{
	[SerializeField] private bool isActive;
	[SerializeField] private Player player;
	[SerializeField] private Image[] hearts;
	[SerializeField] private Image[] emptyHearts;
	private bool previousIsActive;
	[SerializeField] private GameObject endScren;
	[SerializeField] private AudioController audioToStop;
	[SerializeField] private AudioController audioToPlay;

	private void Start()
	{
		if (isActive)
		{
			UpdateHealthUI();
		}
		previousIsActive = isActive;
	}

	private void Update()
	{
		if (isActive)
		{
			UpdateHealthUI();
		}
		if (!previousIsActive && isActive)
		{
			UpdateHealthUI();
		}
		previousIsActive = isActive;
	}

	private void UpdateHealthUI()
	{
		if (!endScren.activeSelf)
		{
			if (audioToStop != null)
			{
				audioToStop.FadeToAudio(enable: false);
			}
			if (audioToPlay != null)
			{
				audioToPlay.FadeToAudio(enable: true);
			}
			Time.timeScale = 0f;
			endScren.SetActive(value: true);
		}
		if (player == null)
		{
			return;
		}
		int num = Mathf.FloorToInt(player.health);
		if (num >= 3)
		{
			for (int i = 0; i < hearts.Length; i++)
			{
				hearts[i].enabled = true;
				emptyHearts[i].enabled = false;
			}
			return;
		}
		switch (num)
		{
		case 2:
		{
			Image obj = hearts[0];
			bool flag = (hearts[1].enabled = true);
			obj.enabled = flag;
			hearts[2].enabled = false;
			Image obj2 = emptyHearts[0];
			flag = (emptyHearts[1].enabled = false);
			obj2.enabled = flag;
			emptyHearts[2].enabled = true;
			break;
		}
		case 1:
		{
			hearts[0].enabled = true;
			Image obj3 = hearts[1];
			bool flag = (hearts[2].enabled = false);
			obj3.enabled = flag;
			emptyHearts[0].enabled = false;
			Image obj4 = emptyHearts[1];
			flag = (emptyHearts[2].enabled = true);
			obj4.enabled = flag;
			break;
		}
		default:
		{
			for (int j = 0; j < hearts.Length; j++)
			{
				hearts[j].enabled = false;
				emptyHearts[j].enabled = true;
			}
			break;
		}
		}
	}
}
