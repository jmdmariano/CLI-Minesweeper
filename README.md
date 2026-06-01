## Game Setup
To play the game on your local machine, use
```bash
git clone https://github.com/jmdmariano/CLI-Minesweeper.git
```
and
```bash
dotnet run --project CLI-Minesweeper
```

## LLM Usage and Attribution
The implementation for this project was written entirely by Gemini 3.1 Pro. Only a single prompt was necessary to yield a working Program.fs file that met the requirements specified in the accompanying requirements document. There were no errors aside from a minor syntax error in line 174. I believe the experience of using an LLM was smooth because the game's behavior was specific and clearly defined.

## Change Log
There are only two minor changes to note between the requirements document and the game's form.
(1) Covered tiles are denoted with a " . " instead of being left blank.
(2) Empty tiles (i.e., 0 neighboring mines) are left blank instead of being denoted with a " 0 ".

These changes make the game much easier to look at, which is important given the limited visual capability of a CLI. The first change makes it easier to identify the coordinates of covered tiles. Additionally, by not populating a large portion of the board with 0s, the visual strain a player might experience by staring at an array of numbers and flags is reduced. As these minor visual changes enhance the game by a degree, they should be considered reasonable.
