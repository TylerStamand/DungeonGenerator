using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;


/// <summary>
/// Takes a list of square rooms and makes them organic looking
/// </summary>
public class Automaton {

    
    public static bool[,] GenerateMap(GenerationOptions options, List<Room> rooms, List<Edge> edges) {
        Automaton generator = new Automaton(options, rooms, edges);
        generator.InitializeMap();
        
        for (int i = 0; i < options.NumberOfSteps; i++) {
            generator.SimulationStep();
        }
        return ConvertMapToBool(generator.cellMap);
    }

    static bool[,] ConvertMapToBool(CellState[,] cellMap) {
        bool[,] boolMap = new bool[cellMap.GetLength(0), cellMap.GetLength(1)];
        for(int x = 0; x < cellMap.GetLength(0); x++) {
            for(int y = 0; y < cellMap.GetLength(1); y++) {
                if(cellMap[x,y] == CellState.Alive || cellMap[x,y] == CellState.AlwaysAlive) {
                    boolMap[x,y] = true;
                }
                else {
                    boolMap[x, y] = false;
                }
            }
        }
        return boolMap;
    }

    int width, height, deathLimit, birthLimit, distanceFromRoomBoundaries, distanceFromPathBoundaries;
    
    float initialChance; 

    CellState[,] cellMap;

    List<Room> rooms;
    List<Edge> edges;
    Random random;

    Automaton(GenerationOptions options, List<Room> rooms, List<Edge> edges) {
        this.rooms = rooms;
        this.edges = edges;
        random = new Random(options.Seed);
        this.distanceFromRoomBoundaries = options.DistanceFromRoomBoundaries;
        this.distanceFromPathBoundaries = options.DistanceFromPathBoundaries;
        this.deathLimit = options.DeathLimit;   
        this.birthLimit = options.BirthLimit;
        this.initialChance = options.InitialChance01;
        this.width = options.Width;
        this.height = options.Height;
    
        cellMap = new CellState[width, height];
        

    }





    void InitializeMap() {
      
        //Set alive/dead cells a certain distance from room size
        foreach (Room room in rooms) {
            int xStart = room.XPosition - distanceFromRoomBoundaries;
            int yStart = room.YPosition - distanceFromRoomBoundaries;
            int xEnd = room.XPosition + distanceFromRoomBoundaries;
            int yEnd = room.YPosition + distanceFromRoomBoundaries;

            //Makes sure room cells stay within original boundaries
            if(xStart < room.OriginalXPosition || xStart < 0) 
                xStart = room.OriginalXPosition;
            if(yStart < room.OriginalYPosition || yStart < 0)
                yStart = room.OriginalYPosition;
            if(xEnd > room.OriginalXPosition + room.OriginalWidth || xStart > width)
                xEnd = room.OriginalXPosition + room.OriginalWidth;
            if(yEnd > room.OriginalYPosition + room.OriginalHeight || yStart > height) 
                yEnd = room.OriginalYPosition + room.OriginalHeight;

            for(int x = xStart; x < room.XPosition + room.Width+ distanceFromRoomBoundaries && x < width; x++) {
                for (int y = yStart; y < room.YPosition + room.Height+ distanceFromRoomBoundaries && y < width; y++) {
                    if(random.NextDouble() > initialChance) {
                        cellMap[x,y] = CellState.Alive;
                    }
                    else {
                        cellMap[x, y] = CellState.Dead;
                    }
                }
            }

        }

        //Set outer border of each room to always dead
        foreach(Room room in rooms) {
            int x;
            int y;
            
            y = room.OriginalYPosition;
            for(x = room.OriginalXPosition; x < room.OriginalXPosition + room.OriginalWidth; x++) {
                cellMap[x,y] = CellState.AlwaysDead;
            }
            y = room.OriginalYPosition + room.OriginalHeight - 1;
            for (x = room.OriginalXPosition; x < room.OriginalXPosition + room.OriginalWidth; x++) {
                cellMap[x, y] = CellState.AlwaysDead;
            }

            x = room.OriginalXPosition;
            for (y = room.OriginalYPosition; y < room.OriginalYPosition + room.OriginalHeight; y++) {
                cellMap[x, y] = CellState.AlwaysDead;
            }


            x = room.OriginalXPosition + room.OriginalWidth - 1;
            for (y = room.OriginalYPosition; y < room.OriginalYPosition + room.OriginalHeight; y++) {
                cellMap[x, y] = CellState.AlwaysDead;
            }
        }

        //Set alive/dead cells a certain distance from path
        //Set actual path to always alive

        foreach (Edge edge in edges) {


            int currentX = (int)edge.From.Room.Center.x;
            int currentY = (int)edge.From.Room.Center.y;
            int endX = (int)edge.To.Room.Center.x;
            int endY = (int)edge.To.Room.Center.y;

            int xIncrement = currentX < endX ? 1 : -1;
            int yIncrement = currentY < endY ? 1 : -1;



            if (random.NextDouble() > .5) {

                while (currentX != endX) {
                    currentX += xIncrement;
                    for(int i = -distanceFromPathBoundaries; i < distanceFromPathBoundaries; i++ ) {
                        if (cellMap[currentX, currentY] == CellState.AlwaysAlive) continue;
                        if (i == 0) {
                            cellMap[currentX, currentY] = CellState.AlwaysAlive;
                        }
                        else {
                            if(currentY + i < height && currentY + i > 0)
                                cellMap[currentX, currentY + i] = random.NextDouble() > initialChance ? CellState.Alive : CellState.Dead ;  
                        } 
                    }
                }
                while (currentY != endY) {
                    currentY += yIncrement;
                    for (int i = -distanceFromPathBoundaries; i < distanceFromPathBoundaries; i++) {
                        if (cellMap[currentX, currentY] == CellState.AlwaysAlive) continue;
                        if (i == 0) {
                            cellMap[currentX, currentY] = CellState.AlwaysAlive;
                        }
                        else {
                            if(currentX + i < width && currentX + i > 0)
                                cellMap[currentX + i, currentY] = random.NextDouble() > initialChance ? CellState.Alive : CellState.Dead;

                        }

                    }

                }
            }

            //Do it backwards
            else {
                while (currentY != endY) {
                    currentY += yIncrement;
                    for (int i = -distanceFromPathBoundaries; i < distanceFromPathBoundaries; i++) {
                        if (cellMap[currentX, currentY] == CellState.AlwaysAlive) continue;
                        if (i == 0) {
                            cellMap[currentX, currentY] = CellState.AlwaysAlive;
                        }
                        else {
                            if (currentX + i < width && currentX + i > 0)
                                cellMap[currentX + i, currentY] = random.NextDouble() > initialChance ? CellState.Alive : CellState.Dead;
                        }

                    }

                }
                while (currentX != endX) {
                    currentX += xIncrement;
                    for (int i = -distanceFromPathBoundaries; i < distanceFromPathBoundaries; i++) {
                        if (cellMap[currentX, currentY] == CellState.AlwaysAlive) continue;
                        if (i == 0) {
                            cellMap[currentX, currentY] = CellState.AlwaysAlive;
                        }
                        else {
                            if (currentY + i < height && currentY + i > 0)
                                cellMap[currentX, currentY + i] = random.NextDouble() > initialChance ? CellState.Alive : CellState.Dead;
                        }
                    }
                }

            }
        }



        

        //Set rooms to always alive

        foreach(Room room in rooms) {
            for(int x = room.XPosition; x < room.XPosition + room.Width; x++) {
                for(int y = room.YPosition; y < room.YPosition + room.Height; y++) {
                    cellMap[x, y] = CellState.AlwaysAlive;
                }
            }
        }


        //Set outer border of board to always dead
       
        for (int x = 0; x < width; x++) {
            cellMap[x, 0] = CellState.AlwaysDead;
            cellMap[x, height-1] = CellState.AlwaysDead;
        }
          
        for (int y = 0; y < height; y++) {
            cellMap[0, y] = CellState.AlwaysDead;
            cellMap[width-1, y] = CellState.AlwaysDead;
        }
    
    }


