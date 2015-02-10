# unityleveltiles
Unity tile-based level generator.

This will help you creating tile-based levels simply by creating a textfile containing the structure of your tiles.

An alias is a 2-character string that gets mapped to a prefab. Right now it has to be exactly 2 characters long, otherwise it wouldn't work.

Just apply the TilesCreator.cs script to any Gameobject in your scene.
Then just add all your floor tiles to the tiles section and provide aliases for them in the aliases section.

Your tiles should all have the same size, otherwise there will be gaps between your tiles or they would overlap.

In this example I have mapped the strings A1 to A4 to 4 different blue floor tiles.

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



<iframe width="560" height="315" src="https://www.youtube.com/embed/JQKZKk2Spvg" frameborder="0" allowfullscreen></iframe>

 
