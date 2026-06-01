# CLI Minesweeper
Twenty mines have been randomly placed on a 10-by-12 board. Reveal all non-mine tiles to win, and use flags to keep track of mine positions.

## Prerequisites
To conform to CS20200 course requirements, players must have .NET 10 installed on their device.

- [.NET 10 SDK](https://dotnet.microsoft.com/download)  
  Verify with: `dotnet --version` (should show `10.x.x`)

The above portion was taken from the course's example project `README.md` [file](https://github.com/KAIST-CS20200/project-example/blob/main/README.md).

## Game Setup
If .NET 10 has been successfully set up, you can play the game by using the commands
```bash
git clone https://github.com/jmdmariano/CLI-Minesweeper.git
```
and
```bash
dotnet run --project CLI-Minesweeper
```

## How to Play
At the program's start, players will see a covered 10-by-12 board. A tile's coordinates refer to its corresponding row and column labels (e.g., A2, B11, C6, etc.).

1. Enter `R [Tile]` (e.g., `R E9`) to reveal any first tile of your choice. It is always an empty tile (i.e., no neighboring mines). Player input need not be uppercase.
2. A tile's neighbors include the tile Up, Down, Left, Right, Upper Left, Upper Right, Lower Left, and Lower Right to the tile if these tiles exist. A tile has at most 8 neighbors. Except for empty tiles, revealed tiles indicate the number of mines in neighboring tiles. 
3. Use this information to identify the positions of the mines.
4. To flag a tile as a mine, use `F [Tile]`. Use the same command to unflag a tile. You cannot place more than 20 flags on the board, and you may also not reveal flagged tiles. If you wish to reveal a flagged tile, unflag it first.
5. Revealing a tile that is a mine instantly ends the game. 
6. On the other hand, revealing all tiles that are not mines results in a win. Note that it is possible to win without placing any flags, and players seeking an additional challenge are free to do so.
7. At the end of each game, players are given an option to restart. `Y` returns the player to Step 1, and `N` terminates the program. Best of luck! 

 
## LLM Usage and Attribution
The implementation for this project was written entirely by Gemini 3.1 Pro. Only a single prompt was necessary to yield a working `Program.fs` file that met the requirements specified in the accompanying requirements document. There were no errors aside from a minor syntax error in line 174. I believe the experience of using an LLM was smooth because the game's behavior was specific and clearly defined.

## Change Log
There are only two minor changes to note between the requirements document and the game's form.
1. Covered tiles are denoted with a " . " instead of being left blank.
2. Empty tiles (i.e., 0 neighboring mines) are left blank instead of being denoted with a " 0 ".

These changes make the game much easier to look at, which is important given the limited visual capabilities of a CLI. The first change makes it easier to identify the coordinates of covered tiles. Additionally, by not populating a large portion of the board with 0s, the visual strain a player might experience by staring at an array of numbers and flags is reduced. As these minor visual changes enhance the game by a degree, they should be considered reasonable.