    void SimulationStep() {
        CellState[,] newMap = new CellState[cellMap.GetLength(0), cellMap.GetLength(1)];
        Array.Copy(cellMap, newMap, cellMap.GetLength(0) * cellMap.GetLength(1));


        for (int x = 0; x < cellMap.GetLength(0); x++) {
            for (int y = 0; y < cellMap.GetLength(1); y++) {

                //Skip cells that dont change
                if(cellMap[x,y] == CellState.AlwaysAlive || cellMap[x,y] == CellState.AlwaysDead)
                    continue;

                int neighbors = CountAliveNeighbors(cellMap, x, y);

                if(cellMap[x,y] == CellState.Alive) {
                    if(neighbors < deathLimit) {
                        newMap[x,y] = CellState.Dead;
                    }
                    else {
                        newMap[x,y] = CellState.Alive;
                    }
                }
                else if(cellMap[x,y] == CellState.Dead) {
                    if(neighbors > birthLimit) {
                        newMap[x,y] = CellState.Alive;
                    }
                    else {
                        newMap[x, y] = CellState.Dead;
                    }
                }
            }
        }

        Array.Copy(newMap, cellMap, cellMap.GetLength(0) * cellMap.GetLength(1));

    }


    int CountAliveNeighbors(CellState[,] map,  int x, int y) {
        int count = 0;
        for(int i = -1; i < 2; i++) {
            for(int j = -1; j < 2; j++) {
                int neighborX = x+i;
                int neighborY = y+j;

                //If looking at the middle point
                if(i == 0 && j == 0) {
                   continue;
                }
                else if(neighborX < 0 || neighborY < 0 || neighborX >= width || neighborY >= height) {
                    count += 1;
                }
                else if(map[neighborX,neighborY] == CellState.Alive || map[neighborX, neighborY] == CellState.AlwaysAlive) {
                    count += 1;
                }
            } 
        }

        return count;
    }
 
}

enum CellState {
    Dead,
    Alive,
    AlwaysDead,
    AlwaysAlive
}

public struct GenerationOptions {
    public int Seed;
    public int DistanceFromRoomBoundaries;
    public int DistanceFromPathBoundaries;
    public int DeathLimit;
    public int BirthLimit; 
    public int Width;
    public int Height;
    public float InitialChance01;
    public int NumberOfSteps;
}

