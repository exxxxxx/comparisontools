VisualStudioComparisonTools 1.1.1 - Copyright 2008 Mikko Halttunen

*Prequisites*
-Requires WinMerge executable to be installed in "C:\Program Files\WinMerge\WinMergeU.exe"

*Installation*
-Setup has copied files "VisualStudioComparisonTools.dll" and "VisualStudioComparisonTools.addin" to visual studio add-in directory.
The VisualStudioComparisonTools add-in can be used next time Visual Studio is restarted.

*Usage*
-Compare to Clipboard has two different compare modes:
 * Real file to Clipboard comparison
Right click text in the editor and select "Compare with clipboard". The active file will be saved.
Winmerge will open and changes from the clipboard (right panel, read only) can be merged to the file (left panel).
Saving and will save the changes to the real file.
 * Selection to Clipboard comparison
Select text in a file. Right click the selected text and select "Compare with clipboard". The active file will be saved.
Winmerge will open and changes from the clipboard (right panel, read only) can be merged to the selection (left panel).
Saving and exiting winmerge will merge the changes inside the selection to the file where the selection was.
Changes to that file while in winmerge will be lost.
-Compare selected files shows up when two files are selected in the solution explorer. 
The comparison is done totally in the merge tool and both files are editable.
-Compare selected folders shows up when two directories are selected in the solution explorer.
The comparison is done totally in the merge tool.

*Other information*
-VisualStudioComparisonTools will use solution's directory to store temporary files. 
If a solution isn't open, the systems temporary directory will be used.
In normal usage the temporary files should be deleted.

*Problems*
-If the "Compare to Clipboard" text doesn't appear in the right click context menu, try the following:
Go to the command prompt. Go to the location of devenv.exe (for example "C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE")
Write "devenv.exe /resetaddin VisualStudioComparisonTools.Connect" and press enter.

-If you encounter any problems, copy the file "VisualStudioComparisonTools.dll.log4net" from the application directory to the location of devenv.exe. 
Devenv.exe can be found from:
 * Visual Studio 2005
C:\Program Files\Microsoft Visual Studio 8\Common7\IDE
 *Visual studio 2008:
C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE

*Other used libraries*
log4net - See "log4net.LICENSE.txt"

*Known bugs*
-Always after opening visual studio, the right click menu has both compare to clipboard and compare files in the context menu. 
If two files are selected, the compare to clipboard will not do anything.