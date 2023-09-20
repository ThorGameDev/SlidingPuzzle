using System.Collections;
using UnityEngine;
public class StartManager : MonoBehaviour
{
	private bool inAction;
	private Vector3 pos;
	public GameObject title;
	public float amplitude;
	public float frequency;

	public float extendTime;
	public Vector3 inPos;
	public Vector3 outPos;
    private void Start()
    {
		pos = title.transform.position;
    }
    private void Update()
    {
		title.transform.position = pos + new Vector3(0, Mathf.Sin(Time.time * frequency) * Screen.height * amplitude);
    }
	public void PlayGame()
	{
		if(inAction) { return; }
		inAction = true;
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
		Destroy(this.gameObject);
	}
	public void QuitGame()
	{
		if(inAction) { return; }
		inAction = true;
		Debug.Log("Quit");
		Application.Quit();
		Destroy(this.gameObject);
	}
	public void ShowPannel(RectTransform pannel)
	{
		if(inAction) { return; }
		StartCoroutine(Move(pannel, outPos));	
	}
	public void HidePannel(RectTransform pannel)
	{
		if(inAction) { return; }
		StartCoroutine(Move(pannel, inPos));
	}
	private IEnumerator Move(RectTransform moveTarget, Vector3 end)
	{
		if(inAction)
		{
			yield break; 
		} 
		yield return new WaitForEndOfFrame();
		inAction = true;
		Vector3 startPos = moveTarget.anchoredPosition;
		Vector3 point = new Vector3();
		for(float t = 0; t <= extendTime; t += Time.deltaTime)
		{
			point.x = Mathf.Lerp(startPos.x, end.x, t);
			point.y = Mathf.Lerp(startPos.y, end.y, t);
			point.z = Mathf.Lerp(startPos.z, end.z, t);
			moveTarget.anchoredPosition = point; 
			yield return new WaitForEndOfFrame();
		}
		moveTarget.anchoredPosition = end;
		inAction = false;
	}
}
