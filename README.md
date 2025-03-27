# DependencyExplorer - An open-source modern Dependency Walker

## Overview
**DependencyExplorer** is a fork of the [**Dependencies**](https://github.com/lucasg/Dependencies) project by [**Lucas Georges**](https://x.com/_lucas_georges_).
It is a rewrite of the legacy software [Dependency Walker](http://www.dependencywalker.com/) which was shipped along Windows SDKs, but whose development stopped around 2006.

**DependencyExplorer** can help Windows developers troubleshooting their dll load dependencies issues.

<p align="center">
<img alt="Usage Exemple" src="screenshots/UsageExemple.gif"/>
</p>

### [Download here](https://github.com/mariusbancila/DependencyExplorer/releases/tag/v1.12)

NB : due to [limitations on /clr compilation](https://msdn.microsoft.com/en-us/library/ffkc918h.aspx), `DependencyExplorer` needs [Visual C++  Redistributable](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads) installed to run properly.

## Releases (forked version)
* [v1.12](https://github.com/mariusbancila/DependencyExplorer/releases/tag/v1.12)
	* UI improvements:
		* individual font settings (family, size, style) for modules tree, modules list, imports list, and exports list
		* added overlays for architecture types (displays 86, 64 on the right side of the icon); optional, controlled with a user setting
		* replaced error rectangle overlays with images (displays E for error on the left side of the icon)
		* incorrect checksum displayed in red (and tooltip instead of explanatory text)
		* new modules tree context menu commands: copy file path, open in explorer
	* Bugfixes:
		* prevent duplicates in most recent files

## Releases (original version)
* [v1.11](https://github.com/lucasg/Dependencies/releases/download/v1.11.1/Dependencies_x64_Release.zip) :
	* lots of bugfixes and incremental improvements
	* covid pandemic
* [v1.10](https://github.com/lucasg/Dependencies/releases/download/v1.10/Dependencies_x64_Release.zip) :
	* lots of bugfixes and incremental improvements
	* support of Windows 8.1 apisets parsing
* [v1.9](https://github.com/lucasg/Dependencies/releases/download/v1.9/Dependencies_x64_Release.zip) :
	* Display imports and exports the way Depends.exe does.
	* Added user customization for search folders and working directory
	* Added LLVM demangler to availables symbol demangling
	* Fixed Wow64 FsRedirection bugs
	* F5 can now refresh the analysis
	* Added CLR assembly dependencies enumeration
	* Added a packaging option without Peview.exe (which triggers some AV).
* [v1.8](https://github.com/lucasg/Dependencies/releases/download/v1.8/Dependencies_x64_Release.zip) :
	* Add x86/x64 variants for Dependencies
* [v1.7](https://github.com/lucasg/Dependencies/releases/download/v1.7/Dependencies.zip) :
	* Add CLI tool "dependencies.exe"
* [v1.6](https://github.com/lucasg/Dependencies/releases/download/v1.6/Dependencies.zip) :
	* Add appx packaging
* [v1.5](https://github.com/lucasg/Dependencies/releases/download/v1.5/Dependencies.zip) :
	* Support of Sxs parsing
	* Support of api set schema parsing
	* API and Modules list can be filtered
* [v1.0](https://github.com/lucasg/Dependencies/releases/download/v1.0/Dependencies.zip) -- Initial release


## Installation and Usage

`DependencyExplorer` is currently shipped as two binaries (no installer present) : `Dependencies.exe` as a CLI tool and `DependenciesGui.exe` for its GUI counterpart (see screenshot). Just click on one of the release numbers above (preferably the latest), download and uncompress the archive and run `DependenciesGui.exe`.
Since the binary is not signed, `SmartScreen` might scream at runtime. `DependencyExplorer` also bundle `ClrPhTester.exe`, a dumpbin-like executable used to test for non-regressions.

`DependencyExplorer` currently does not recursively resolve child imports when parsing a new PE since it can be really memory-hungry to do so ( it can over a GB even for "simple" PEs ). This behavior can be overridden (app-wide) via a property located in "Options->Properties->Tree build behaviour".

<p align="center">
<img alt="User options" src="screenshots/UserOptions.png"/>
</p>

Tree build behaviours available :

* `ChildOnly` (default) : only process PE child imports and nothing beyond.
* `RecursiveOnlyOnDirectImports`  : do not process delayload dlls.
* `Recursive` : Full recursive analysis. You better have time and RAM on your hands if you activate this setting :

<p align="center">
<img alt="Yes that's 7 GB of RAM being consumed. I'm impressed the application didn't even crash" src="screenshots/RamEater.PNG"/>
</p>


## Limitations

At the moment, `DependencyExplorer` recreates features and "features" of `depends.exe`, which means :

* Only direct, forwarded and delay load dependencies are supported. Dynamic loading via `LoadLibrary` are not supported (and probably won't ever be).
* Support of api set schema redirection since 1.5
* Checks between Api Imports and Exports. 
* Minimal support of sxs private manifests search only.


## Building

Building is straightforward:

* Clone the repo.
* Open the `Dependencies.sln` solution in Visual Studio.
* Select the configuration (`Debug` or `Release`) and the platform (`x86` or `x64`).
* Build the solution.


## Credits and licensing

Special thanks to :

* [ProcessHacker2](https://github.com/processhacker2/processhacker) for :
  * `phlib`, which does the heavy lifting for processing PE informations.
  * `peview`, a powerful and lightweight PE informations viewer.
* [Dragablz](https://github.com/ButchersBoy/Dragablz) a C#/XAML library which implement dockable and dragable UI elements, and can recreate the [MDI programming model](https://en.wikipedia.org/wiki/Multiple_document_interface) in `WPF`.
* @aionescu, @zodiacon and Quarkslab for their public infos on ApiSets schema.
* [Thomas levesque's blog](https://www.thomaslevesque.com) which pretty much solved all my `WPF` programming issues. His [`AutoGridSort`](http://www.thomaslevesque.com/2009/08/04/wpf-automatically-sort-a-gridview-continued/) is used in this project 
* Venkatesh Mookkan [for it's `FilterControl` for ListView used in this project](https://www.codeproject.com/Articles/170095/WPF-Custom-Control-FilterControl-for-ListBox-ListV)
* [demumble](https://github.com/nico/demumble) for demangling GCC symbols on Windows
