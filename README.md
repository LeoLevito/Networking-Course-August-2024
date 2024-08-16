Controls: 
	-WASD, move tank.
	-Spacebar, shoot projectile.

Relevant files:
	Assets/Input/
	-InputReader.cs, very similar or possibly identical to Cornelius's InputReader script from the first lecture.

	Assets/Prefabs/
	- DropdownCanvas, Canvas with Dropdown menu attached.
	- NetworkManager, prefab with NetworkManager component attached. Is placed in main scene.
	- Player, player prefab, has some sprites which makes it look like a little tank, has a collider and a rigidbody. NetworkObject, NetworkTransform, and NetworkRigidbody are attached components as well.
	- Projectile, projectile prefab, has a collider and a rigidbody. NetworkObject, NetworkTransform, and NetworkRigidbody are attached components as well.

	Assets/Scripts/
	- EmoticonUI.cs, code handles updates of the emoticon text displayed above player tanks.
	- NetworkManagerUI.cs, code adds GUI buttons to top left corner of the screen to host, join or quit the game.
	- Player.cs, code for player movement, spawning of projectiles, player health, player death and respawn. Along with Emoticon text display position updates above the player.
	- Projectile.cs, code for projectile movement, collisions, projectile destroy timers.

	Assets/
	- DefaultNetworkPrefabs, this is where both the Player and Projectile Prefabs are placed for the NetworkManager to replicate across the network, if I've understood correctly.
