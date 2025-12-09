Imports System.IO

Public Class clsZones

    Private READY As Boolean = True
    Private DT_RPM As DataTable = Nothing
    Private DT_MAP As DataTable = Nothing
    Private ARR_HISTO_MAP(-1) As Double

    Public Sub New(ByVal RPM_Boundaries As DataTable, ByVal MAP_Boundaries As DataTable)
        If RPM_Boundaries IsNot Nothing Then
            DT_RPM = RPM_Boundaries
        Else
            READY = False
            Exit Sub
        End If

        If MAP_Boundaries IsNot Nothing Then
            DT_MAP = MAP_Boundaries
        Else
            READY = False
            Exit Sub
        End If

        If RPM_Boundaries.Columns.Count <> 4 Then
            READY = False
            Exit Sub
        End If

        If RPM_Boundaries.Rows.Count <> 1 Then
            READY = False
            Exit Sub
        End If

        If MAP_Boundaries.Columns.Count <> 5 Then
            READY = False
            Exit Sub
        End If

        If MAP_Boundaries.Rows.Count <> 5 Then
            READY = False
            Exit Sub
        End If

    End Sub


    Public Function WhatZoneAmI(ByVal RPM As Double, ByVal MAP As Double) As Integer
        ' this function traverses the two tables that define the zones to determine
        ' what zone (0-29) the combination of RPM and MAP yields.

        If Not READY Then
            Return -1
        End If

        ' ----------------------------------------------------------------------------------
        ' STEP 1: will determine what generic set of RPM columns we are dealing with.
        '         the ZONE_MINIMUM is the min zone value for that set of columns.
        '         so this step may determine ZONE_MINIMUM=6, but we may increment
        '         this up to the max value for this column set of 11 once we index into the MAP table.
        ' ----------------------------------------------------------------------------------
        Dim ZONE_MINIMUM As Integer = 0 ' default
        For i As Integer = DT_RPM.Columns.Count - 1 To 0 Step -1
            If RPM >= CDbl(DT_RPM.Rows(0).Item(i)) Then

                ' this code....
                ZONE_MINIMUM = (i + 1) * 6
                Exit For

                ' replaces this code...
                'If i = 3 Then
                '    ZONE_MINIMUM = 24
                '    Exit For
                'ElseIf i = 2 Then
                '    ZONE_MINIMUM = 18
                '    Exit For
                'ElseIf i = 1 Then
                '    ZONE_MINIMUM = 12
                '    Exit For
                'ElseIf i = 0 Then
                '    ZONE_MINIMUM = 6
                '    Exit For
                'End If
            End If
        Next
        ' ----------------------------------------------------------------------------------




        ' ----------------------------------------------------------------------------------
        ' STEP 2: based on the RPM column set, we know what column index to search in the MAP table
        ' ----------------------------------------------------------------------------------

        ' this code...(no reason this line could not go above inside the exit condition of the FOR LOOP
        '              but we will list it here for future Dan's ability to read and support)
        Dim MAP_COL As Integer = ZONE_MINIMUM / 6

        ' replaces this code...
        'Dim MAP_COL As Integer = 0 ' default
        'If ZONE_MINIMUM = 6 Then
        '    MAP_COL = 1
        'ElseIf ZONE_MINIMUM = 12 Then
        '    MAP_COL = 2
        'ElseIf ZONE_MINIMUM = 18 Then
        '    MAP_COL = 3
        'ElseIf ZONE_MINIMUM = 24 Then
        '    MAP_COL = 4
        'End If
        ' ----------------------------------------------------------------------------------


        ' ----------------------------------------------------------------------------------
        ' STEP 3: find out the row of this table and add it to the ZONE_MINIMUM value
        ' ----------------------------------------------------------------------------------
        Dim MAP_ADDER As Integer = 0
        For i As Integer = DT_MAP.Rows.Count - 1 To 0 Step -1
            If MAP >= CDbl(DT_MAP.Rows(i).Item(MAP_COL)) Then
                MAP_ADDER = i + 1
                Exit For
            End If
        Next
        ' ----------------------------------------------------------------------------------


        'Console.WriteLine("RPM: " & RPM & "   MAP:" & MAP & "   ---> ZONE: " & ZONE_MINIMUM + MAP_ADDER)

        Return ZONE_MINIMUM + MAP_ADDER

    End Function



End Class
