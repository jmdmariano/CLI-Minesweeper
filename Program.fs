open System
open System.Text.RegularExpressions

// --- Domain Models ---
type CellContent =
    | Mine
    | Safe of int // Neighboring mines (0 to 8)

type CellState =
    | Covered
    | Revealed
    | Flagged

type Cell = { Content: CellContent; State: CellState }

type GameStatus =
    | Playing
    | Won
    | Lost

type GameState = {
    Grid: Map<int * int, Cell>
    FlagsPlaced: int
    Status: GameStatus
    IsFirstMove: bool
}

type Command =
    | Reveal of int * int
    | ToggleFlag of int * int
    | Invalid of string

// --- Constants & Game Rules ---
let rows = 10
let cols = 12
let mineCount = 20

let allCoords = [ for r in 0 .. (rows - 1) do for c in 0 .. (cols - 1) do yield (r, c) ]

let getNeighbors (r, c) =
    [ for dr in -1 .. 1 do
        for dc in -1 .. 1 do
            if dr <> 0 || dc <> 0 then
                let nr, nc = r + dr, c + dc
                if nr >= 0 && nr < rows && nc >= 0 && nc < cols then
                    yield (nr, nc) ]

// --- Board Initialization ---
// Generates a purely empty board before the first move
let createEmptyBoard () =
    allCoords 
    |> List.map (fun coord -> coord, { Content = Safe 0; State = Covered }) 
    |> Map.ofList

// Populates mines after the first click to guarantee an empty tile (0 neighbors)
let populateBoard firstMove =
    let rng = Random()
    // To guarantee the first tile is '0', the clicked tile and its immediate neighbors cannot be mines.
    let safeZone = getNeighbors firstMove |> Set.ofList |> Set.add firstMove
    
    let mineLocations = 
        allCoords
        |> List.filter (fun c -> not (Set.contains c safeZone))
        |> List.sortBy (fun _ -> rng.Next())
        |> List.take mineCount
        |> Set.ofList

    let calculateContent coord =
        if Set.contains coord mineLocations then Mine
        else
            let count = getNeighbors coord |> List.filter (fun n -> Set.contains n mineLocations) |> List.length
            Safe count

    allCoords
    |> List.map (fun coord -> coord, { Content = calculateContent coord; State = Covered })
    |> Map.ofList

// --- Game Logic ---
let rec reveal (grid: Map<int * int, Cell>) (r, c) =
    match Map.tryFind (r, c) grid with
    | Some cell when cell.State = Covered ->
        let newGrid = Map.add (r, c) { cell with State = Revealed } grid
        match cell.Content with
        | Safe 0 -> 
            // Flood-fill recursion for empty tiles
            getNeighbors (r, c) |> List.fold reveal newGrid
        | _ -> newGrid
    | _ -> grid

let checkWinCondition (state: GameState) =
    let totalSafeCells = (rows * cols) - mineCount
    let revealedSafeCells = 
        state.Grid 
        |> Map.values 
        |> Seq.filter (fun cell -> cell.State = Revealed && match cell.Content with | Safe _ -> true | _ -> false) 
        |> Seq.length
    
    if revealedSafeCells = totalSafeCells then { state with Status = Won } else state

let processTurn command state =
    match command with
    | Invalid msg ->
        printfn "\nError: %s" msg
        printfn "Valid commands: 'R A5' to reveal, 'F A5' to flag/unflag."
        state
    
    | ToggleFlag (r, c) ->
        let cell = state.Grid.[(r, c)]
        match cell.State with
        | Revealed -> 
            printfn "\nCannot flag a revealed tile."
            state
        | Flagged -> 
            let newGrid = Map.add (r, c) { cell with State = Covered } state.Grid
            { state with Grid = newGrid; FlagsPlaced = state.FlagsPlaced - 1 }
        | Covered -> 
            if state.FlagsPlaced >= mineCount then
                printfn "\nYou have used all available flags."
                state
            else
                let newGrid = Map.add (r, c) { cell with State = Flagged } state.Grid
                { state with Grid = newGrid; FlagsPlaced = state.FlagsPlaced + 1 }
                
    | Reveal (r, c) ->
        let cell = state.Grid.[(r, c)]
        match cell.State with
        | Flagged ->
            printfn "\nCannot reveal a flagged tile. Unflag it first."
            state
        | Revealed ->
            printfn "\nTile is already revealed."
            state
        | Covered ->
            // Handle first move deferred generation
            let gridToUse = if state.IsFirstMove then populateBoard (r, c) else state.Grid
            let activeState = { state with Grid = gridToUse; IsFirstMove = false }
            
            let targetCell = activeState.Grid.[(r, c)]
            match targetCell.Content with
            | Mine -> 
                let lostGrid = Map.add (r, c) { targetCell with State = Revealed } activeState.Grid
                { activeState with Grid = lostGrid; Status = Lost }
            | Safe _ -> 
                let newGrid = reveal activeState.Grid (r, c)
                checkWinCondition { activeState with Grid = newGrid }

// --- UI & Input ---
let printBoard (state: GameState) =
    printfn "\nFlags Remaining: %d" (mineCount - state.FlagsPlaced)
    printf "   "
    for c in 1 .. cols do printf "%2d " c
    printfn ""
    
    for r in 0 .. (rows - 1) do
        let rowLabel = char (int 'A' + r)
        printf "%c  " rowLabel
        for c in 0 .. (cols - 1) do
            let cell = state.Grid.[(r, c)]
            match cell.State with
            | Covered -> printf " . "
            | Flagged -> printf " F "
            | Revealed ->
                match cell.Content with
                | Mine -> printf " * "
                | Safe 0 -> printf "   "
                | Safe n -> printf " %d " n
        printfn ""

let parseInput (input: string) : Command =
    let regex = Regex(@"^([RF])\s+([A-J])([1-9]|1[0-2])$", RegexOptions.IgnoreCase)
    let m = regex.Match(input.Trim())
    if m.Success then
        let action = m.Groups.[1].Value.ToUpper()
        let row = int (m.Groups.[2].Value.ToUpper().[0]) - int 'A'
        let col = int m.Groups.[3].Value - 1
        match action with
        | "R" -> Reveal (row, col)
        | "F" -> ToggleFlag (row, col)
        | _ -> Invalid "Unknown action."
    else
        Invalid "Invalid format."

// --- Application Loops ---
let rec gameLoop state =
    printBoard state
    match state.Status with
    | Won -> printfn "\nCongratulations! You have revealed all safe tiles and won the game!"
    | Lost -> printfn "\nBOOM! You revealed a mine. Game Over."
    | Playing ->
        printf "\nEnter command (e.g., 'R A5' or 'F A5'): "
        let input = Console.ReadLine()
        let command = parseInput input
        let nextState = processTurn command state
        gameLoop nextState

let rec appLoop () =
    printfn "--- CLI MINESWEEPER ---"
    let initialState = { 
        Grid = createEmptyBoard ()
        FlagsPlaced = 0
        Status = Playing
        IsFirstMove = true 
    }
    gameLoop initialState
    
    printf "\nWould you like to restart? (Y/N): "
    let restart = Console.ReadLine().Trim().ToUpper()
    if restart = "Y" then 
        Console.Clear()
        appLoop ()
    else 
        printfn "Thanks for playing!"

// --- Entry Point ---
[<EntryPoint>]
let main argv =
    appLoop ()
    0