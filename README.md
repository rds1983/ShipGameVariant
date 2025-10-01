## Overview
<img width="1282" height="752" alt="image" src="https://github.com/user-attachments/assets/4cb04cde-b3d5-42c5-970b-323276ef5024" />

My variant of MonoGame(former XNA) sample ShipGame that doesnt use Content Pipeline. 

[XNAssets](https://github.com/rds1983/XNAssets) is used instead. Hence all assets are loaded in the raw form.

Particularly 3d models are loaded from [glb](https://www.khronos.org/gltf/) using library [DigitalRiseModel](https://github.com/DigitalRiseEngine/DigitalRiseModel).

The game has solution for 3 frameworks: MonoGame.DesktopGL, MonoGame.WindowsDX and FNA.Core

## Building From Source For MonoGame
Simply open ShipGame.MonoGame.DesktopGL.sln or ShipGame.MonoGame.WindowsDX.sln in the IDE.

## Building From Source For FNA
Clone following libraries in one folder:
Link|Description
----|-----------
https://github.com/FNA-XNA/FNA|FNA
https://github.com/rds1983/XNAssets|Asset management library
https://github.com/FontStashSharp/FontStashSharp|Text rendering library
https://github.com/rds1983/DigitalRiseModel|3D model library
this repo|

Then open ShipGame.FNA.Core.sln in the IDE.

## Credits
[MonoGame](https://github.com/MonoGame/MonoGame)

[MonoGame Samples](https://github.com/MonoGame/MonoGame.Samples)

[FNA](https://github.com/FNA-XNA/FNA)





