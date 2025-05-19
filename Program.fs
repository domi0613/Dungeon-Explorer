open System

let printColor (color: ConsoleColor) (text: string) =
    let oldColor = Console.ForegroundColor
    Console.ForegroundColor <- color
    Console.WriteLine(text)
    Console.ForegroundColor <- oldColor

// Game Types and Data Structures

let rand = Random()

type Item =
    | Potion of int
    | Weapon of string * int
    | Armor of string * int
    | Gold of int
    | Special of string

type Enemy = {
    Name: string
    Health: int
    Attack: int
}

type Player = {
    Name: string
    mutable Health: int
    mutable Attack: int
    mutable Defense: int
    mutable Gold: int
    mutable Inventory: ResizeArray<Item>
}

type RoomType =
    | Empty
    | EnemyRoom of Enemy
    | Treasure of Item
    | Trap of int
    | Merchant of string * (Item * int) list

// === Status Panel ===

let showStatus (player: Player) =
    let status = [
        $"Player: {player.Name}"
        $"HP    : {player.Health}"
        $"ATK   : {player.Attack}"
        $"DEF   : {player.Defense}"
        $"Gold  : {player.Gold}"
    ]
    let top = Console.CursorTop
    let rightCol = 50
    let originalCursorTop = Console.CursorTop
    for i in 0 .. status.Length - 1 do
        if top + i < Console.BufferHeight then
            Console.SetCursorPosition(rightCol, top + i)
            Console.Write(status.[i].PadRight(30))
    Console.SetCursorPosition(0, originalCursorTop)

let countSpecialItem (player: Player) (name: string) =
    player.Inventory
    |> Seq.filter (function Special n when n = name -> true | _ -> false)
    |> Seq.length

// === Utility ===

let pause () =
    Console.WriteLine("\nPress Enter to continue...")
    Console.ReadLine() |> ignore

let getStringInput (prompt: string) =
    Console.Write(prompt)
    Console.ReadLine()

let rec getIntInput (prompt: string) =
    Console.Write(prompt)
    match Console.ReadLine() with
    | null -> Console.WriteLine("Invalid input."); getIntInput prompt
    | input ->
        match Int32.TryParse(input) with
        | true, value -> value
        | _ ->
            Console.WriteLine("Invalid input.")
            getIntInput prompt

let generateItem () =
    match rand.Next(5) with
    | 0 -> Potion(rand.Next(10, 31))
    | 1 ->
        let weapons = [ "Rusty Sword", 5; "Axe", 7; "Magic Blade", 10 ]
        let name, atk = weapons.[rand.Next(weapons.Length)]
        Weapon(name, atk)
    | 2 ->
        let armors = [ "Leather Armor", 3; "Chainmail", 5; "Mystic Robe", 7 ]
        let name, def = armors.[rand.Next(armors.Length)]
        Armor(name, def)
    | 3 -> Gold(rand.Next(10, 51))
    | _ -> Special("Ancient Relic")

let maybeDropLoot (player: Player) =
    if rand.NextDouble() < 0.5 then  // 50% chance
        let item = generateItem()
        printColor ConsoleColor.Green "\nYou found bonus loot!"
        match item with
        | Potion amt -> printColor ConsoleColor.Cyan $"Potion (+{amt} HP) added to inventory."
        | Weapon(name, atk) -> printColor ConsoleColor.Yellow $"Weapon: {name} (+{atk} ATK) added to inventory."
        | Armor(name, def) -> printColor ConsoleColor.Yellow $"Armor: {name} (+{def} DEF) added to inventory."
        | Gold amount -> printColor ConsoleColor.Yellow $"Gold: {amount} coins added to inventory."
        | Special name -> printColor ConsoleColor.Magenta $"Special Item: {name} added to inventory."
        player.Inventory.Add(item)
        pause()

// === Player ===

let createPlayer () =
    Console.WriteLine("Welcome to Dungeon Explorer!")
    let name = getStringInput "Enter your name: "
    {
        Name = name
        Health = 100
        Attack = 10
        Defense = 0
        Gold = 50
        Inventory = ResizeArray()
    }

let showInventory player =
    Console.WriteLine("\nInventory:")
    if player.Inventory.Count = 0 then
        Console.WriteLine(" - Empty")
    else
        player.Inventory
        |> Seq.iteri (fun i item ->
            match item with
            | Potion amount -> printfn " %d. Potion (+%d HP)" (i + 1) amount
            | Weapon(name, atk) -> printfn " %d. Weapon: %s (+%d ATK)" (i + 1) name atk
            | Armor(name, def) -> printfn " %d. Armor: %s (+%d DEF)" (i + 1) name def
            | Special name -> printfn " %d. Special Item: %s" (i + 1) name
            | Gold amount -> printfn " %d. Gold: %d coins" (i + 1) amount
        )
    pause()

