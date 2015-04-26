# Game Design

## Game Overview
The game is a 2D shooter in a maze where players have limited visibility. The maze is generated randomly (according to some criteria) at start-up.

All players are divided into two teams, _Team Hot_ and _Team Cold_. The goal is to eliminate all players of the opposing team.

Players move at the same time but once one player finds an opponent, they take turns shooting at each other and all movement is halted.

## Players
The players have a number of properties, most of them assigned randomly to make the game more interesting. These are a few of the relevant properties of a player, along with their initial values (or range of potential initial values):

-  Health (100): Each player starts with 100 health.
-  Health Threshold (10-50): A threshold is used for players to determine whether they are still healthy or not.
-  Shooting Accuracy (0.85-0.95): Not all players are equally good at hitting their targets. The accuracy determines whether a bullet hits its target.
-  Headshot Change (0.1-0.2): If a player does hit his opponent, he has a chance of hitting the opponent's head, doubling the damage.
-  Back Damage (35-75): The base damage that is dealt if the player shoots at an opponent from the back or the side.
-  Front Damage (35-Back Damage): It is assumed that it is harder to attack an enemy from the front and he is likely to have stronger armor, thus damage dealt to the front is less than damage dealt to the back. This gives an advantage to the player that _finds_ his opponent over the player that has been found.
-  Ammo (3): Each player has 3 bullets. When a player is out of bullets, he uses his knife and his damage is halved.
-  Slowness (200-350): This determines how slow the player is. This is a value in milliseconds which is the time it takes the player to move from one tile to the next.
- Visibility (5): How far the player can see in the maze.

This gives the players some input that can lead to different decisions and behavior:

- Is the player hurt (health below health threshold)?
- Is the player out of ammo?
- Is there an enemy nearby?

The visibility determines whether the player notices enemies nearby. If there is a living human player, all tiles that are out of the visibility range are hidden, otherwise, all AI visibility ranges are highlighted with the player's color.

The players are aware where their living team mates are.

A player can reload his gun but that leaves him immobile and vulnerable for a time.

To make the game a bit more interesting, each player is assigned a random name and a random color of his team (Hot/Cold referring to the colors).

## AI Algorithms
Three different players are available, with the distinguishing factor being the player's decision-making: Human player, Finite State Machine (FSM) player, and Decision Tree (DT) player.

### Human Player
Each game can have max. 1 human player. The controls are listed in the [readme]( https://github.com/furgerf/304cr-assignment-2/blob/master/README.md#game-controlsusage).

If there is a human player that is alive, the game displays the player's view and hides some of the AI players' information.

### FSM Player
The first AI technique used is the Finite State Machine. The FSM used is visualized [here](https://github.com/furgerf/304cr-assignment-2/blob/master/fsm.png "Finite State Machine").

Initially, a FSM player is in the _Find Enemy_ state, which is shown on the player's token as a green circle. If an opponent enters the visibility range, the FSM player moves to the _Attack Enemy_ state which is shown as a yellow-ish circle. If the FSM
player ends up next to an opponent, either by catching up to an opponent or by getting spotted by one, the player enters the _Combat_ state (red circle). If the player dies, he is now _Dead_, which is shown in a light blue circle. On the other hand, if he kills his opponent, one of three things happens:

If the player does not have any ammo left, he starts _Reloading_ (purple circle). If he has any ammo left and considers himself healthy, he resumes looking for enemies. If he has ammo but is below the health threshold, he tries to find a friendly player to
follow for protection _Find/Follow Friend_ (pink circle), ignoring enemies that he would spot.

### DT Player
The second kind of AI player is controlled by a Decision Tree. This tree is created dynamically from a number of sets of input (current status values)/output (decisions) data, and the same tree is used by all DT players.

Whenever a decision is to be made, the DT is queried, and the player acts according to the decision.

The decisions that can be taken are _MoveToEnemy_ (shown as a yellow diamond), _MoveToFriend_ (pink diamond), _SeekEnemy_ (green diamond), and _Reload_ (gray diamond).

### Influence Map
A common element in shooter games is that members of a team know each other's position, usually via some sort of radar. The friendly player's position is taken into account both when looking for friends and when looking for enemies.

When a player is looking for friends, he targets the closest friend and moves in the direction that is closest to the friend's location.

When a player is looking for enemies, the influence map technique is used: Each direction where the player could go is assigned an influence value, and the direction with the least influence is the direction where the player moves to. The influence value
is calculated by summing the negative distance of all friendly players that are nearby, leading to a higher influence of players that are nearby.

The reason to use this technique is that players who are looking for enemies should not move towards each other, as it is more efficient to look for enemies by spreading out.

## Small Tricks
Some small adjustments to the planned behavior are made:

- A player that is hurt and following a team mate can not be followed by another team mate. This prevents two hurt player to stay at the same place, moving back and forth.
- A player never moves backwards (to the tile he was on before) unless he ends up in a dead end.

## Method Analysis
Because the map is unknown to the players, path finding could not be used (path finding is explored in detail in [assignment 1](https://github.com/furgerf/304cr-assignment-1)). This may lead to strange AI movement, which is dependent on the actual
map. It appears that the randomly-generated maps are quite difficult for the AI entities. A way around this problem would be to use path finding on a partial map that expands with the player's map exploration. However, this was deemed to be overly complicated, as path finding is not requested in tis assignment.

Another extension of the game related to this problem would be to give the AI players a memory of recently-visited tiles, and having them chose tiles they have not visited (often), rather than making decisions based on the influence map. However, this
approach has drawbacks as well, for instance is it possible for several AI entities of the same team to always move in the same direction.

Using the influence maps helps the players of opposing teams find each other. This quite simple method proved to be very effective at increasing the game experience.

Due to the limited number of possible actions that can be taken, the two methods (FSM and DT) may look fairly similar. However, already here, it was clear that DT is a lot simpler, not in the sense of the underlying algorithm, but in the usage of the
algorithm. As will be discussed in more detail in the [implementation file](https://github.com/furgerf/304cr-assignment-2/blob/master/IMPLEMENTATION.md), building the DT is a very general and thus re-usable problem. After the tree is built, all that is
left to implement is how the actions are carried out. Thus, the tree can be created before the game starts (preserving CPU resources during the game) and the decision-making and the reaction to the decisions can be separated nicely.

A further advantage of DT is that each entity can have its own DT, giving the players individual behavior. This could be achieved for example by randomly assigning different decisions for the same player status (e.g. cautious players that do not attack when injured and kamikaze players that disregard their health entirely).

One of the main issues that was anticipated is that I have not programmed any games before or know any game engines. Thus, the game was not designed to be overly complex, as the complete game had to be written from scratch.

