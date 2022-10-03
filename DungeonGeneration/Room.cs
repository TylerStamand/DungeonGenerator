using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
public class Room : IEquatable<Room> {

    
    public readonly int OriginalXPosition;
    public readonly int OriginalYPosition;
    public readonly int OriginalWidth;
    public readonly int OriginalHeight;

    public List<Room> North;
    public List<Room> South;
    public List<Room> East;
    public List<Room> West;

    public int XPosition;
    public int YPosition;
    public int Width;
    public int Height;

    public int Area => Width * Height;
    public Vector2Int Center => new Vector2Int(XPosition + Width/2, YPosition + Height/2 );
    
    public Room(int xPosition, int yPosition, int width, int height) {
        XPosition = xPosition;
        YPosition = yPosition;
        Width = width;
        Height = height;

        OriginalXPosition = xPosition;
        OriginalYPosition = yPosition;
        OriginalWidth = width;
        OriginalHeight = height;

        North = new List<Room>();
        South = new List<Room>();
        East = new List<Room>();
        West = new List<Room>();
    }

    public Room(Room room) {
        XPosition = room.XPosition;
        YPosition = room.YPosition;
        Width = room.Width;
        Height = room.Height;

        OriginalXPosition = room.OriginalXPosition;
        OriginalYPosition = room.OriginalYPosition;
        OriginalWidth = room.OriginalWidth;
        OriginalHeight = room.OriginalHeight;


        North = new List<Room>();
        foreach(Room adjRoom in room.North) {
            North.Add(new Room(adjRoom));
        }
        East = new List<Room>();
        foreach(Room adjRoom in room.East) {
            East.Add(new Room(adjRoom));
        }

        West = new List<Room>();
        foreach (Room adjRoom in room.West) {
            West.Add(new Room(adjRoom));
        }

        South = new List<Room>();
        foreach (Room adjRoom in room.South) {
            South.Add(new Room(adjRoom));
        }

        
    }


    public List<Room> GetAdjacentRooms() {
        List<Room> rooms =  new List<Room>();
        rooms.AddRange(North);
        rooms.AddRange(South);
        rooms.AddRange(East);
        rooms.AddRange(West);
        return rooms;
    }

    

    public bool Equals(Room other) {
        if(XPosition == other.XPosition &&
            YPosition == other.YPosition &&
            Width == other.Width &&
            Height == other.Height) {
                return true;
        }
        return false;
    }

    public override bool Equals(object obj) {
        return Equals(obj as Room);
    }

    
}

