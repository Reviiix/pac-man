# pac-man
A recreation of the original pac-man arcade game made with Unity.

Grid System
The grid system generates works in the editor to generate a grid layout for you to create levels with. Grid items can either be blank or walls.

Movement System
The Movement System implements the Movement Jobs abstraction to control game movement.

Project Initialiser
Handles the order of game initialisation. Allows for better debugging of race conditions by giving granular control over order or initialisation. Also gives an easy way to inject dependencies.

# Abstractions
Singleton
A type singleton abstract class. Inheritors will follow the singleton pattern.

Movement Jobs
Generic movement methods using the job system. There are 2 collections, 'objects to move' and 'positions to move to'. Collections 2s values are applied to collection 1s values every frame.