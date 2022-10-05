# We're still very early in development,<br/>please don't expect much yet. Thank you

<img height="120" src="https://user-images.githubusercontent.com/8362329/189507308-8887d4e0-94d8-4380-9520-8c00ff48fb6e.png"/>

The new Tivoli made by cute fairies https://tivoli.space

Steam keys will be available sometime. In the mean time, dev builds are also [uploaded to Itch.io here](https://makifoxgirl.itch.io/tivolispace)

See our [Trello page](https://trello.com/b/za4VZKkl/tivoli-space)!

## Using a second Steam account for testing

<!--
-   Create a second user in Windows settings

-   `runas /user:Lobstertje /savecred "C:\Program Files (x86)\Steam\steam.exe"`

this doesnt work for steam lmao what!!!

it uses "C:\Program Files (x86)\Steam\config" instead of user appdata

-->

Using a second Windows user doesn't work with Steam. ¯\\\_(ツ)\_/¯

-   Download **Sandboxie Plus** from https://sandboxie-plus.com/downloads (it's open source now!)

-   **Sandbox > Create New Box** and call it **SteamForTesting**

-   Download **Steam** from https://store.steampowered.com/about/

-   **SteamForTesting > Run > Run Program** and select SteamSetup.exe

    -   You'll be asked to close Steam which is fine

    -   You'll receive notifications on Sandboxie, remember choice and accept

    -   When it gets stuck on **creating shortcut**, press **SteamForTesting > Terminate All Programs**

-   **Start Steam** using `"C:\Program Files\Sandboxie-Plus\Start.exe" /box:SteamForTesting "C:\Sandbox\Maki\SteamForTesting\drive\C\Program Files (x86)\Steam\steam.exe"`

-   **Start Tivoli builds** using `"C:\Program Files\Sandboxie-Plus\Start.exe" /box:SteamForTesting "Tivoli Space.exe"`
