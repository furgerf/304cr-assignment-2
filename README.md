# 304cr-assignment-2

## Synopsis
This project contains my solution for the second assignment of the module 304cr "Games and AI" at Coventry University 2014/15. The core challenge is to design and implement a game with at least two different game AI techniques. The game is implemented as a C#-project in Visual Studio.

## Design
The game design is outlined in the [design file](https://github.com/furgerf/304cr-assignment-2/blob/master/DESIGN.md).

## Implementation
Implementation details are discussed in the [implementation file](https://github.com/furgerf/304cr-assignment-2/blob/master/IMPLEMENTATION.md).

## Game Controls/Usage
The following keys are handled when the game is opened:

- Enter key: Stops/Starts the game
- Space key: Pauses/Resumes the game
- r: Reload (human player)
- wasd/arrow keys: Move (human player)

## License
See the [license file](https://github.com/furgerf/304cr-assignment-2/blob/master/LICENSE.md).

## Known Issues/Limitations
- because every player only knows the currently visible part of the map, AI players may move in suboptimal patterns
- because no path finding is used (as the map is not known), players may struggle to move towards other (friendly/enemy) players, depending on the map
- the sound effects sometimes don't work well
- since moving to concurrent sound effects (with `MediaPlayer` instead of `SoundPlayer`), the application crashes when terminated while a sound is playing
- when running the application in another environment, the timing may be off which makes the entire game very fast or slow. Different related values such as player speed or frame rate can be adjusted in the `Player` and `Constants` class

