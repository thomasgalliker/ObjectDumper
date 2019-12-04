Imports ObjectDumperSample.Netfx.VB.ObjectDumperSample.Netfx.VB

Module Program

    Sub Main()
        Dim [dictionaryInt32String] = new Dictionary(Of Int32, String) from {
                                                                           { 1, "Value1" },
                                                                           { 2, "Value2" },
                                                                           { 3, "Value3" }
                                                                       }

        Dim personsDump = ObjectDumper.Dump([dictionaryInt32String], DumpStyle.VisualBasic)

        Console.WriteLine(personsDump)
        Console.ReadLine()
    End Sub

    


End Module
