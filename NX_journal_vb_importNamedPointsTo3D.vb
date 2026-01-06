' Journal desciption:
' This journal imports and names points from a .txt file into an open 3D part
' It will ask you to insert the full path to the .txt and will give you an example
' Each point should be on the new line, in the format:
' name, x, y, z
' Example:
' myPoint1, -56.005, 202.88, 3.1
'
' Written in VB.Net
' Tested on Siemens NX 2412.3
' v1 - 2025-12 - First created

Imports System
Imports NXOpen
Imports NXOpenUI
Imports System.IO
Imports System.Globalization

Module NXJournal
Sub Main(ByVal args() As String)

    Dim theSession As NXOpen.Session = NXOpen.Session.GetSession()
    Dim workPart As NXOpen.Part = theSession.Parts.Work
    Dim displayPart As NXOpen.Part = theSession.Parts.Display
    Dim theUISession As UI = UI.GetUI

    ' NX will decide for itself if decimal separator
    ' should be a dot "." or a comma "," based on a region
    ' that's also why in the "Convert.ToDecimal" method there is "CultureInfo.InvariantCulture"
    Dim current As CultureInfo = CultureInfo.CurrentCulture
    Dim UsCulture As New CultureInfo("en-US")
    Dim numFmt As System.Globalization.NumberFormatInfo
    numFmt = New System.Globalization.NumberFormatInfo()
    numFmt.NumberDecimalSeparator = "."

    ' get path to the file with points
	' Chr(34) = quotation marks "
    Dim inputPathTxt As String
    inputPathTxt = NXInputBox.GetInputString("Insert the full path for a .txt file with quotation marks "& Chr(34)& Chr(34), "Input Box", Chr(34)& "yourDisk:\yourPath\yourFile.txt"& Chr(34))
	inputPathTxt=inputPathTxt.TrimEnd(Chr(34)).TrimStart(Chr(34))

    Try
        Dim sr As StreamReader = New StreamReader(inputPathTxt)

        ' define delimiter as comma "," while decimal separator should be a dot "."
        Dim delim As Char() = {","}
        Dim line As String
        Dim pointInfo As String()

        Dim point As Point3d
        Dim newPoint As Point
        Dim X, Y, Z As Decimal
        Dim pointName As String

        line = sr.ReadLine()
        While line IsNot Nothing
            pointInfo = line.Split(delim)

            ' get the name and coordinates of a point
            pointName = pointInfo(0)
            X = Convert.ToDecimal(pointInfo(1), CultureInfo.InvariantCulture)
            Y = Convert.ToDecimal(pointInfo(2), CultureInfo.InvariantCulture)
            Z = Convert.ToDecimal(pointInfo(3), CultureInfo.InvariantCulture)

            ' create and rename a point
            point = New Point3d(X, Y, Z)
            newPoint = workPart.Points.CreatePoint(point)
            newPoint.SetName(pointName)
            newPoint.SetVisibility(SmartObject.VisibilityOption.Visible)

            line = sr.ReadLine()
        End While

        sr.Close()

    Catch E As Exception
        Console.WriteLine("The file could not be read:")
        Console.WriteLine(E.Message)

    End Try

    ' updating the Part Navigator so the points will be seen
    Dim markId1 As NXOpen.Session.UndoMarkId = Nothing
    theSession.UpdateManager.UpdateModel(workPart, markId1)

End Sub
End Module