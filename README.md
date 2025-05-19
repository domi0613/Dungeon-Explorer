Dungeon Explorer by [Illés Dominik] [U5RWPY]

Welcome to Dungeon Explorer — a text-based console adventure game written in F#! Explore dangerous rooms, fight deadly enemies, discover hidden treasure, and shop with mysterious merchants on your journey through the dungeon.

---

🎯 Project Motivation

Dungeon Explorer was developed to blend core functional programming concepts with engaging gameplay mechanics. It serves both as a learning project for F# and a creative challenge to implement RPG elements using a functional-first paradigm.

Goals:

- Learn and practice functional programming in a game environment
- Design a modular, extensible console game
- Apply OOP features (mutable state, records) in F#
- Have fun while learning!

---

🕹️ How to Play

1. Enter your name.
2. Choose how many rooms to explore (recommended: 10–30).
3. Explore rooms by selecting from options:
   - Fight enemies
   - Use items from inventory
   - Visit merchants and buy loot
   - Survive traps and collect treasures
4. Special items unlock powerful bonuses when collected in sets of 3.

Controls:

```
1 - Explore next room
2 - Use an item
3 - View inventory
```

---

⚔️ Enemy Types

| Enemy    | HP  | ATK |
|----------|-----|-----|
| Skeleton | 30  | 5   |
| Goblin   | 20  | 7   |
| Orc      | 40  | 10  |

Enemies may drop loot and gold upon defeat.

---

💰 Loot Table

Loot can be found in treasure rooms, after combat, or bought from merchants.

Common Loot

- **Potion (+10–30 HP)** — Restores health
- **Gold (10–50 coins)** — Spend at merchants

Weapons

| Name         | ATK Bonus |
|--------------|-----------|
| Rusty Sword  | +5        |
| Axe          | +7        |
| Magic Blade  | +10       |
| Iron Sword   | +10       (Merchant only)

Armor

| Name           | DEF Bonus |
|----------------|-----------|
| Leather Armor  | +3        |
| Chainmail      | +5        |
| Mystic Robe    | +7        |
| Scale Armor    | +5        (Merchant only)

Special Items

- **Ancient Relic** – Collect 3 to gain +10 HP and +10 Gold
- **Map Fragment** – Collect 3 to unlock a hidden treasure room

> Note: These items are not usable but auto-trigger their effects once 3 are collected.

---

🧙 Merchants

Randomly encountered in rooms. You can buy items with your gold. Merchant names include:

- Bob the Trader
- Mira
- Shady Dealer

Sample Merchant Inventory:

- Potion (20 HP) – 15 gold
- Iron Sword (+10 ATK) – 30 gold
- Scale Armor (+5 DEF) – 25 gold
- Map Fragment – 50 gold

---

☠️ Traps

Some rooms may contain traps dealing 5–15 HP damage instantly.

---

🎁 Special Mechanics

- **Loot Drops**: 50% chance to get bonus loot after defeating enemies.
- **Set Bonuses**: Certain special items activate buffs when 3 are collected.

---

🧾 Credits

Developed as a personal learning project using F# and .NET console applications.
Developer and Tester: Dominik Illés
Copyright (C) 2025 U5RWPY All rights reserved.

---

📦 How to Run

Requires [.NET SDK](https://dotnet.microsoft.com/en-us/download) installed.

1. Clone or copy the code to a `.fs` file
2. Open terminal or command prompt
3. Run using:

```bash
dotnet fsi DungeonExplorer.fs
```

Enjoy your adventure! 🗡️🛡️💎