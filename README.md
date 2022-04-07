# Chicory Macaroni

This script automates the process of converting the Mac edition of Chicory: A Colorful Tale to be runnable on Windows.

The main reason for doing this is that the Windows edition is compiled using YYC which makes game scripts uneditable, however the Mac edition somehow wasn’t compiled with YYC.

Special thanks to [JdavisBro](https://github.com/JdavisBro) for figuring all this out, this script just automates most of the steps to be less tedious.

## How to use

First of all, **make a backup of your save!!** You can find it at `%localappdata%\paintdog\`, zip it and upload to your Dropbox/Google Drive/whatever for safekeeping.

1. Obtain the Windows and Mac editions of the game, you need both `data.win` and `game.ios`
   
   If you don’t have a spare Mac, you can use alternative tools mentioned at https://www.speedrun.com/hollowknight/guide/jlten to download the Mac edition
   
   App ID: 1123450<br />
   Depot ID: 1123452
2. Download [UndertaleModTool](https://github.com/krzys-h/UndertaleModTool/releases) and extract it, despite its name, it’s not just for Undertale!
3. Download [the script](https://github.com/chicory-pizza/chicory-macaroni/archive/refs/heads/master.zip) and extract it
4. Run UndertaleModTool
5. Click Scripts > Run other script...
6. Find and select `ConvertChicoryMacaroni.csx`
7. Select `data.win` from the **Windows** edition
8. Select `game.ios` from the **Mac** edition
9. Once finished, copy `RunMacaroni.bat` and `Runner.exe` to the Windows edition

You’re all done! You should be able to open `macaroni.win` in UndertaleModTool to start editing game scripts.

To run the game, just open `RunMacaroni.bat`

## How to tell it’s working

* Game icon is a generic Game Maker logo
* Title bar says “Chicory_A_Colorful_Tale” (underscores)
* Game version on the main menu is v1.0.0.57

## How to revert

Delete `macaroni.win`, `RunMacaroni.bat` and `Runner.exe`

If you want to be really sure, you can also use Steam’s “verify integrity of game files” feature and restore your saved game backup (you did make a backup, right?)

## Why this name

I asked on the modding Discord for a good food pun, and Somewhat immediately suggested Macaroni, because Mac-and-Windows, get it...?

## License

GPL v3.0, same as UndertaleModTool
