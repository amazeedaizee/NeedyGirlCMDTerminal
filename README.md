# NeedyGirlCMDTerminal

This repo actually contains the source code for two programs in one:

- A BepInEx mod where the game acts as a server, accepting basic commands from one connected client as a way to control the game.
- A program where it acts as a client to the mod above; it sends these commands to the game executed from the command line.

Both of these communicate through localhost `127.0.0.1` on port 55770.

Below will explain each in more detail:

## Command Server

This is the BepInEx mod that makes the game run on a server in localhost `127.0.0.1`.
It can be useful in case other programs want to interact with the game in some way.
While running, it can accept specific commands from other programs, such as `caution ok` to accept the warning at the beginning of the game.

After a client sends a command, the server will sometimes reply with extra information, before ending its message with `>` on a new line, indicating that it's ready to receive commands again from the client.
If you plan on writing a program with this mod in mind, make sure to consider the above. Both the server and client are meant to share information with each other and wait until all information is sent/received.

Please note that this mod is slightly unstable. The game might hang if it's forcefully closed while it's in the process of sending/receiving a message to a connected client.

## Command Terminal

This is the program that can send some commands to the server.

While most commands are handled by the server, the `help` command is specifically handled by the client itself. 
The `help` command provides a way of explaining to the user some extra info if needed while using the console.
This is done by the server sending `?>` on a new line. This program does not capture `?>` or `>` in the console when the server sends them.

If yopu want to make a client yourself, it's up to you whether or not you want to include a `help` command.

# Commands

Here are the list of commands that's supported in the server:

> [!NOTE]
> To use the `debug` command, your game has to have Debug Mode enabled. 

- action
- caution
- cut
- debug
- ending
- info
- jine
- load
- login
- notification
- open
- options
- picture
- read
- reload
- reset
- shutdown
- stream
- tweeter
- unzip
- video
- webcam 
- window

If you want some more in-depth information for each command, feel free to take a look in the `tipstricks.md` file.
