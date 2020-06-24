Imports ObjectDumperSample.Netfx.VB.ObjectDumperSample.Netfx.VB

Module Program

    Sub Main()
        Dim persons = New List(Of Person) From {
            New Person With {.Name = "John", .Age = 20},
            New Person With {.Name = "Thomas", .Age = 30}
        }

        Dim personsDump = ObjectDumper.Dump(persons, DumpStyle.VisualBasic)

        Console.WriteLine(personsDump)
        Console.ReadLine()
    End Sub

End Module
