# Against The Grain  
## Game Design Document

## 1. High Concept

**Against The Grain** is a top-down 2D turn-based tactical strategy game where agricultural production fuels resistance against industrial expansion. Players manage crops, labor, and combat units on a shared grid-based battlefield while defending farmland from advancing robotic forces.

The core experience centers on spatial logistics, economic planning, and positional combat under pressure.

## 2. Thematic Design Note

While mechanically focused on grid tactics and resource systems, the game is thematically framed as:

A struggle between farmers/local agriculture and mechanical industrial expansion.

- **Farmers** represent labor, stewardship, and cyclical growth.
- **Robots** represent efficiency, encroachment, and linear expansion.
- The battlefield grid symbolizes contested land — both farmland and territory.

This theme informs system relationships:
- Crops require time and care.
- Combat strength emerges from successful cultivation.
- Enemy pressure disrupts production cycles.

The theme supports the mechanics but does not override systemic clarity.

## 3. Core Gameplay Pillars

### 3.1 Tactical Agriculture

- Crops are planted, watered, and harvested on grid tiles.
- Crop growth progresses turn-by-turn.
- Harvested crops serve as economic resources.
- *Scope Dependent* Crops may be used to feed animals for temporary upgrades

**Design Intent:** Farming is spatial and strategic, not background flavor.

### 3.2 Resource-Driven Combat

- Animal units require harvested crops / economic factors to activate.
- Stronger and specialized animals require more planning and resources to utilize.
- Combat readiness depends on economic foresight.

**Design Intent:** Military strength is an extension of agricultural success.

### 3.3 Spatial Strategy on a Shared Grid

- Combat and farming occur on the same board.
- Crops, farmers, animals, and enemies all compete for limited space.
- Movement and attack ranges are visualized.

**Design Intent:** Space is the primary strategic constraint.


### 3.4 Role-Based Unit Design

Each unit fulfills a specific strategic role. The following are proposed unit ideas:

#### Farmers

**Green Farmer**
- Can plant, water, and harvest crops.
- Greater movement range.
- Non-combat unit.

**Red Farmer**
- All Green Farmer abilities.
- Has offensive abilities.
- Moderate movement range.

**Carepenter**
- Builds fences or other defensive structures that obstruct enemy forces
- Has offensive abilities.
- Low movement range


#### Animals

**Chicken**
- Large movement range.
- Best used in groups.
- Can distract Robots

**Cow**
- Small movement range.
- Durable front-line unit.


**Ram/Goat**
- Medium movement range.
- Can knock units around into other spaces

**Design Intent:** No unit is universally optimal; synergy and planning determine success.

### 3.5 Deterministic Enemy Pressure

- Robot enemies operate using grid-based pathfinding.
- They advance toward player-controlled territory.
- Their behavior is predictable but relentless.

**Design Intent:** Enemy logic creates mounting positional pressure without randomness.

## 4. Core Systems

## 4.1 Turn Structure

**Player Turn**
- Move units.
- Perform farming actions.
- Attack enemies using offensive units.
- Purchase / Feed Animals

**End Turn**

**Enemy Turn**
- Robots move and attack based on pathfinding logic.

## 4.2 Crop System

- Crops must be:
  1. Planted
  2. Watered
  3. Grown over time
  4. Harvested
- Harvested crops enter the player’s resource pool.
- Crops occupy grid space and influence tactical positioning.
- Crops may be destryoed by enemy units.

- Crop Ideas:
  - Wheat: Simplest crop, moderate sell value and no special buff abilities.
  - Peppers: Buff focused crop, increases unit movement speed when consumed.

## 4.3 Economy System

- Crops function either function as the primary currency or something you can exchange for generic money.
- Animals must be fed/and or bought before use.
- Feeding removes crops from inventory.
- Stronger units require greater resource investment.

The economy directly limits combat capability.


## 4.4 Combat System

- Grid-based movement.
- Orthogonal (perpendicular) attack adjacency required.
- Units deal fixed damage values.
- No randomness in damage resolution.

Combat clarity reinforces tactical planning.


## 4.5 Spatial Constraints

- Every tile can hold:
  - A crop
  - A unit (Friendly / Enemy)
  - An Obstacle (Rocks, fences, etc)
  - An interactable building (Barns, Farmhouses, etc)
- Farming reduces open mobility.
- Combat reduces safe farming zones.

This creates constant logistical trade-offs.

## 5. Player Tension & Decision Loops

### Primary Decision Loop

1. Expand crop production.
2. Convert harvest into combat units.
3. Defend land from robot encroachment.

### Secondary Tensions

- Plant now or reposition to defend?
- Feed multiple weaker units or one stronger unit?
- Protect farmland or sacrifice it for tactical positioning?


## 6. Controls

**Mouse**
- Select and place units.
- Highlight valid tiles.

**Arrow Keys**
- Navigate action menus.

**Enter**
- Confirm selection.

**Space**
- End turn.

**ESC**
- Pause menu.

# 7. Technical Implementation

- Developed in Unity using C#.
- Grid-based movement and tile interaction system.
- Visualized movement and interaction ranges.
- Deterministic enemy pathfinding within grid constraints.