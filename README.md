# unityleveltiles
Unity tile-based level generator

Just apply the TilesCreator.cs script to any Gameobject in your scene.
Then just add all your floor tiles to the tiles section and provide aliases for them in the aliases section.

![Alt text](Screenshots/Screenshot1.png?raw=true)

Choose a textfile which will contain all your aliases. It could look like this:

```
A1A2A1A3A1A4A3A4A3
A2A1A4A1  A2A1A2
A3A4A2A2A4A2A1A2A1
-A4A3A3A4A2A1A4A3A4
A1A2A1A3A1A4A3A4A3
```

The dash ("-") will make the row shifted by the width of a single tile divided by 2.
Blanks (multiples of two) will leave the place for the tile empty.

Hit the "Build Object" button and watch it happen.

![Alt text](Screenshots/Screenshot2.png?raw=true)





 
