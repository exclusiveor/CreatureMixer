using UnityEngine;
using System.Collections;

[System.Serializable]
public struct BoardPos
{
    public int x;
    public int y;

    public BoardPos(int newx, int newy)
    {
        x = newx;
        y = newy;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public bool Equals(BoardPos obj)
    {
        return obj.x == x && obj.y == y;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((BoardPos)obj);
    }

    public static bool operator ==(BoardPos obj1, BoardPos obj2)
    {
        if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        return (obj1.x == obj2.x && obj1.y == obj2.y );
    }
    
    public static bool operator !=(BoardPos obj1, BoardPos obj2)
    {
        return !(obj1 == obj2);
    }

    public static BoardPos operator +(BoardPos obj1, BoardPos obj2)
    {
        return new BoardPos(obj1.x + obj2.x, obj1.y + obj2.y);
    }

    public static BoardPos operator -(BoardPos obj1, BoardPos obj2)
    {
        return new BoardPos(obj1.x - obj2.x, obj1.y - obj2.y);
    }

    public static BoardPos operator /(BoardPos obj1, int divider)
    {
        return new BoardPos(obj1.x / divider, obj1.y / divider);
    }

    public static BoardPos operator *(BoardPos obj1, int divider)
    {
        return new BoardPos(obj1.x * divider, obj1.y * divider);
    }

    public static readonly BoardPos Zero = new BoardPos(0, 0);

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            // Suitable nullity checks etc, of course :)
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }

    public float DistanceTo(BoardPos to)
    {
        return Mathf.Sqrt(Mathf.Pow(x - to.x, 2) + Mathf.Pow(y - to.y, 2));
    }
}