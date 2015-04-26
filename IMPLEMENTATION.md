# Implementation

## Package Overview
The source code is split into five sections:

- Common: Contains common members such as enums, constants, or util methods.
- Controller: Contains the `GameController` class that reacts to various events triggered during the game.
- Map: Contains the `Cell` and `Map` classes that are used to build the virtual game world.
- Model: Contains the remaining classes of the model, all classes associated with the player entities.
- View: Contains custom forms and controls.

## Events
The game is entirely event-based, no polling is used. All relevant events are triggered by the `Player` class, namely when the health, ammo, location, number of kills changes, or when the player dies.

## Multithreading
Various threads are used. Notably, each `Player` has its own movement thread which is responsible for visually smooth movement.

For synchronisation, two global locks (defined in the `Constants` class) are used: _MovementLock_ and _ShootingLock_. The purpose of the MovementLock is that no two players are allowed to start a move at the very same time. Whenever a player moves (his location
changes), the `GameController` instance checks whether the player can shoot at any other player. Only after this check is completed are the other players allowed to move, to prevent timing issues.

The ShootingLock does not serve an in-game purpose but is used to stop or pause the game quickly when players are shooting. Without that, any active gunfight would continue until one of the players is killed when the game is paused or stopped.

## Decision Tree
The Decision Tree consists of nodes that are instances of the `Decision` class. A `Decision` has 4 fields: _Positive_, _Negative_, _Test_, and _Type_. The tree is queried by evaluating the root node. If a queried node is an inner node, the _Test_ is
carried out with the supplied input data (current state that is used to make the decision). If the _Test_ is successful, the _Positive_ subtree is queried, and the _Negative_ subtree otherwise. If a queried node is a leaf node, the _Type_ contains the value of the leaf that represents the decision (and
corresponds to the number of the enum member) and is returned.

This worked very well and in a general fashion: In order to create a tree, data is required: an array of delegates (the tests), a matrix of booleans (the input data that should be learned), and an array of integers (the decisions that should be made with
the given input data).

The tree is created by calculating the entropy and then the information gain for each attribute (column in the input data). The attribute with the highest information gain is used for the current `Decision` node and the tests, input data, and decisions are
split for the two new children of the tree, _Positive_ and _Negative_, for which the tree building is called recursively.

With this approach, any DT of any size can be built dynamically, requiring any number of binary input values representing the current state and returning integer values representing the decisions. The transition between this more abstract data and the
actually used data of the `DtPlayer` is made with the `DecisionData` struct.

## GameController
The main purpose of the GameController is to handle events that concern more than one player and to provide a mechanism to start/pause/resume/stop the game.

In order to provide methods for finding friends/enemies for example, two dictionaries are used: \_opponents and \_friends. Each player is a key in both dictionaries, with its value being an array of all opposing/friendly players.

## Player
The `Player` is an abstract class, defining all general fields, events, and methods for the players. An example of such general a method is `void Damage(Player opponent, int damage, bool frontalAttack, bool headshot, bool knife)` which is called when one player
damages another player.

A number of methods are abstract as their implementation depends on what kind of player it is. For example, `void DrawPlayerImplementation(Graphics graphics, int scaleFactor, Rectangle box)` delegates the drawing of some implementation details (such as the
current state of the FSM player). Unsurprisingly, the `HumanPlayer` implementation does very little, as the reaction to events such as moving players are made via the keyboard.

## Maze
The `Maze` is generated randomly at the beginning. For that, a stack-based approach is used where each `Cell` initially is a wall. Unvisited cells can be _followed_, turning them into cells where the entities can move to. Randomly, neighbors are marked as
_visited_ for the map generation algorithm, meaning that they will stay walls. This ensures that always, the complete maze is accessible (and not split into multiple submazes). However, it was observed that sometimes, these mazes are quite complicated.

