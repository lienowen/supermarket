# Static + Dynamic Supermarket Game Plan

## Product direction

Build the game as a fixed-camera 2.5D management game instead of a free-roaming 3D supermarket.

The player should feel that the store is alive, but only a small number of objects need to move:

- product boxes
- replenishment cart
- shelf stock slots
- customers
- money / task feedback
- timed events

The environment, lighting, refrigerator body, warehouse walls and most decoration remain static artwork.

## Core loop

1. Select a drink box in the backroom.
2. Load it onto the cart.
3. Fill the cart to the mission requirement.
4. Move the cart to the sales floor.
5. Tap a visible missing shelf slot.
6. Restock that slot.
7. Open the store.
8. Customers consume shelf stock and pay.
9. Low stock creates new replenishment pressure.
10. Earnings unlock later shifts and store upgrades.

## Day 01 milestone

The first playable milestone is intentionally narrow:

- one fixed split-screen scene: backroom + sales floor
- six pickable drink boxes
- one replenishment cart
- six visible shelf slots
- missing-slot-only restocking
- customer purchasing after the opening task
- visible income feedback
- task progress and timer

The purpose is to prove the loop before adding more departments.

## Runtime architecture

### Static presentation

`Day01ScreenPresentation`

Responsibilities:

- compose the fixed 2D scene
- draw designed art at screen-space positions
- expose screen-space hit areas
- draw six logical shelf slots
- preserve the exact slot chosen by the player
- synchronize slot visuals when customers reduce stock

### Interaction flow

`Day01TapFlowController`

Responsibilities:

- select a box
- animate box to cart
- move cart to shelf
- accept taps only on visible missing slots
- animate product to the chosen slot
- hand inventory changes to existing gameplay systems

### Inventory source of truth

`ShelfSystem`

Responsibilities:

- own stock count
- receive restock operations
- expose inventory to customer AI
- trigger mission progress and XP

### Selling loop

`CustomerAI`

Responsibilities:

- walk to product area
- consume stock
- queue / checkout
- add income
- leave the store

## Implementation rule

Do not turn the entire shelf into one large restock button.

A restock interaction should only succeed when the player taps a visible missing slot. This makes the static artwork behave like an interactive management scene instead of a decorative background.

## Next milestones

### M2 — mixed products

- cola
- water
- milk
- juice
- wrong-product penalty
- product-specific missing slots

### M3 — live retail pressure

- low-stock warnings
- customer demand spikes
- lunch rush
- hot-weather water demand
- promotional product multiplier

### M4 — management layer

- order inventory
- upgrade shelf capacity
- faster cart
- faster checkout
- unlock snacks / frozen / produce zones

## Current branch

Development work for this milestone lives on:

`feature/static-dynamic-restock-v1`
