# DCM Analytics
A public-facing repository containing the code used for gameplay telemetry in
the game [Iris Burning](https://irisburning.com/), licensed under the Apache
License version 2.0.

Iris Burning is a trademark of Die-Cast Magic Studios, LLC.  Copyright 2017 -
All rights reserved.  We release any and all warranty, express or implied, 
related to this code or the results of running this code, and assert no
responsibility for any damage incurred in relation to downloading, compiling,
linking, modifying, and/or executing this code in source or binary form.

This code is thoroughly documented through comments so that it's easy to see
exactly what is going on under the hood.

## Why is this here?
In the interest of users' privacy, we want to expose exactly what kind of data
and events we are phoning home with for gameplay metrics.  Specifically, we can
always *say* we're sending home gameplay data, but users will have no way of
knowing exactly what is being sent to us without packet-sniffing their own
internet connections attempting to reverse-engineer the game.

As Iris Burning's code is obfuscated to deter reverse-engineering, this section
is one of the few that is specifically ignored by the obfuscation process so
that users who are paranoid can match the code found in the game 1:1 with the
code found in this repository, and thus verify that the information we are
sending back is not personally identifiable in any way.

Developers on the Iris Burning team are forbidden from even calling the 
GameAnalytics calls directly and must use calls found inside this repository.

## How do I run this code?
There are a few hurdles you'll need to get out of the way.  Firstly, Iris 
Burning is feature-frozen on Unity version 5.5.0f3, so your results may vary
based on any API changes found in later versions of the Unity engine.

Secondly, we are using the latest build of the Unity 5 GameAnalytics repository.
This repository has been forked by us so that a feature-frozen version GA's code
can be matched 1:1 with this repository, and can be found [here](https://github.com/DieCastMagicStudios/GA-SDK-UNITY).

Thirdly, two classes are called by this code that are part of the Iris Burning
main game code: `IrisPlayer` and `DeveloperConsole`.  The former is what holds
player state information and global state information about the current game
session; the latter is simply a static class for interfacing with the ingame
IBASIC developer console in Iris Burning.

These classes are not included in this repository, but you can run this code by
substituting any call to `IrisPlayer.current.WinCondition` (the only reason 
`IrisPlayer` is referenced is for its storage of the current global win 
condition) with any `int` of your choice;<sup>†</sup> and substitute any call to
`DeveloperConsole` using the following mappings:

| Iris Burning API Call    	| Unity 5 API Call   	|
|--------------------------	|--------------------	|
| `DeveloperConsole.Print` 	| `Debug.Log`        	|
| `DeveloperConsole.Warn`  	| `Debug.LogWarning` 	|
| `DeveloperConsole.Error` 	| `Debug.LogError`   	|

This will print any messages, warnings, or errors to the Unity console as they
would be printed to the Iris Burning console.

Finally, you should ensure you are running the latest version of [Photon Unity
Networking](https://www.assetstore.unity3d.com/en/#!/content/1786) as this code
relies on the Photon global states to determine if it's a multiplayer or
singleplayer match.

<sup>†</sup> The default win condition in Iris Burning, at the time of writing,
is 5000. 