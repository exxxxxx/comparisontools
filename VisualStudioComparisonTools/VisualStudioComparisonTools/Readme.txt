VisualStudioComparisonTools

Copyright 2008 Mikko Halttunen
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; version 2 of the License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

Prerequisites
    * Requires WinMerge executable to be installed in "C:\Program Files\WinMerge\WinMergeU.exe"

Installation
    * Running the setup will copy files "VisualStudioComparisonTools.dll" and "VisualStudioComparisonTools.addin" to Visual Studio's add-in directory. The VisualStudioComparisonTools add-in can be used next time Visual Studio is restarted.

Usage
    * Compare to Clipboard has two different compare modes:
          o Clipboard to File comparison
                + Right click text in the editor and select "Compare with clipboard". The active file will be saved.
                + Winmerge will open and changes from the clipboard (right panel, read only) can be merged to the file (left panel).
                + Saving will save the changes to the real file.
          o Clipboard to Text editor selection comparison
                + Select text in a file. Right click the selected text and select "Compare with clipboard". The active file will be saved.
                + Winmerge will open and changes from the clipboard (right panel, read only) can be merged to the selection (left panel).
                + Saving and exiting winmerge will merge the changes inside the selection to the file where the selection was.
    * Compare selected files shows up when two files are selected in the solution explorer.
          o The comparison is done totally in the merge tool and both files are editable.
    * Compare selected folders shows up when two directories are selected in the solution explorer.
          o The comparison is done totally in the merge tool.

Other information
    * VisualStudioComparisonTools will use solution's directory to store temporary files.
          o If a solution isn't open, the system's temporary directory will be used.
          o In normal usage the temporary files should be deleted.

Problems
    * If the "Compare to Clipboard" text doesn't appear in the right click context menu, try the following:
          o Go to the command prompt. Go to the location of devenv.exe (for example "C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE")
          o Write "devenv.exe /resetaddin VisualStudioComparisonTools.Connect" and press enter.
    * If you encounter any problems, copy the file "VisualStudioComparisonTools.dll.log4net" from the application directory to the location of devenv.exe.
          o Visual Studio 2005
                + C:\Program Files\Microsoft Visual Studio 8\Common7\IDE
          o Visual studio 2008:
                + C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE

Other used libraries
    * log4net - http://logging.apache.org/log4net/index.html

Known bugs
    * Always after opening visual studio, the right click menu has both Compare to clipboard and Compare files in the context menu.
          o If two files are selected, the compare to clipboard will not do anything.
