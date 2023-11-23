using startrek;
using System;
using System.Reflection;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;

public class starTrek
{
    public static int quadX, quadY;
    public static int cellX, cellY;
    public static int[,,,] grid = new int[8, 8, 8, 8];
    static int score=0;
    static int shieldStrength=5000;
    static int inventory = 1000;
    static int moveCount = 0;
    static int numKlingons = 0;
    static int energy = 5000;
    public static void Main(String[] args)
    {
        bool shipDead = false;
        populateGrid();
        //Console.WriteLine("grid is populated.");
        placeKlingons();
        //Console.WriteLine("klingons are placed...");
        placeBases();
        placeShip();
        intro();
        while (!shipDead)
        {
            displayQuad();
            String choice = getCommand();
            choice = choice.ToUpper();
            switch (choice)
            {
                case "NAV":
                    {
                        nav();
                        energy -= 20;
                        break;
                    }
                case "MAP":
                    {
                        drawMap();
                        break;
                    }

                case "PHA": {
                        if (getKlingonCount(quadX, quadY) > 0)
                        {
                            Console.WriteLine($"How many phasors do you need? You have {inventory} available: ");
                            int pha = Int32.Parse(Console.ReadLine());

                            if (inventory >= pha)
                            {
                                inventory -= pha;
                                killKlingons(pha, quadX, quadY);
                                shieldStrength -= 100 * pha;
                            }
                            else Console.WriteLine("Not enough inventory.");
                        }
                        else Console.WriteLine(" No klingons in the quadrant.");
                        break;
                    }

                case "SHI": {
                        Console.WriteLine("Time to replenish your shields. How much energy do you need?");
                        int newEnergy = Int32.Parse(Console.ReadLine());
                        if (newEnergy > energy) Console.WriteLine("You don't have enough energy available.");
                        else {
                            energy -= newEnergy;
                            shieldStrength += newEnergy;
                            Console.WriteLine("energy transferred to shields.");
                        }
                        
                        
                        break;
                    }

                case "EXIT": {
                        return;
                    }

                case "HLP": {
                        Console.WriteLine("List of commands: ");
                        Console.WriteLine("NAV: move to another quadrant, or within the current.");
                        Console.WriteLine("MAP: pull a map of klingons in the galaxy.");
                        Console.WriteLine("PHA: shoot some klingons");
                        Console.WriteLine("SHI: replenish shields from energy bank.");
                        Console.WriteLine("EXIT: end the game.");
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            moveCount++;
            if (moveCount == Cell.MAXMOVES && numKlingons > 0)
            {
                Console.WriteLine("game over, you have made too many moves. ");
                return;
            }
            else if (numKlingons == 0) {
                Console.WriteLine("you have killed all the klingons.");
                return;
            }
        }

    }

    public static void intro() {
        Console.WriteLine("You are fighting klingons. ");
    }
    public static void displayQuad()
    {
        Console.WriteLine("*****************************************************************************");
        Console.WriteLine($"You are in quadrant {quadX},{quadY}:");
        Console.WriteLine($"Your shield strength is: {shieldStrength}");
        Console.WriteLine($"inventory: {inventory}");
        Console.WriteLine($"moves done: {moveCount}");
        Console.WriteLine($"number of klingons left: {numKlingons}");
        Console.WriteLine($"energy: {energy}");
        try
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    switch (grid[quadX, quadY, x, y])
                    {
                        case Cell.STAR:
                            {
                                Console.Write(" * ");
                                break;
                            }
                        case Cell.EMPTY:
                            {
                                Console.Write(" . ");
                                break;
                            }
                        case Cell.BASE: {
                                Console.Write(" B ");
                                break;
                            }
                        case Cell.SHIP:
                            {
                                Console.Write(" E ");
                                break;
                            }
                        case Cell.KLINGON:
                            {
                                Console.Write("<K>");
                                break;
                            }
                    }
                }
                Console.WriteLine();
            }
        }
        catch (Exception e) {
            Console.WriteLine("location is out of bounds...");
        }
    }
    public static String getCommand()
    {
        Console.Write("Command: ");
        return Console.ReadLine();
    }
    public static void nav()
    {
        try
        {
            Console.WriteLine("Enter a bearing <0..7>:");
            int bearing = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Enter a warp factor<1..8>: ");
            double warp = Double.Parse(Console.ReadLine());
            bool canWe = true;
            switch (bearing)
            {
                case 0:
                    {
                        //north
                        int tempX = 0;
                        int tempY = (int)(-8 * warp);
                        int xoff = 0;
                        int yoff = tempY % 8;
                        int xquad = 0;
                        int yquad = tempY / 8;

                        if (blockCheck(quadX, quadY, cellX, cellY - 1) == Cell.STAR) canWe = false;
                        else canWe = true;
                        quadY += yquad;
                        cellY += yoff;
                        break;
                    }
                case 1:
                    {
                        //northeast
                        int tempX = (int)(8 * warp);
                        int tempY = (int)(-8 * warp);
                        int xoff = 1 * tempX % 8; ;
                        int yoff = 1 * tempY % 8;
                        int xquad = tempX / 8;
                        int yquad = 1 * tempY / 8;

                        if (blockCheck(quadX, quadY, cellX + 1, cellY - 1) == Cell.STAR) canWe = false;
                        else canWe = true;
                        quadY += yquad;
                        cellY += yoff;
                        quadX += xquad;
                        cellX += xoff;
                        break;
                    }
                case 2:
                    {
                        //east
                        int tempX = (int)(8 * warp);
                        int tempY = 0;
                        int xoff = 1 * tempX % 8; ;
                        int yoff = 0;
                        int xquad = tempX / 8;
                        int yquad = 0;

                        if (blockCheck(quadX, quadY, cellX + 1, cellY) == Cell.STAR) canWe = false;
                        else canWe = true;
                        quadY += yquad;
                        cellY += yoff;
                        quadX += xquad;
                        cellX += xoff;
                        break;
                    }

                case 3:
                    {
                        //southeast
                        int tempX = (int)(8 * warp);
                        int tempY = (int)(8 * warp);
                        int xoff = 1 * tempX % 8; ;
                        int yoff = 1 * tempY % 8;
                        int xquad = tempX / 8;
                        int yquad = 1 * tempY / 8;

                        if (blockCheck(quadX, quadY, cellX + 1, cellY + 1) == Cell.STAR) canWe = false;
                        else canWe = true;
                        quadY += yquad;
                        cellY += yoff;
                        quadX += xquad;
                        cellX += xoff;
                        break;
                    }
                case 4:
                    {
                        //south
                        int tempX = 0;
                        int tempY = (int)(8 * warp);
                        int xoff = 0; ;
                        int yoff = 1 * tempY % 8;
                        int xquad = 0;
                        int yquad = 1 * tempY / 8;

                        if (blockCheck(quadX, quadY, cellX, cellY + 1) == Cell.STAR) canWe = false;
                        else canWe = true;
                        quadY += yquad;
                        cellY += yoff;
                        quadX += xquad;
                        cellX += xoff;
                        break;
                    }
                case 5:
                    {
                        //southwest
                        int tempX = (int)(-8 * warp);
                        int tempY = (int)(8 * warp);
                        int xoff = 1 * tempX % 8; ;
                        int yoff = 1 * tempY % 8;
                        int xquad = 1 * tempX / 8;
                        int yquad = 1 * tempY / 8;

                        if (blockCheck(quadX, quadY, cellX - 1, cellY + 1) == Cell.STAR) canWe = false;
                        else canWe = true;
                        quadY += yquad;
                        cellY += yoff;
                        quadX += xquad;
                        cellX += xoff;
                        break;
                    }
                case 6:
                    {
                        //west
                        int tempY = 0;
                        int tempX = (int)(-8 * warp);
                        int yoff = 0;
                        int xoff = 1 * tempX % 8;
                        int yquad = 0;
                        int xquad = 1 * tempX / 8;

                        if (blockCheck(quadX, quadY, cellX - 1, cellY) == Cell.STAR) canWe = false;
                        else canWe = true;
                        quadY += yquad;
                        cellY += yoff;
                        quadX += xquad;
                        cellX += xoff;
                        break;
                    }
                case 7:
                    {
                        //northwest
                        int tempX = (int)(-8 * warp);
                        int tempY = (int)(-8 * warp);
                        int xoff = 1 * tempX % 8; ;
                        int yoff = 1 * tempY % 8;
                        int xquad = 1 * tempX / 8;
                        int yquad = 1 * tempY / 8;

                        if (blockCheck(quadX, quadY, cellX - 1, cellY - 1) == Cell.STAR) canWe = false;
                        else canWe = true;
                        quadY += yquad;
                        cellY += yoff;
                        quadX += xquad;
                        cellX += xoff;
                        break;
                    }
            }

            if (!canWe)
            {
                Console.WriteLine("Bad navigation, not moving.");
                return;
            }

            //correct coords if negative
            if (cellX < 0)
            {
                cellX = 7 - cellX;
                quadX--;
            }
            if (cellY < 0)
            {
                cellY = 7 - cellY;
                quadY--;
            }
            if (cellX > 7)
            {
                cellX -= 7;
                quadX++;
            }
            if (cellY > 7)
            {
                cellY -= 7;
                quadY++;
            }

            if (quadX >= 0 && quadX < 8 && quadY >= 0 && quadY < 8)
            {
                if (grid[quadX, quadY, cellX, cellY] != Cell.KLINGON)
                {
                    if (grid[quadX, quadY, cellX, cellY] == Cell.BASE)
                    {
                        Console.WriteLine("You have stumbled on a base. How many phasors do you need?");
                        int p = Int32.Parse(Console.ReadLine());
                        inventory += p;
                        Console.WriteLine("Inventory added.");
                    }
                    else
                    {
                        deleteShip();
                        grid[quadX, quadY, cellX, cellY] = Cell.SHIP;
                    }
                }
                else
                {
                    numKlingons--;
                    shieldStrength -= 100;
                    Console.WriteLine($"You have killed 1 klingon. Your shield strength is: {shieldStrength}");
                }
            }
            else Console.WriteLine("could not navigate to uncharted territory.");
        }
        catch (Exception e) { Console.WriteLine("exception in navigation, try again."); }
        }
    public static void drawMap() {
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                int cnt = getBaseCount(x, y);
                if (cnt >= 10) Console.WriteLine($" {cnt}");
                else Console.Write($"  {cnt}");
            }
            Console.WriteLine();
        }
    }

    public static int getBaseCount(int x, int y) {
        int start = (y * 8 + x) * 64;
        int cnt = 0;
        for (int pos = start; pos < start + 64; pos++) {
            int[] parsed = parseGrid(pos);
            int x1 = parsed[0];
            int y1 = parsed[1];
            int x2 = parsed[2];
            int y2 = parsed[3];
            if (grid[x1, y1, x2, y2] == Cell.BASE) cnt++;
        }

        return cnt;
    }

    //get klingon count in quadrant
    public static int getKlingonCount(int x, int y)
    {
        int start = (y * 8 + x) * 64;
        int cnt = 0;
        for (int pos = start; pos < start + 64; pos++)
        {
            int[] parsed = parseGrid(pos);
            int x1 = parsed[0];
            int y1 = parsed[1];
            int x2 = parsed[2];
            int y2 = parsed[3];
            if (grid[x1, y1, x2, y2] == Cell.KLINGON) cnt++;
        }

        return cnt;
    }
    public static bool exists(int num, int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == num) return true;
        }
        return false;
    }

    public static int[] parseGrid(int num)
    {
        int x1 = (num / 64) % 8;
        int y1 = (num / 64) / 8;
        int x2 = (num % 64) % 8;
        int y2 = (num % 64) / 8;
        int[] nums = { x1, y1, x2, y2 };
        return nums;
    }
    public static void populateGrid()
    {
        Random rnd = new Random();
        int maxPos = 64 * 64;
        int randNum = 0;
        int[] starLocs = new int[Cell.NUMSTARS];
        for (int i = 0; i < Cell.NUMSTARS; i++)
        {
            do
            {
                randNum = rnd.Next(maxPos);
            } while (exists(randNum, starLocs));
            int[] parsed = parseGrid(randNum);
            int x = parsed[0];
            int y = parsed[1];
            int x1 = parsed[2];
            int y1 = parsed[3];
            grid[x, y, x1, y1] = Cell.STAR;
        }
    }

    public static void placeKlingons()
    {
        Random rnd = new Random();
        int maxPos = 64 * 64;
        int randNum = 0;
        bool isStar = false;
        numKlingons = Cell.MAXKLINGONS;
        int[] starLocs = new int[numKlingons];
        for (int i = 0; i < numKlingons; i++)
        {
            do
            {
                randNum = rnd.Next(maxPos);
                int[] posArray = parseGrid(randNum);
                int x2 = posArray[0];
                int y2 = posArray[1];
                int x3 = posArray[2];
                int y3 = posArray[3];
                if (grid[x2, y2, x3, y3] > 0) isStar = true;
                else isStar = false;
            } while (exists(randNum, starLocs) || isStar);
            int[] parsed = parseGrid(randNum);
            int x = parsed[0];
            int y = parsed[1];
            int x1 = parsed[2];
            int y1 = parsed[3];
            grid[x, y, x1, y1] = Cell.KLINGON;
        }
    }

    //place the bases around
    public static void placeBases()
    {
        Random rnd = new Random();
        int maxPos = 64 * 64;
        int randNum = 0;
        bool isStar = false;
        int numKlingons = Cell.MAXBASES;
        int[] starLocs = new int[numKlingons];
        for (int i = 0; i < numKlingons; i++)
        {
            do
            {
                randNum = rnd.Next(maxPos);
                int[] posArray = parseGrid(randNum);
                int x2 = posArray[0];
                int y2 = posArray[1];
                int x3 = posArray[2];
                int y3 = posArray[3];
                if (grid[x2, y2, x3, y3] > 0) isStar = true;
                else isStar = false;
            } while (exists(randNum, starLocs) || isStar);
            int[] parsed = parseGrid(randNum);
            int x = parsed[0];
            int y = parsed[1];
            int x1 = parsed[2];
            int y1 = parsed[3];
            grid[x, y, x1, y1] = Cell.BASE;
        }
    }

    //place the ship
    public static void placeShip() {

        int maxPos = 64 * 64;
        Random rnd = new Random();
        int shipPos = 0;
        do
        {
            shipPos = rnd.Next(maxPos);
            int[] parsed = parseGrid(shipPos);
            quadX = parsed[0];
            quadY = parsed[1];
            cellX = parsed[2];
            cellY = parsed[3];
        } while (grid[quadX,quadY,cellX,cellY] > 0);
        grid[quadX, quadY, cellX, cellY] = Cell.SHIP;
    }

    //kill some klingons
    public static void killKlingons(int pha, int x, int y) {
        int start = (8 * y + x) * 64;
        for (int cyc = start; cyc < start + 64; cyc++) {
            int[] parsed = parseGrid(cyc);
            int x1 = parsed[0];
            int y1 = parsed[1];
            int x2 = parsed[2];
            int y2 = parsed[3];
            if (grid[x1, y1, x2, y2] == Cell.KLINGON && pha>0 ) {
                grid[x1, y1, x2, y2] = 0;
                pha--;
                numKlingons--;
            }
        }
    }

    //delete old ship
    public static void deleteShip() {
        int maxNum = 64 * 64;
        for (int i = 0; i < maxNum; i++) {
            int[] parsed = parseGrid(i);
            int x1 = parsed[0];
            int y1 = parsed[1];
            int x2 = parsed[2];
            int y2 = parsed[3];
            if (grid[x1, y1, x2, y2] == Cell.SHIP) grid[x1, y1, x2, y2] = 0;
        }
    }

    //check for a star blockage
    public static int blockCheck(int quadX, int quadY, int cellX, int cellY) {
        if (cellX < 0) {
            cellX+=8;
            quadX--;
        }
        if (cellX > 7)
        {
            cellX -=8;
            quadX++;
        }
        if (cellY < 0)
        {
            cellY+=8;
            quadX--;
        }
        if (cellY > 7)
        {
            cellY -= 8;
            quadX++;
        }
        if (quadX < 0 || quadX > 7 || quadY < 0 || quadY > 7)
        {
            return -1;
        }
        else {
            if (grid[quadX, quadY, cellX, cellY] == Cell.STAR) return Cell.STAR;
        }
        return 0;
    }
}