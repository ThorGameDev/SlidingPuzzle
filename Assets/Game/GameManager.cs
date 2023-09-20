using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class GameManager : MonoBehaviour
{
	public GameObject tile;
	private int boardSize;
	public Tile[,] tiles;
	public Vector2Int emptySpot;
	private Vector2Int mousePos;
	public Vector2 boardOffset;
	public Texture2D[] pictures;
	public int choice;
	public int shuffleItterations;
	private Sprite remainigSprite;
	public GameObject intendedPositionIndicator;
	private bool imobilised = true;
    private void Start()
    {
		if(choice == -1)
		{
			choice = UnityEngine.Random.Range(0,pictures.Length);
		}
		boardSize = PlayerPrefs.GetInt("Size", 3) - 1;
		tiles = new Tile[boardSize + 1, boardSize + 1];
		emptySpot = new Vector2Int(0, boardSize);
		boardOffset = new Vector2(-0.5f * boardSize,-0.5f * boardSize);
		mousePos = new Vector2Int();
		float scale = ((float)boardSize / 2) + 0.5f;
		Camera.main.orthographicSize = scale;
		for(int x = 0; x <= boardSize; x++)
		{
			for(int y = 0; y <= boardSize; y++)
			{
				if(x == emptySpot.x && y == emptySpot.y)
				{
					Texture2D tC = pictures[choice];
					float lH = tC.height > tC.width ? tC.width : tC.height;
					Rect r = new Rect(lH / (boardSize + 1) * x,lH / (boardSize + 1) * y, lH / (boardSize + 1), lH / (boardSize + 1));
					remainigSprite = Sprite.Create(tC, r, Vector2.one * 0.5f, lH / (boardSize + 1));
					continue;
				}
				GameObject newTile = Instantiate(tile.gameObject);
				Tile newTileBehavior = newTile.GetComponent<Tile>();
				newTileBehavior.position = new Vector2Int(x,y); 
				newTileBehavior.intendedPosition = newTileBehavior.position;
				newTileBehavior.manager = this;
				tiles[x,y] = newTileBehavior;
				Texture2D textureChoice = pictures[choice];
				float lowerHeight = textureChoice.height > textureChoice.width ? textureChoice.width : textureChoice.height;
				Rect rect = new Rect(lowerHeight / (boardSize + 1) * x,lowerHeight / (boardSize + 1) * y, lowerHeight / (boardSize + 1), lowerHeight / (boardSize + 1));
				newTile.GetComponent<SpriteRenderer>().sprite = Sprite.Create(textureChoice, rect, Vector2.one * 0.5f, lowerHeight / (boardSize + 1));
			}
		}	
		StartCoroutine(ShuffleBoard());
    }
	private IEnumerator ShuffleBoard()
	{
		Vector2Int lastSpot = new Vector2Int(emptySpot.x,emptySpot.y);
		for(int i = shuffleItterations * (int)Mathf.Pow(boardSize, 2.5f); i > 0; i--)
		{
			List<Vector2Int> canadates = new List<Vector2Int>{};
			Vector2Int pos = emptySpot + Vector2Int.right;
			if(pos.x >= 0 && pos.y >= 0 && pos.x <= boardSize && pos.y <= boardSize && pos != lastSpot)
			{
				canadates.Add(pos);
			}
			pos = emptySpot + Vector2Int.left;
			if(pos.x >= 0 && pos.y >= 0 && pos.x <= boardSize && pos.y <= boardSize && pos != lastSpot)
			{
				canadates.Add(pos);
			}
			pos = emptySpot + Vector2Int.up;
			if(pos.x >= 0 && pos.y >= 0 && pos.x <= boardSize && pos.y <= boardSize && pos != lastSpot)
			{
				canadates.Add(pos);
			}
			pos = emptySpot + Vector2Int.down;
			if(pos.x >= 0 && pos.y >= 0 && pos.x <= boardSize && pos.y <= boardSize && pos != lastSpot)
			{
				canadates.Add(pos);
			}
			pos = canadates[Random.Range(0,canadates.Count)];	
			lastSpot = emptySpot;
			tiles[pos.x, pos.y].Move(emptySpot);
			yield return new WaitForSeconds(0.01f);
		}
		imobilised = false;
		yield return null;
	}
	private void VerifyConfiguration()
	{
		bool won = true;
		for(int x = 0; x <= boardSize; x++)
		{
			for(int y = 0; y <= boardSize; y++)
			{
				if(tiles[x, y] == null){ continue; }
				if(tiles[x, y].position != tiles[x, y].intendedPosition)
				{
					won = false;
					break;
				}
			}
			if(won == false)
			{
				break;
			}
		}
		if(won == true)
		{
			imobilised = true;
			GameObject newTile = Instantiate(tile.gameObject);
			Tile newTileBehavior = newTile.GetComponent<Tile>();
			newTileBehavior.position = new Vector2Int(emptySpot.x,emptySpot.y); 
			newTileBehavior.intendedPosition = newTileBehavior.position;
			newTileBehavior.manager = this;
			newTile.GetComponent<SpriteRenderer>().sprite = remainigSprite; 
			
			StartCoroutine(WinLoop());
		}
	}
	private IEnumerator WinLoop()
	{
		while(Input.GetMouseButton(0) == false)
		{
			yield return new WaitForEndOfFrame();
		}
		while(Input.GetMouseButton(0) == true)
		{
			yield return new WaitForEndOfFrame();
		}
		while(Input.GetMouseButton(0) == false)
		{
			yield return new WaitForEndOfFrame();
		}
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
	private void Update()
	{
		if(imobilised == true)
		{
			intendedPositionIndicator.transform.position = new Vector3(0,0,1000000);
			return;
		}
		mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - boardOffset.x);
		mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - boardOffset.y);	
		if(mousePos.x >= 0 && mousePos.y >= 0 && mousePos.x <= boardSize && mousePos.y <= boardSize)
		{
			Tile target = tiles[mousePos.x,mousePos.y];
			if (target != null)
			{
				intendedPositionIndicator.transform.position = new Vector3(target.intendedPosition.x + boardOffset.x, target.intendedPosition.y + boardOffset.y, 0);
				if(Input.GetMouseButtonDown(0))
				{
					target.Shuffle();
					VerifyConfiguration();
				}
			}
			else
			{
				intendedPositionIndicator.transform.position = new Vector3(0,0,1000000);
			}
		}
		else
		{
			intendedPositionIndicator.transform.position = new Vector3(0,0,1000000);
		}
	}
}
