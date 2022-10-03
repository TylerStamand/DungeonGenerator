using System;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : IEquatable<Vertex> {
    public Room Room;
    public Vertex Parent;
    
    public Vertex(Room room, Vertex parent = null) {
        Room = room;
        Parent = parent;
    }

    public bool Equals(Vertex other) {
        return Room.Equals(other.Room);
    }

    public override bool Equals(object obj) {
        return Equals(obj as Vertex);
    }
}
public class Edge : IEquatable<Edge>, IComparable<Edge> {
    
    public Edge(Vertex to, Vertex from, int distance) {
        To = to;
        From = from;
        Distance = distance;
    }

    public Vertex To;
    public Vertex From;
    public int Distance;


    public bool Equals(Edge otherEdge) {

        return (To.Room.Equals( otherEdge.To.Room) && From.Room.Equals( otherEdge.From.Room)) || (To.Room.Equals(otherEdge.From.Room) && From.Room.Equals(otherEdge.To.Room));
    }

    public override bool Equals(object obj) {
        return Equals(obj as Edge);
    }
    

    public int CompareTo(Edge other) {
        return Distance - other.Distance;
    }
}

public class Graph {
    public List<Vertex> Vertices = new List<Vertex>();
    public List<Edge> Edges = new List<Edge>();

    Vertex Find(Vertex vertex) {
        if(vertex.Parent == null) {
            return vertex;
        }
        return Find(vertex.Parent);
    }

    void Union(Vertex x, Vertex y) {
        x.Parent = y;
    }

    public bool IsCycle() {

        foreach (Vertex vertex in Vertices) {
            vertex.Parent = null;
        }

        foreach (Edge edge in Edges) {
            Vertex x = Find(edge.To);
            Vertex y = Find(edge.From);

            if(x == y) {
                return true;
            }


            Union(x, y);

        }
        return false;
    }

}


public class EdgeSelector {
    
    public static List<Edge> GetEdges(List<Room> rooms ) {
        EdgeSelector edgeSelector = new EdgeSelector(rooms);
        return edgeSelector.graph.Edges;
    }

    List<Room> rooms;
    List<Edge> sortedEdges;
    List<Vertex> vertices;
    Graph graph;
    EdgeSelector(List<Room> rooms) {
        this.rooms = rooms;
        graph = new Graph();
        vertices = new List<Vertex>();
        sortedEdges = new List<Edge>();

        foreach (Room room in rooms) { 
            vertices.Add(new Vertex(room));
        }
        graph.Vertices = vertices;

        SetSortedEdges();
        FindMST();
    }

    void FindMST() {
        foreach(Edge edge in sortedEdges) {
            graph.Edges.Add(edge);
            if(graph.IsCycle()) {
                graph.Edges.Remove(edge);
            }
            if(graph.Edges.Count == rooms.Count - 1) {
                return;
            }
        }
    }

    void SetSortedEdges() {
        foreach (Room room in rooms) {
            
           Vertex x = GetVertexOfRoom(room);
            foreach (Room adjacentRoom in room.GetAdjacentRooms()) {
                Vertex y = GetVertexOfRoom(adjacentRoom);
                Edge newEdge = new Edge(x, y, CalculateEdgeDistance(room, adjacentRoom));

                //This checks for backwards paths from other rooms
                if (!sortedEdges.Contains(newEdge)) {
                  
                    sortedEdges.Add(newEdge);

                }
            }
        }
        sortedEdges.Sort();
    }

    Vertex GetVertexOfRoom(Room room)
    {
        foreach (Vertex vertex in vertices)
        {
            if (vertex.Room == room)
            {
                return vertex;
            }
        }
        return null;
    }

    int CalculateEdgeDistance(Room room1, Room room2) {
        int XDistance = (int)room1.Center.x - (int)room2.Center.x;
        XDistance = Math.Abs(XDistance);
        int YDistance = (int)room1.Center.y - (int)room2.Center.y;
        YDistance = Math.Abs(YDistance);
        return XDistance + YDistance;
    }


}