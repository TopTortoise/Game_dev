using UnityEngine;
public class WilsonList
{
  public class Node
  {
    public Vector3Int Value;
    public Node Next;

    public Node(Vector3Int value)
    {
      Value = value;
      Next = null;
    }
  }

  public Node Head;
  public Node Tail;
  public int size;

  public WilsonList()
  {
    Head = null;
    Tail = null;
    size = 0;
  }
  public bool Add(Vector3Int val)
  {
    //check if already exist
    if(kill(val)){
      return false;
    }

    Node newNode = new Node(val);
    if (size == 0)
    {
      Head = newNode;
      Tail = newNode;
    }
    else
    {
      Tail.Next = newNode;
      Tail = newNode;
    }
    size++;
    return true;
  }


  public bool kill(Vector3Int val)
  {
    Node curr = Head;
    Node prev = Head;
    while (curr != null && !curr.Value.Equals(val))
    {
      prev = curr;
      curr = curr.Next;
    }

    if (curr == null) return false;
    int diff = 0;
    while(curr != null){
      diff++;
      curr = curr.Next;
    }
    size -= diff;
    prev.Next = null;
    Tail = prev;

    return true;
  }


}
