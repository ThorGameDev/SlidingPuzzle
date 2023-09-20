using UnityEngine;
public class Tile : MonoBehaviour
{
	public Vector2Int position;
	public Vector2Int intendedPosition;
	[HideInInspector] public GameManager manager;
	private void Start()
	{
		UpdatePos();
	}
	private void UpdatePos()
	{
		this.transform.position = new Vector2(position.x + manager.boardOffset.x, position.y + manager.boardOffset.y);
	}
	public void Shuffle()
	{
		if(manager.emptySpot.x == position.x + 1 && manager.emptySpot.y == position.y)
		{
			Move(manager.emptySpot);
		}
		else if(manager.emptySpot.x == position.x - 1 && manager.emptySpot.y == position.y)
		{
			Move(manager.emptySpot);
		}
		else if(manager.emptySpot.x == position.x && manager.emptySpot.y == position.y + 1)
		{
			Move(manager.emptySpot);
		}
		else if(manager.emptySpot.x == position.x && manager.emptySpot.y == position.y - 1)
		{
			Move(manager.emptySpot);
		}
	}
	public void Move(Vector2Int pos)
	{
		manager.tiles[position.x,position.y] = null;
		manager.tiles[pos.x, pos.y] = this;
		manager.emptySpot = position;
		position = pos;
		UpdatePos();
	}
}
