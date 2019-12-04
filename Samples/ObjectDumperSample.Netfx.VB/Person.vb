Namespace ObjectDumperSample.Netfx.VB
    Public Class Person

        Public Sub New()

            Me.GetOnly = 11
            Me.SetOnly = 99
            Me.ByteArray = New Byte[] { 1, 2, 3, 4 }
 End Sub

        Public Name As String

        Public Property [char] As Char

        Public Property Age As Integer

        Public Property SetOnly As Integer

        Public Property GetOnly As Integer

        Private Property [Private] As Integer

        Public Property [Bool] As Boolean

        Public Property [Byte] e As Byte

        Public Property ByteArray As Byte()

        Public Property [SByte] As SByte

        Public Property [Float] As Decimal

        Public Property [Uint] As UInteger

        Public Property [Long] As Long

        Public Property [ULong] As ULong

        Public Property [Short] As Short

        Public Property [UShort] As UShort

        Public Property [Decimal] As Decimal

        Public Property [Double] As Double

        Public Property [DateTime] As DateTime

        Public Property NullableDateTime As DateTime?

        Public Property [Enum] As DateTimeKind
    End Class
End Namespace
