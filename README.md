# UniGit
An open source GIT Unity3D editor plugin.

[![GitHub release](https://img.shields.io/github/release/simeonradivoev/UniGit.svg)](https://github.com/simeonradivoev/UniGit/releases)
[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](https://github.com/simeonradivoev/UniGit/blob/master/LICENSE.md)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4A4LQGA69LQ5A)

![UniGit Icon](https://github.com/simeonradivoev/UniGit/raw/master/Images/UnityGitIcon.png)

# Contents
* [Features](#features)
* [Screenshots](#screenshots)
* [Building](#building)
* [Limitations](#limitations)
* [Not implemented yet](#not-implemented-yet)
* [Unity Thread](https://forum.unity3d.com/threads/opensource-unigit-in-editor-git-gui.440646/)
* [Asset Store](http://u3d.as/Bxf)

# [Features:](https://github.com/simeonradivoev/UniGit/wiki/Features-and-Usage)
* Pull, Push, Merge, Fetch changes
* Remote Management
* Secure Credentials Manager
* Project View status icons
* Open Source
* Conflict resolvement 
* Support for External programs like Tortoise Git
* Support for Credential Managers like Windows Credentials Manager
* (Beta) Support for Git LFS
* Multi-Threaded support
* Branch Switching and Creation
* In-Editor Diff Inspection
* Git Log Window

For more info on all the fetures and how to use them, chek out the [wiki](https://github.com/simeonradivoev/UniGit/wiki/Features-and-Usage).

# Screenshots
### History Window
![Git history window](https://github.com/simeonradivoev/UniGit/raw/master/Images/HistoryScreenshot.png)
### Diff Window
![Git Diff Window](https://github.com/simeonradivoev/UniGit/raw/master/Images/DiffScreenshot.png)
### Project View status overlays
![Project View Overlays](https://github.com/simeonradivoev/UniGit/raw/master/Images/ProjectScreenshot.png)
### Diff Inspector
![Diff Inspector](https://github.com/simeonradivoev/UniGit/raw/master/Images/DiffInspector.png)
### Settings window
![Settings window](https://github.com/simeonradivoev/UniGit/raw/master/Images/SettingsGeneralScreenshot.png)
### Git Log
![Git Log Window](https://github.com/simeonradivoev/UniGit/raw/master/Images/GitLogScreenshot.png)

# Building
As of Unity 2017.3, [Assembly Definition files](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html) were introduced. UniGit has an assemby definition file that that unity uses to compile a dll located in `Library/ScriptAssemblies/UniGit.dll`, once the editor has compiled all the scripts.

*You can create a .unitypackage by going to `UniGit > Export Package` in Unity's top menu. Unity automatically updates all file paths and dependancy DLLs such as: UnityEngine.dll and UnityEditor.dll*

*You can build a .dll library using the provided Visual Studio 2015 project in the *UniGitVs* folder.<br>
There are also build scripts provided in the *UniGitVs* folder called `build_dev.bat` and `build_release`
All you need is to change the Path to Unity's DLLs. You can check [Unity's Managed Plugins Documentation](https://docs.unity3d.com/Manual/UsingDLL.html) for more info or you can use the built-in UniGit package exporter as mentioned above.*

Once you change the path of unity's DLLs, you can build the project. Visual studio will copy all necessary files into `UniGitVs/bin/Debug` or `UniGitVs/bin/Release` folders. These files include the UniGit icons and resources, as well as the LibGit2Sharp library and it's dependencies, so that you can quickly copy all the files and put it in your project neatly wrapped in a DLL library.

## Notes
* UniGit is developed on a windows machine and has only been tested on a windows machine.

## Limitations:
* Inbuilt Credentials Manager works on Windows only, for now.
* Pushing only works with HTTP (libgit2sharp limitation)

## Not implemented yet
* Unity scene/prefab merging
* Rebasing (with inbuilt tools)
