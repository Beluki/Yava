
## About

Yava is a small emulator/game launcher for Windows. In short: it's a convenient
menu to run all your games from a single place. It's also portable, meaning you
can put Yava and all your emulators/roms in a usb key and carry them anywhere.

Here is a screenshot:

![Screenshot1](https://raw.github.com/Beluki/Yava/master/Screenshot/Screenshot1.png)

Features:

* Simple, fast, lightweight, robust.

* Easy to configure through an ini file. The file can be reloaded from the GUI.

* It doesn't try manage your roms or impose any particular organization.
  In fact, it doesn't even keep an internal database, just uses folders and files.

* It can launch anything, with any command-line parameters. Emulators tested
  include Bizhawk, DesMuMe, Dolphin, Fceux, Kega, Mame, DosBox...

* Provides hotkeys to close the launched emulators, both properly and forcefully.

* Remembers the last selected file for each folder.

* You can "search as you type" like in Windows Explorer. Multiple folders can be
  selected, so it's possible to search the entire rom collection.

* A single exe with no dependencies other than the .NET Framework 4.0.

## The folders file

Yava is configured using a INI file, named "Folders.ini". This file is located in
the same folder as Yava.exe and contains everything Yava needs to know about the
folders and files it will launch.

Here is an example:

```ini
[Genesis]
path = Romset\Genesis
executable = Emulators\Kega Fusion\Fusion.exe

[Super Nintendo]
path = Romset\Super Nintendo
executable = Emulators\Snes 9x\snes9x.exe
```

Each section in the INI file represents a folder in Yava's left panel.

Here are all the available specifications for a section:

* `path`: where the files for this folder are located in the filesystem. Required.

* `executable`: what program to use to launch the files. Required.

* `extensions`: a list of extensions to filter the files with, comma separated.
   Optional (Yava will display all files by default). Example: `zip, 7z, smc`

* `parameters`: additional command-line arguments to add to the executable.
   Optional: `"%FILEPATH%"` by default. Example: `"%FILEPATH%" --video fullscreen`

* `workingdirectory`: the startup path for the executable.
   Optional: Yava will use the same folder where the executable is located by default.

All the paths can be either absolute or relative.
When specifying `executable`, `parameters` and `workingdirectory`, you can use %FILEPATH%
and %FOLDERPATH% to refer to the current file being launched and the folder it belongs to
respectively.

Here is a more complete example, for the Dolphin emulator:

```ini
[Wii]
path = Romset\Wii
executable = Emulators\Dolphin 4.0.3\Play\Dolphin.exe
parameters = --batch --exec "%FILEPATH%"
extensions = iso, wbfs
```

A complete Folders.ini (the one I use) is available in the [Extra][] folder
in the repository.

[Extra]: https://github.com/Beluki/Yava/tree/master/Extra

## Compiling and installation

Building Yava is a matter of opening the included Visual Studio 2010
solution and clicking the build button (or using msbuild). The source code
has no dependencies other than the [.NET Framework][] 4.0+.

There are binaries for the latest version in the [Releases][] tab above.

Yava doesn't need to be installed. It can run from any folder and doesn't
write to the Windows registry. It's possible to run it from an usb stick
provided the .NET Framework is available on the target machine.

[.NET Framework]: http://www.microsoft.com/en-us/download/details.aspx?id=30653
[Releases]: https://github.com/Beluki/Yava/releases

## Keyboard shortcuts

In Yava itself:

   Key     | Use
:--------: | :----------------------------------------------------------
   Tab     | Change between the left and right panel.
    F5     | Reload the Folders.ini file.

While running a game:

   Key                      | Use
:-------------------------: | :-----------------------------------------------------------
   Control + Shift + C      | Close the game, properly (equivalent to closing the window).
   Control + Shift + K      | Close the game, forcefully (kills the game process).

## Portability

Yava is tested on Windows 7 and 8, using the .NET Framework 4.0+.
[Mono][] is not supported.

The folders file encoding is UTF-8 with or without a BOM signature. Notepad
will work, although I suggest something better such as [Notepad2][] or
[Sublime Text][].

[Mono]: http://mono-project.com
[Notepad2]: http://www.flos-freeware.ch/notepad2.html
[Sublime Text]: http://www.sublimetext.com

## Status

This program is finished!

Yava is feature-complete and has no known bugs. Unless issues are reported
I plan no further development on it other than maintenance.

## License

Like all my hobby projects, this is Free Software. See the [Documentation][]
folder for more information. No warranty though.

[Documentation]: https://github.com/Beluki/Yava/tree/master/Documentation

