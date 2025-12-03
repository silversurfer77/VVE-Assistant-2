' Taken from this open source project and translated into VB.NET'
' https://www.codeproject.com/Articles/5293980/Graph3D-A-Windows-Forms-Render-Control-in-Csharp
' see also: https://csharp.agrimetsoft.com/exercises/3D_Chart_Graph
' https://www.dropbox.com/s/ji9vszolojnq210/3D%20Chart%20Graph.rar?dl=0

Imports Graph3D


Imports delRendererFunction = Graph3D.Plot3D.Graph3D.eRaster
Imports cPoint3D = Graph3D.Plot3D.Graph3D.cPoint3D
Imports eRaster = Graph3D.Plot3D.Graph3D.eRaster
Imports cScatter = Graph3D.Plot3D.Graph3D.cScatter
Imports eNormalize = Graph3D.Plot3D.Graph3D.eNormalize
Imports eSchema = Graph3D.Plot3D.ColorSchema.eSchema



Public Class cls3D
    Dim graph3D1 As Plot3D.Graph3D


    Public Sub New(ByRef graph As Plot3D.Graph3D)
        graph3D1 = graph
    End Sub


    Public Sub Plot3D(ByVal values(,) As Integer, Optional ByVal c_Colors() As Color = Nothing, Optional ByVal ARR_Z_AXIS_MAX_MIN_SCALE() As Double = Nothing)
        SetSurface(values, ARR_Z_AXIS_MAX_MIN_SCALE)

        ' get rid of the default axis...the RPM values are wrong...where are they being set in the first place?
        'graph3D1.Raster = CType(3, eRaster)
        If c_Colors Is Nothing Then
            c_Colors = Graph3D.Plot3D.ColorSchema.GetSchema(CType(My.Settings.GraphColor, eSchema)) 'user defined
        End If
        graph3D1.SetColorScheme(c_Colors, 3)




    End Sub
    Private Sub SetSurface(ByVal s32_Values As Integer(,), Optional ByVal ARR_Z_AXIS_MAX_MIN_SCALE() As Double = Nothing)
        Dim i_Points3D As cPoint3D(,) = New cPoint3D(s32_Values.GetLength(0) - 1, s32_Values.GetLength(1) - 1) {}

        For X As Integer = 0 To s32_Values.GetLength(0) - 1
            For Y As Integer = 0 To s32_Values.GetLength(1) - 1
                i_Points3D(X, Y) = New cPoint3D(X * 10, Y * 500, s32_Values(X, Y))
            Next
        Next

        graph3D1.AxisX_Legend = "RPM"
        graph3D1.AxisY_Legend = "MAP"
        graph3D1.AxisZ_Legend = "VVE"
        graph3D1.SetSurfacePoints(i_Points3D, eNormalize.Separate, ARR_Z_AXIS_MAX_MIN_SCALE)
    End Sub

End Class