let useItem player =
    showInventory player
    if player.Inventory.Count > 0 then
        let choice = getIntInput "Choose an item to use (0 to cancel): "
        if choice > 0 && choice <= player.Inventory.Count then
            match player.Inventory.[choice - 1] with
            | Potion amount ->
                player.Health <- player.Health + amount
                Console.WriteLine($"You used a potion and recovered {amount} HP!")
                player.Inventory.RemoveAt(choice - 1)
            | Weapon(name, atk) ->
                player.Attack <- player.Attack + atk
                Console.WriteLine($"You equipped {name}! ATK increased by {atk}.")
                player.Inventory.RemoveAt(choice - 1)
            | Armor(name, def) ->
                player.Defense <- player.Defense + def
                Console.WriteLine($"You equipped {name}! DEF increased by {def}.")
                player.Inventory.RemoveAt(choice - 1)
            | Special description ->
                Console.WriteLine($"You cannot use the {description}.")
            | _ ->
                Console.WriteLine("Unknown item type.")
            pause()

// === Generation ===

let generateEnemy () =
    [|
        { Name = "Skeleton"; Health = 30; Attack = 5 }
        { Name = "Goblin"; Health = 20; Attack = 7 }
        { Name = "Orc"; Health = 40; Attack = 10 }
    |].[rand.Next(3)]

let generateMerchant () =
    let name = [ "Bob the Trader"; "Mira"; "Shady Dealer" ] |> List.item (rand.Next(3))
    let stock = [
        Potion 20, 15
        Weapon("Iron Sword", 10), 30
        Armor("Scale Armor", 5), 25
        Special("Map Fragment"), 50
    ]
    Merchant(name, stock)

let interactWithMerchant (player: Player) (name: string) (stock: (Item * int) list) =
    let rec shopLoop () =
        Console.Clear()
        Console.WriteLine($"\nMerchant {name}: What would you like to buy?")
        stock
        |> List.iteri (fun i (item, price) ->
            let desc =
                match item with
                | Potion hp -> $"Potion (+{hp} HP)"
                | Weapon(n, atk) -> $"Weapon: {n} (+{atk} ATK)"
                | Armor(n, def) -> $"Armor: {n} (+{def} DEF)"
                | Special name -> $"Special: {name}"
                | Gold amt -> $"Gold Pouch (+{amt} Gold)"
            Console.WriteLine($"{i + 1}. {desc} - {price} gold")
        )
        Console.WriteLine("0. Leave")
        Console.WriteLine($"Your gold: {player.Gold}")
        showStatus player
        match getIntInput "Choose item to buy: " with
        | 0 -> ()
        | choice when choice >= 1 && choice <= stock.Length ->
            let item, price = stock.[choice - 1]
            if player.Gold >= price then
                player.Gold <- player.Gold - price
                Console.WriteLine("Purchase successful!")
                player.Inventory.Add(item)
            else
                Console.WriteLine("Not enough gold.")
            pause()
            shopLoop()
        | _ ->
            Console.WriteLine("Invalid choice.")
            shopLoop()
    shopLoop()

let generateRoom () =
    match rand.Next(5) with
    | 0 -> Empty
    | 1 -> EnemyRoom(generateEnemy())
    | 2 -> Treasure(generateItem())
    | 3 -> Trap(rand.Next(5, 16))
    | _ -> generateMerchant()

// === Combat ===

let fight (player: Player) (enemy: Enemy) =
    Console.Clear()
    printColor ConsoleColor.Magenta $"\nA wild {enemy.Name} appears!"

    let mutable enemyHealth = enemy.Health
    while enemyHealth > 0 && player.Health > 0 do
        showStatus player
        Console.WriteLine($"\n{player.Name}'s HP: {player.Health}")
        Console.WriteLine($"{enemy.Name}'s HP: {enemyHealth}")
        Console.WriteLine("1. Attack\n2. Use Item")
        match Console.ReadLine() with
        | "1" ->
            printColor ConsoleColor.Yellow $"You hit the {enemy.Name} for {player.Attack} damage!"
            enemyHealth <- enemyHealth - player.Attack
        | "2" -> useItem player
        | _ -> Console.WriteLine("Invalid action.")
        if enemyHealth > 0 then
            Console.WriteLine($"{enemy.Name} hits you for {enemy.Attack} damage!")
            player.Health <- player.Health - enemy.Attack
    if player.Health > 0 then
        Console.WriteLine($"You defeated the {enemy.Name}!")

        let goldReward = rand.Next(5, 21)
        player.Gold <- player.Gold + goldReward
        Console.ForegroundColor <- ConsoleColor.Yellow
        Console.WriteLine($"You found {goldReward} gold on the enemy.")
        Console.ResetColor()
        maybeDropLoot player
    else
        printColor ConsoleColor.DarkRed $"You died..."
    pause()

