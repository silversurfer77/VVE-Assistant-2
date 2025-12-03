

Public Class clsMulti_Datatable

    Dim DTs() As DataTable

    Public Sub New(Optional ByVal NEW_DT As DataTable = Nothing)
        If NEW_DT IsNot Nothing Then
            ReDim DTs(0)
            DTs(0) = NEW_DT
        End If
    End Sub

    Public Sub AppendNewDT(ByVal NEW_DT As DataTable)

        If DTs Is Nothing Then
            ReDim DTs(0)
            DTs(0) = NEW_DT
        Else

            Dim STR_ERR As String = ""
            If Verify_DTs_Match(NEW_DT, STR_ERR) Then
                ReDim Preserve DTs(DTs.Length)
                DTs(DTs.Length - 1) = NEW_DT
            Else
                Throw New Exception(STR_ERR)
            End If

        End If


    End Sub

    Public Sub RemoveLastDT()

        If DTs IsNot Nothing Then
            ReDim Preserve DTs(DTs.Length - 1)
        End If

    End Sub

    Public Function GetCurrentDT() As DataTable
        If DTs Is Nothing Then
            Return New DataTable
        End If

        Return DTs(DTs.Length - 1)
    End Function








    Public Function Compute_Average_DT() As DataTable
        If DTs Is Nothing Then
            Return Nothing
        End If
        If DTs.Length = 0 Then
            Return Nothing
        End If
        If DTs.Length = 1 Then
            Return DTs(0)
        End If

        Dim DT_AVE As DataTable = DTs(0).Clone

        For Each col As DataColumn In DT_AVE.Columns
            col.AllowDBNull = True
        Next

        Dim ARR_AVE(DTs(0).Columns.Count - 1) As Object
        Dim RUNNING_TOTAL As Decimal = 0.0
        Dim COUNT As Integer = 0
        Dim FOUND As Boolean = False
        Dim TEMP_OBJ As Object = Nothing

        For i As Integer = 0 To DTs(0).Rows.Count - 1

            ' this is slow...and not needed due to the IF FOUND statement
            'ReDim ARR_AVE(DT(0).Columns.Count - 1)

            For j As Integer = 0 To DTs(0).Columns.Count - 1
                FOUND = False
                COUNT = 0
                RUNNING_TOTAL = 0.0
                TEMP_OBJ = Nothing

                For k As Integer = 0 To DTs.Length - 1
                    TEMP_OBJ = Get_DT_Value(DTs(k), i, j)

                    If IsNumeric(TEMP_OBJ) Then
                        FOUND = True
                        COUNT += 1
                        RUNNING_TOTAL += TEMP_OBJ
                    End If
                Next

                If FOUND Then
                    ARR_AVE(j) = RUNNING_TOTAL / COUNT
                Else
                    ARR_AVE(j) = DBNull.Value
                End If
            Next

            DT_AVE.Rows.Add(ARR_AVE)
        Next


        Return DT_AVE
    End Function



    Private Function Get_DT_Value(ByVal DT_IN As DataTable, ByVal ROW As Integer, ByVal COL As Integer) As Object
        Try
            Return CDec(DT_IN.Rows(ROW).Item(COL))
        Catch ex As Exception
            Return DBNull.Value
        End Try
    End Function

    Private Function Verify_DTs_Match(ByVal DT_NEW As DataTable,
                                      ByRef ERR_REASON As String,
                                      Optional ByVal NEW_HEADERS() As String = Nothing) As Boolean

        ' Optional ByVal NEW_HEADERS() As String = Nothing
        ' Required: always pass in the new headers when pasting in a new table
        ' Optional: when doing a math operation or some manipulation on the data that already exists within the system



        Dim DT_OLD As DataTable = GetCurrentDT()
        If DT_OLD Is Nothing Then
            Return True
        End If

        If DT_NEW Is Nothing Then
            Return True
        End If

        ' ---------------------------------------------------------------------------------------------
        ' Check Y-Axis (MAP or PR)
        ' ---------------------------------------------------------------------------------------------
        If DT_OLD.Rows.Count < DT_NEW.Rows.Count Then
            ERR_REASON = "-> New table has MORE MAP ROWS (" & DT_NEW.Rows.Count & ") than the original Zone Definition (" & DT_OLD.Rows.Count & ")"
            Return False
        End If

        If DT_OLD.Rows.Count > DT_NEW.Rows.Count Then
            ERR_REASON = "-> New table has LESS MAP ROWS (" & DT_NEW.Rows.Count & ") than the original Zone Definition (" & DT_OLD.Rows.Count & ")"
            Return False
        End If


        If NEW_HEADERS IsNot Nothing Then
            Dim ROWHEADERS_ZONE() As String = clsLib.ReadArrayFromFile(clsLib.GetZoneFileName)

            If ROWHEADERS_ZONE.Length <> NEW_HEADERS.Length Then
                ERR_REASON = "-> Missing MAP breakpoints, did you Copy With Axis?"
                Return False
            End If

            For i As Integer = 0 To ROWHEADERS_ZONE.Length - 1
                If ROWHEADERS_ZONE(i) <> NEW_HEADERS(i) Then
                    ERR_REASON = "-> MAP BREAKPOINTS do not match the original Zone Definition. First breakpoint mismatch is Zone Definition: [" & ROWHEADERS_ZONE(i) & "] vs New Table: [" & NEW_HEADERS(i) & "]"
                    Return False
                End If
            Next
        End If
        ' ---------------------------------------------------------------------------------------------



        ' ---------------------------------------------------------------------------------------------
        ' Check X-Axis (RPM)
        ' ---------------------------------------------------------------------------------------------
        If DT_OLD.Columns.Count < DT_NEW.Columns.Count Then
            ERR_REASON = "-> New table has MORE RPM COLUMNS (" & DT_NEW.Columns.Count & ") than the original Zone Definition (" & DT_OLD.Columns.Count & ")"
            Return False
        End If

        If DT_OLD.Columns.Count > DT_NEW.Columns.Count Then
            ERR_REASON = "-> New table has LESS RPM COLUMNS (" & DT_NEW.Columns.Count & ") than the original Zone Definition (" & DT_OLD.Columns.Count & ")"
            Return False
        End If

        For i As Integer = 0 To DT_OLD.Columns.Count - 1
            If DT_OLD.Columns(i).ColumnName <> DT_NEW.Columns(i).ColumnName Then
                ERR_REASON = "-> RPM BREAKPOINTS do not match the original Zone Definition. First breakpoint mismatch is Zone Definition: [" & DT_OLD.Columns(i).ColumnName & "] vs New Table: [" & DT_NEW.Columns(i).ColumnName & "]"
                Return False
            End If
        Next
        ' ---------------------------------------------------------------------------------------------

        Return True
    End Function

End Class

