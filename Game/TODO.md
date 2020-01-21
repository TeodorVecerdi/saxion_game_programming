<h4>TODO</h4>

<span><b>General</b></span>
- [ ] <span style="color: lime">Implement player death
- [ ] <span style="color: red">Player is able to move diagonally by pressing W&A or another combination of keys enabling sneaky and cheaty behaviour
- [ ] <span style="color: darkgreen">Use diamond spawner for butterfly instead of spawning diamonds directly 
- [ ] <span style="color: darkgreen">Background flashes once collected enough diamonds
- [ ] <span style="color: darkgreen">Animate time getting added to player score at the end of room



<span><b>Code Changes</b></span>
+ [ ] convert public variables to private with getter where necessary
+ [ ] general code cleanup; keep variables to correct name convention where necessary

<hr/>
<h4>DONE</h4>

<span><b>Butterfly AI Description</b></span>
- [x] Movement behaviour same as firefly (wall follow)
- [x] When a boulder falls on butterfly it turns a 3x3 area into diamonds.
<hr/>

- [x] <span style="color: none">Add sound
- [x] Separate GameObjects for different tile types (40*22 = 880 objects max incl. the player)
- [x] Implement falling for diamond and butterfly
- [x] Make door flash once revealed
- [x] Amoeba grows into empty spaces 
- [x] Amoeba turns into diamonds if contained, boulders otherwise when time done
- [x] Player collects diamond if diamond falls on player
- [x] If ctrl is pressed do action without moving such as getting diamonds, pushing boulders or removing dirt
- [x] Add boulder push mechanic
- [x] Add player animation
- [x] Add time left to score when finished
- [x] Implement firefly follow wall AI
- [x] Implement camera behaviour
- [x] Implement butterfly AI
- [x] Add object for spawning diamonds (eg after butterfly explosion); also add to TileType
