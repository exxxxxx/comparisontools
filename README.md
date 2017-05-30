## Project Description
Visual Studio Comparison Tools is a add-in for Visual Studio which uses external tools to compare files, folders and clipboard. Features: Comparing two files, selecting folders for comparison from the solution explorer and comparing (and merging) clipboard to a file or selected area in a file

Visual Studio Gallery Page: [https://marketplace.visualstudio.com/items?itemName=MikkoHalttunen.VisualStudioComparisonTools](https://marketplace.visualstudio.com/items?itemName=MikkoHalttunen.VisualStudioComparisonTools)

**Prerequisites**
* Requires a comparison util such as WinMerge or Beyond Compare
* Tested with WinMerge and Beyond Compare 3 & 4. Write to the comments if you encounter problems setting up other tools!

**Extension Installation (For VS 2012/2013/2015/2017)**
* Install through visual studio Extension Manager or by double clicking the vsix file

**Extension Installation (For VS 2010) Build 3.1.40**
* Double click the vsix file of  found from [https://github.com/exxxxxx/comparisontools/raw/master/Publish/2010/VSCompTools.vsix]

**Add-In Installation (For VS 2005/2008) Build 2.2**
* Download the installation package from [https://github.com/exxxxxx/comparisontools/raw/master/Publish/2008/VisualStudioComparisonToolsSetup.msi]
* Running the setup will copy files "VisualStudioComparisonTools.dll" and "VisualStudioComparisonTools.addin" to Visual Studio's add-in directory. The VisualStudioComparisonTools add-in can be used next time Visual Studio is restarted.

**Usage**
* **Compare to Clipboard** has two different compare modes:
	* **Clipboard to File** comparison
		* Right click text in the editor and select "Compare with clipboard". The active file will be saved.
		* Winmerge will open and changes from the clipboard (right panel, read only) can be merged to the file (left panel).
		* Saving will save the changes to the real file.
	* **Clipboard to Text** editor selection comparison
		* Select text in a file. Right click the selected text and select "Compare with clipboard". The active file will be saved.
		* Winmerge will open and changes from the clipboard (right panel, read only) can be merged to the selection (left panel).
		* Saving and exiting winmerge will merge the changes inside the selection to the file where the selection was.
* **Compare selected files** shows up when two files are selected in the solution explorer. 
	* The comparison is done totally in the merge tool and both files are editable.
* **Compare selected folders** shows up when two directories are selected in the solution explorer.
	* The comparison is done totally in the merge tool.

**Configuration for Add-In**
* Configuration file can be found from Environment.SpecialFolder.CommonApplicationData \ VisualStudioComparisonTools\ config.xml
	* (In Vista and Windows 7: C:\ProgramData\Visual Studio Comparison Tools)
* Comparison tool exe path
	* Doesn't anymore need to be WinMerge
	* Default: C:\Program Files (x86)\WinMerge\WinMergeU.exe
	* If not found from the default folder, tries to find it from both Program Files or Program Files (x86)
* Parameters to the comparison tool (see example from the configuration file)
	* Tags which will be replaced by the add-in: 
		* {"[%File1%](%File1%)"} First filename to compare
		* {"[%File2%](%File2%)"} Second filename to compare
		* {"[%SELECTION_FILENAME%](%SELECTION_FILENAME%)"} (file name used in the title in winmerge, when comparing selection from a file instead of the full file
		* {"[%SELECTION_TITLE%](%SELECTION_TITLE%)"} WinMerge's title arguments when comparing selection of a file
		* {"[%CLIPBOARD_TITLE%](%CLIPBOARD_TITLE%)"} WinMerge's title arguments when comparing clipboard
* VisualStudioComparisonTools will use system temp directory to store temporary files, but it can be configured to store them solution's directory under _VisualStudioComparisonTools folder

**Problems with Add-In**
* If the "Compare to Clipboard" text doesn't appear in the right click context menu, try the following:
	* Go to the command prompt. Go to the location of devenv.exe (for example "C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE")
	* Write "devenv.exe /resetaddin VisualStudioComparisonTools.Connect" and press enter.
* If you encounter any problems, copy the file "VisualStudioComparisonTools.dll.log4net" and log4net.dll from the application directory to the location of devenv.exe. A VisualStudioComparisonTools-log.txt file will be created.
	* Visual studio 2017:
		* C:\Program Files\Microsoft Visual Studio 15.0\Common7\IDE
		* C:\Program Files (x86)\Microsoft Visual Studio 15.0\Common7\IDE
    * Visual studio 2015:
		* C:\Program Files\Microsoft Visual Studio 14.0\Common7\IDE
		* C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE
    * Visual studio 2013:
		* C:\Program Files\Microsoft Visual Studio 12.0\Common7\IDE
		* C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE
	* Visual studio 2012:
		* C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE
		* C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE
	* Visual studio 2010:
		* C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE
		* C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE
	* Visual studio 2008:
		* C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE
		* C:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE
	* Visual Studio 2005
		* C:\Program Files\Microsoft Visual Studio 8\Common7\IDE
		* C:\Program Files (x86)\Microsoft Visual Studio 8\Common7\IDE

**Other used libraries**
* log4net - [http://logging.apache.org/log4net/index.html](http://logging.apache.org/log4net/index.html)