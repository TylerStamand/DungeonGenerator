using System.Collections.Generic;

public class RoomNode {

    public RoomNode(Room room) {
        Room = room;
        // Head = head;
        // Left = left;
        // Right = right;

        NorthFrontier = new List<RoomNode>();
        SouthFrontier = new List<RoomNode>();
        EastFrontier = new List<RoomNode>();
        WestFrontier = new List<RoomNode>();
    }

    public RoomNode Head;
    public RoomNode Left;
    public RoomNode Right;

    public Room Room;


    public List<RoomNode> NorthFrontier;
    public List<RoomNode> SouthFrontier;
    public List<RoomNode> EastFrontier;
    public List<RoomNode> WestFrontier;

    public bool IsLeaf => (Left == null && Right == null);

    

}