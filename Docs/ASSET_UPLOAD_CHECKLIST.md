# Day 01 Asset Upload Checklist

Use this checklist when replacing placeholders with final art.

## Upload location

Recommended source folder:

`Assets/Art/Day01/Source/`

Recommended imported sprite folder:

`Assets/Resources/Generated/Day01/`

## First batch — required

| File name | Type | Suggested size | Alpha | Purpose |
|---|---|---:|---|---|
| `day01_backroom_bg.png` | static background | 1600×1200 | no | warehouse half of scene |
| `day01_salesfloor_bg.png` | static background | 1600×1200 | no | sales floor half of scene |
| `day01_cart.png` | dynamic sprite | 768×768 | yes | replenishment cart |
| `day01_box_cola.png` | dynamic sprite | 512×512 | yes | cola case |
| `day01_box_water.png` | dynamic sprite | 512×512 | yes | water case |
| `day01_box_milk.png` | dynamic sprite | 512×512 | yes | milk case |
| `day01_shelf_frame.png` | static / semi-static sprite | 1200×1000 | yes | refrigerator or shelf frame without products |
| `day01_product_cola.png` | dynamic sprite | 256×512 | yes | single cola bottle |
| `day01_product_water.png` | dynamic sprite | 256×512 | yes | single water bottle |
| `day01_product_milk.png` | dynamic sprite | 256×512 | yes | single milk bottle |
| `day01_customer_01.png` | dynamic sprite | 512×1024 | yes | first customer |
| `day01_customer_02.png` | dynamic sprite | 512×1024 | yes | second customer |

## Important slicing rule

Do not bake products into the shelf background.

The refrigerator / shelf frame should be exported without saleable products. Products must be separate sprites so the game can show:

- full slot
- low stock
- empty slot
- restock animation
- customer purchase

## Shadow rule

For transparent moving sprites:

- keep a soft contact shadow only when it belongs to the object
- do not include large environment shadows
- avoid hard rectangular shadows around alpha edges

## Perspective rule

Keep one consistent camera angle across:

- boxes
- cart
- shelf
- customers

A front or light 3/4 perspective is preferred. Do not mix top-down product art with eye-level shelf art.

## Safe margins

Transparent sprites should keep roughly 5–10% empty padding around the visible object. This avoids clipping during selection scale, bounce and fly animations.

## Naming rule

Use lowercase snake case and keep product IDs stable:

- `cola`
- `water`
- `milk`
- `juice`

These names should match gameplay product IDs later.

## First replacement priority

1. shelf frame without products
2. three single-product sprites
3. cart
4. three product boxes
5. backroom and sales-floor backgrounds
6. customers

The first seven items are enough to make the replenishment loop look coherent before adding more departments.
