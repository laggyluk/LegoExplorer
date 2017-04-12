# LegoExplorer
Software part of my Lego buggy. Android phone is mounted on the buggy and controls the brick via Bluetooth. It's camera is used to stream image to second instance of this app that controls the whole thing on PC or another android phone via Wifi.
Made in Unity.

usage: On the device that is supposed to be controlling the brick first pair them then launch app and go to menu > set role to "buggy", then Menu > Bluetooth > Connect.
On the controlling device set role "controller" in menu and that's it

There is a button for enabling remote phone torchlight but the code that is supposed to do so doesn't work on my device. Still I've left the button in place.

this project uses:
https://github.com/dyadica/Unity_EV3
https://github.com/RevenantX/LiteNetLib
https://github.com/laggyluk/UnityWebCamUdpStream