// === Special Rewards Logic ===

let checkSpecialRewards (player: Player) =
    let relics = countSpecialItem player "Ancient Relic"
    if relics >= 3 then
        Console.ForegroundColor <- ConsoleColor.Magenta
        Console.WriteLine("✨ You feel the power of the relics surge within you! +10 HP, +10 Gold.")
        Console.ResetColor()
        player.Health <- player.Health + 10
        player.Gold <- player.Gold + 10
        // Remove 3 relics
        let toRemove =
            player.Inventory
            |> Seq.mapi (fun i item -> i, item)
            |> Seq.filter (fun (_, item) -> item = Special "Ancient Relic")
            |> Seq.take 3
            |> Seq.map fst
            |> Seq.toList
        toRemove |> List.rev |> List.iter (fun i -> player.Inventory.RemoveAt(i))

    let fragments = countSpecialItem player "Map Fragment"
    if fragments >= 3 then
        Console.ForegroundColor <- ConsoleColor.Cyan
        Console.WriteLine("🗺️ You piece together the map and find a hidden room full of treasure!")
        Console.ResetColor()
        player.Inventory.Add(generateItem())
        let treasureGold = rand.Next(30, 61)
        player.Gold <- player.Gold + treasureGold
        // Remove 3 fragments
        let toRemove =
            player.Inventory
            |> Seq.mapi (fun i item -> i, item)
            |> Seq.filter (fun (_, item) -> item = Special "Map Fragment")
            |> Seq.take 3
            |> Seq.map fst
            |> Seq.toList
        toRemove |> List.rev |> List.iter (fun i -> player.Inventory.RemoveAt(i))

// === Exploration ===

let exploreRoom player =
    Console.Clear()
    let room = generateRoom()
    showStatus player
    match room with
    | Empty ->
        Console.WriteLine("The room is empty.")
    | EnemyRoom enemy ->
        fight player enemy
    | Treasure item ->
        printColor ConsoleColor.Green "You found a treasure!"
        match item with
        | Potion amt ->
            printColor ConsoleColor.Cyan $"Potion (+{amt} HP) added to inventory."
            player.Inventory.Add(item)
        | Weapon(name, atk) ->
            printColor ConsoleColor.Yellow $"Weapon: {name} (+{atk} ATK) added to inventory."
            player.Inventory.Add(item)
        | Armor(name, def) ->
            printColor ConsoleColor.Yellow $"Armor: {name} (+{def} DEF) added to inventory."
            player.Inventory.Add(item)
        | Gold amount ->
            printColor ConsoleColor.Yellow $"Gold: {amount} coins added to your pocket."
            player.Gold <- player.Gold + amount
        | Special name ->
            printColor ConsoleColor.Magenta $"Special Item: {name} added to inventory."
            player.Inventory.Add(item)
    | Trap dmg ->
        printColor ConsoleColor.Red $"It's a trap! You take {dmg} damage."
        player.Health <- player.Health - dmg
    | Merchant(name, stock) ->
        Console.WriteLine($"You meet a merchant named {name}.")
        interactWithMerchant player name stock
    checkSpecialRewards player
    pause()

// === Game Loop ===

let rec gameLoop (player: Player) (dungeonSize: int) =
    if dungeonSize <= 0 then
        Console.WriteLine("You’ve reached the end of the dungeon!")
        showStatus player
    elif player.Health <= 0 then
        Console.WriteLine("You died in the dungeon.")
        showStatus player
    else
        Console.Clear()
        Console.WriteLine($"Rooms left: {dungeonSize}")
        showStatus player
        Console.WriteLine("1. Explore next room\n2. Use item\n3. View inventory")
        match Console.ReadLine() with
        | "1" -> exploreRoom player; gameLoop player (dungeonSize - 1)
        | "2" -> useItem player; gameLoop player dungeonSize
        | "3" -> showInventory player; gameLoop player dungeonSize
        | _ -> Console.WriteLine("Invalid choice."); pause(); gameLoop player dungeonSize

// === Game Start ===

let rec startGame () =
    Console.Clear()
    let player = createPlayer()
    let dungeonSize = getIntInput "How many rooms do you want to explore? (e.g. 5–30): "
    gameLoop player dungeonSize
    let restart = getStringInput "Play again? (y/n): "
    if String.Equals(restart, "y", StringComparison.OrdinalIgnoreCase) then
        startGame()
    else
        Console.WriteLine("Thanks for playing!")

[<EntryPoint>]
let main argv =
    startGame()
    0