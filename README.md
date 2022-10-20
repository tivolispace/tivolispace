# We're still very early in development,<br/>please don't expect much yet. Thank you

<img height="120" src="https://user-images.githubusercontent.com/8362329/189507308-8887d4e0-94d8-4380-9520-8c00ff48fb6e.png"/>

The new Tivoli made by cute fairies https://tivoli.space

Steam keys will be available sometime. In the mean time, dev builds are also [uploaded to Itch.io here](https://makifoxgirl.itch.io/tivolispace)

See our [Trello page](https://trello.com/b/za4VZKkl/tivoli-space)!

## Requirements

-   Final IK 2.1 is needed. Just add to the client, it'll be git ignored

## Using a second account for testing

Running Steam twice on one machine is really difficult, so instead you need to set an environment variable before running.

-   Go to https://tivoli.space/api/auth/steam and login

-   Copy the token and set it using `set AUTH_TOKEN=`

-   Run `Tivoli Space.exe`, it will skip logging in with a Steam auth ticket

    -   Eventually you'll be able to disable VR too but for now just build without OpenXR in project settings

    -   If you want to login with a different backend, also supply `set OVERRIDE_API_URL=`

    -   Feel free to place .bat files in git root dir, it's ignored!
