# XLabyrinth
This is a C# implementation of the puzzle game Labyrinth. This is a trippy pen-and-paper board game involving treasure, combat, masonry and hamsters. Presumably, it originates somewhere in Eastern Europe and easily lends itself to being supported by some simple software. Big thanks to my friend Helge for creating a good Python implementation of it and making it popular among our friends.

This particular version of the game is an experiment in test-driven development and sensible cross-platform app architecture.

If Visual Studio complains that the solution file was created with a different version, it is probably caused by the dependency to Xamarin. The LabyrinthEngine and LabyrinthTests projects are independent of this, however. I've built the project with Visual Studio Community Edition 2015 with Xamarin.