# Chicory Macaroni

This script automates the process of converting the Mac edition of Chicory: A Colorful Tale to be runnable on Windows.

The main reason to do this is that the Windows edition is compiled using YYC which makes game scripts uneditable, however the Mac edition somehow wasn’t compiled with YYC.

If you have trouble getting this to work, check out our [Chicory Modding Discord](https://data.chicory.pizza/discord)!

## How to use

First of all, **make a backup of your save!!** You can find it at `%localappdata%\paintdog\`, zip it and upload to your Dropbox/Google Drive/whatever for safekeeping.

1. Obtain the Windows and Mac editions of the game, you need both `data.win` and `game.ios`

   If you don’t have a spare Mac, you can use alternative tools mentioned at [https://www.speedrun.com/hollowknight/guide/jlten](https://www.speedrun.com/hollowknight/guide/jlten) to download the Mac edition

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

<details>
<summary>Troubleshooting</summary>

The script was originally built for UndertaleModTool v0.4.0.4, using older or newer versions may or may not work correctly.

The script was tested against the game data files of Windows edition v1.0.0.59 and Mac edition v1.0.0.57, using other versions may or may not work correctly.

The script is only tested on Windows and the UndertaleModTool GUI, using other operating systems or the CLI are not guaranteed to work.

</details>

<details>
<summary>Manual steps</summary>

Everything that the automated script does can be done manually if you have trouble with the script, please do file a GitHub issue or contact our Discord so the script can be fixed though!

1. Obtain the Windows and Mac editions of the game

2. Merge the shaders from the Windows edition to the Mac data

   2. Open `data.win` (Windows edition) in UndertaleModTool
   3. Click Scripts > Unpack assets > ExportShaderData.csx
   4. Select an export folder
   5. Open `game.ios` (Mac edition) in UndertaleModTool
   6. Click Scripts > Repack assets > ImportShaderData.csx
   7. Select the previous export folder
   8. Save the newly modified data as `macaroni.win` next to `data.win` (Windows edition)

3. Obtain GameMaker runner version 2.3.2.426

   1. Download the Windows edition of GameMaker at [https://accounts.yoyogames.com/downloads](https://accounts.yoyogames.com/downloads), you need a free YoYo Account
   2. Once GameMaker is installed and running, click File > Preferences
   3. Go to Runtime Feeds > Master
   4. Install version 2.3.2.426
   5. Go to `C:\ProgramData\GameMakerStudio2\Cache\runtimes\runtime-2.3.2.426\windows` and copy `Runner.exe` next to Chicory's `data.win`

4. Create a batch file with this contents and save as `RunMacaroni.bat` inside the game folder

   ```
   start .\Runner.exe -game macaroni.win
   ```

To run the game, just open `RunMacaroni.bat`

</details>

## How to tell it’s working

- Game icon is a generic GameMaker logo
- Title bar says “Chicory_A_Colorful_Tale” (underscores)
- Game version on the main menu is v1.0.0.57

## How to revert

Delete `macaroni.win`, `RunMacaroni.bat` and `Runner.exe`

If you want to be really sure, you can also use Steam’s “verify integrity of game files” feature and restore your saved game backup (you did make a backup, right?)

## Why this name

I asked on the modding Discord for a good food pun, and Somewhat immediately suggested Macaroni, because Mac-and-Windows, get it...?

## Special thanks

- [JdavisBro](https://github.com/JdavisBro) for figuring all this out, this script just automates most of the steps to be less tedious
- Undertale modding community for [UndertaleModTool](https://github.com/krzys-h/UndertaleModTool)

## License

GPL v3.0, same as UndertaleModTool
