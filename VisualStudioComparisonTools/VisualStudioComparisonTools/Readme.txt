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

----------

Visual Studio Comparison Tools is a add-in for Visual Studio which uses external tools to compare files, folders and clipboard. Features: Comparing two files, selecting folders for comparison from the solution explorer and comparing (and merging) clipboard to a file or selected area in a file

Prerequisites

    * Requires a comparison util such as WinMerge
    * Tested only with WinMerge. Write to the comments if you encounter problems setting up other tools!


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


Configuration

    * Configuration file can be found from Environment.SpecialFolder.CommonApplicationData \ VisualStudioComparisonTools\ config.xml
          o (In Vista and Windows 7: C:\ProgramData\Visual Studio Comparison Tools)
    * Comparison tool exe path
          o Doesn't anymore need to be WinMerge
          o Default: C:\Program Files (x86)\WinMerge\WinMergeU.exe
          o If not found from the default folder, tries to find it from both Program Files or Program Files (x86)
    * Parameters to the comparison tool (see example from the configuration file)
          o Tags which will be replaced by the add-in:
                + [%File1%] First filename to compare
                + [%File2%] Second filename to compare
                + [%SELECTION_FILENAME%] (file name used in the title in winmerge, when comparing selection from a file instead of the full file
                + [%SELECTION_TITLE%] WinMerge's title arguments when comparing selection of a file
                + [%CLIPBOARD_TITLE%] WinMerge's title arguments when comparing clipboard
    * VisualStudioComparisonTools will use system temp directory to store temporary files, but it can be configured to store them solution's directory under _VisualStudioComparisonTools folder


Problems

    * If the "Compare to Clipboard" text doesn't appear in the right click context menu, try the following:
          o Go to the command prompt. Go to the location of devenv.exe (for example "C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE")
          o Write "devenv.exe /resetaddin VisualStudioComparisonTools.Connect" and press enter.
    * If you encounter any problems, copy the file "VisualStudioComparisonTools.dll.log4net" and log4net.dll from the application directory to the location of devenv.exe. A VisualStudioComparisonTools-log.txt file will be created.
          o Visual Studio 2005
                + C:\Program Files\Microsoft Visual Studio 8\Common7\IDE
                + C:\Program Files (x86)\Microsoft Visual Studio 8\Common7\IDE
          o Visual studio 2008:
                + C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE
                + C:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE
    * Visual studio 2010 support coming soon
    * Problems with web site projects: Menu items not visible always
    * Cannot compare files between projects


Other used libraries

    * log4net - http://logging.apache.org/log4net/index.html
