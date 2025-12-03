Public Class clsGridManager

    Private _GRD As DataGridView
    Private _DECIMAL_PLACES As Integer = -1
    Private _ROW_HEADERS() As String

    Public Sub New(ByRef GRID As DataGridView, ByVal DECIMAL_PLACES As Integer)
        _GRD = GRID
        _DECIMAL_PLACES = DECIMAL_PLACES

        DoubleBufferGrid()
    End Sub

    Public Enum enmCopyOptions
        WithHeaders
        WithOutHeaders
        SelectedCellsOnly
    End Enum

    Public ReadOnly Property Grid As DataGridView
        Get
            Return _GRD
        End Get
    End Property

    Public ReadOnly Property DataSource(Optional ByVal AsACopy As Boolean = True) As DataTable
        Get
            If DirectCast(_GRD.DataSource, DataTable) Is Nothing Then
                Return Nothing
            Else
                If AsACopy Then
                    Return DirectCast(_GRD.DataSource, DataTable).Copy
                Else
                    Return DirectCast(_GRD.DataSource, DataTable)
                End If
            End If
        End Get
    End Property

    Public Property DecimalPlaces() As Integer
        Get
            Return _DECIMAL_PLACES
        End Get
        Set(value As Integer)
            _DECIMAL_PLACES = value
            GridDecimalFormat()
        End Set
    End Property

    Public Property RowHeaders() As String()
        Get
            Return _ROW_HEADERS
        End Get
        Set(value As String())
            _ROW_HEADERS = value
        End Set
    End Property

    Public ReadOnly Property ColumnHeaders() As String()
        Get
            If _GRD Is Nothing Then
                Return {}
            End If
            If _GRD.Columns.Count = 0 Then
                Return {}
            End If

            Dim ARR(_GRD.Columns.Count - 1) As String
            For i As Integer = 0 To _GRD.Columns.Count - 1
                ARR(i) = _GRD.Columns(i).Name
            Next

            Return ARR
        End Get
    End Property

    Public Sub SetDatatable(ByVal DT As DataTable, Optional ByVal ROW_HEADERS() As String = Nothing)

        Dim COL_SCROLL As Integer = _GRD.HorizontalScrollingOffset
        Dim ROW_SCROLL As Integer = _GRD.FirstDisplayedScrollingRowIndex
        'Dim HOR_OFF = _GRD.FirstDisplayedScrollingColumnIndex

        ' set datasource
        _GRD.DataSource = DT

        ' do this before setting decimal places
        _ROW_HEADERS = ROW_HEADERS

        ' do this before actually applying row headers
        GridDecimalFormat()

        ' now...handle row headers
        ApplyRowHeaders()

        ' resize all the columns
        ResizeColumns()

        'redraw
        '_GRD.Invalidate()
        _GRD.Refresh()


        ' write to file
        ' paint colors

        If COL_SCROLL > -1 Then
            _GRD.HorizontalScrollingOffset = COL_SCROLL
        End If

        If ROW_SCROLL > -1 Then
            _GRD.FirstDisplayedScrollingRowIndex = ROW_SCROLL
        End If


        '_GRD.AutoScrollOffset = ROW_SCROLL


    End Sub

    Public Sub ApplyRowHeaders()
        If _ROW_HEADERS IsNot Nothing Then

            For i As Integer = 0 To _GRD.Rows.Count - 1
                If i <= _ROW_HEADERS.Length - 1 Then
                    _GRD.Rows(i).HeaderCell.Value = _ROW_HEADERS(i)
                End If
            Next
            '_GRD.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
        End If
    End Sub

    Public Sub ResizeColumns()
        '_GRD.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
        _GRD.AutoResizeColumns()
    End Sub

    Public Sub GridDecimalFormat()
        If _GRD.ColumnCount = 0 Then
            Exit Sub
        End If




        'Array.Sort(_ROW_HEADERS)
        'If IsNumeric(_ROW_HEADERS(_ROW_HEADERS.Length - 1)) Then
        '    If _ROW_HEADERS(_ROW_HEADERS.Length - 1) <= 3.0 Then
        '        'Gen5: MAP/BARO style
        '        _DECIMAL_PLACES = 2
        '    Else
        '        'Gen4: MAP style

        '    End If
        'End If

        Dim FORMAT_STRING As String = ""

        If _DECIMAL_PLACES < 0 Then
            FORMAT_STRING = "" 'no format/raw data
        ElseIf _DECIMAL_PLACES = 0 Then
            'FORMAT_STRING = "#" ' if the number is zero, this will suppress it from displaying (make it appear blank)
            FORMAT_STRING = "0" ' this will display one digit at least (so a zero will be zero, not blank)
        ElseIf _DECIMAL_PLACES > 0 Then
            FORMAT_STRING = "0." & FORMAT_STRING.PadRight(_DECIMAL_PLACES, "0"c)
        End If

        ' don't exit early...when changing a grid's data source with more columns
        ' this will cause the new ones to not get formatted
        'If _GRD.Columns(0).DefaultCellStyle.Format = FORMAT_STRING Then
        '    Exit Sub
        'End If

        ' SPEED UP APPLYING DECIMALS!!!
        ' remove all data, but clone the columns so the format can be applied, then re-apply the datasource
        Dim DT As DataTable = DirectCast(_GRD.DataSource, DataTable)
        If DT Is Nothing Then
            Exit Sub
        End If
        _GRD.DataSource = Nothing
        _GRD.DataSource = DT.Clone
        For i As Integer = 0 To _GRD.ColumnCount - 1
            _GRD.Columns(i).DefaultCellStyle.Format = FORMAT_STRING
        Next
        _GRD.DataSource = DT
    End Sub

    Public Sub PreventGridSort()
        For Each COL As DataGridViewColumn In _GRD.Columns
            COL.SortMode = DataGridViewColumnSortMode.NotSortable
        Next
    End Sub

    Public Sub DoubleBufferGrid()
        ' enable doublebuffering of datagrids. without this, painting with zones is super slow!
        ' https://stackoverflow.com/questions/118528/horrible-redraw-performance-of-the-datagridview-on-one-of-my-two-screens

        _GRD.GetType.InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.SetProperty, Nothing, _GRD, New Object() {True})
    End Sub

    Public Sub CopyGridToClipboard(ByVal CopyOptions As enmCopyOptions)

        If _GRD.RowCount = 0 Or _GRD.ColumnCount = 0 Then
            Exit Sub
        End If

        If CopyOptions = enmCopyOptions.SelectedCellsOnly Then

            If _GRD.GetCellCount(DataGridViewElementStates.Selected) = 0 Then
                Exit Sub
            End If
            _GRD.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText

        ElseIf CopyOptions = enmCopyOptions.WithHeaders Then
            _GRD.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            _GRD.SelectAll()
        ElseIf CopyOptions = enmCopyOptions.WithOutHeaders Then
            _GRD.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
            _GRD.SelectAll()
        End If

        Try
            Clipboard.SetDataObject(_GRD.GetClipboardContent)
        Catch ex As System.Runtime.InteropServices.ExternalException
            Throw New Exception("The Clipboard could not be accessed. Please try again.")
        End Try
    End Sub

    Public Sub DeleteRow(ByVal ROW_NUM As Integer)
        Dim DT As DataTable = DataSource(False)
        If DT Is Nothing Then
            Exit Sub
        End If

        DT.Rows.RemoveAt(ROW_NUM)
    End Sub

    Public Sub AddRow(ByVal ROW As DataRow)
        Dim DT As DataTable = DataSource(False)
        If DT Is Nothing Then
            Exit Sub
        End If

        DT.ImportRow(ROW)
    End Sub

    Public Sub UpdateRow(ByVal ROW_NUM As Integer, ByVal ROW As DataRow)
        Dim DT As DataTable = DataSource(False)
        If DT Is Nothing Then
            Exit Sub
        End If

        If DT.Rows(ROW_NUM).ItemArray.Length <> ROW.ItemArray.Length Then
            Exit Sub
        End If

        For i As Integer = 0 To DT.Rows(ROW_NUM).ItemArray.Length - 1
            If IsNumeric(ROW(i)) Then
                DT.Rows(ROW_NUM).Item(i) = ROW(i)
            End If
        Next

        If _ROW_HEADERS Is Nothing Then
            Exit Sub
        End If

        If _ROW_HEADERS.Length = DT.Rows.Count Then
            If InStr(_ROW_HEADERS(ROW_NUM), "*") = 0 Then
                _ROW_HEADERS(ROW_NUM) = _ROW_HEADERS(ROW_NUM) & "*"
            End If

        End If


    End Sub



End Class
