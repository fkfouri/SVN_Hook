Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Friend Class Common

    ''' <summary>
    ''' Registra o Trace, escreve um arquivo TXT para saber ate onde o codigo rodou no SVN Server
    ''' </summary>
    Friend Shared Sub RegisterTrace(ByVal ref As String)
        Try
            Dim oEscrever As System.IO.StreamWriter

            Dim caminho As String = "d:\Repositories\Teste.txt"
            Dim msg As String = ref

            If Not IO.File.Exists(caminho) Then
                oEscrever = IO.File.CreateText(caminho)
            Else
                oEscrever = New IO.StreamWriter(caminho, True, System.Text.Encoding.GetEncoding("ISO-8859-1"))
            End If

            oEscrever.WriteLine(msg)
            oEscrever.Flush()
            oEscrever.Close()

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Leitura do Arquivo de restricoes
    ''' </summary>
    Friend Shared Function ReadFile(repos As String,
                                    file As String) As Array
        Dim out As New ArrayList
        Dim fi As New FileInfo(repos & "\hooks\" & file)
        'Dim fi As New FileInfo("C:\Inetpub\CockpitDesk\ApoioCockpit\SVN_Hooks\ignore-folders.txt")

        If fi.Exists Then
            Dim arquivo As New IO.FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            Dim Leitura As New StreamReader(arquivo, System.Text.Encoding.GetEncoding("ISO-8859-1"))

            Do While Not Leitura.EndOfStream
                Dim Linha As String = Leitura.ReadLine()
                out.Add(Linha)
            Loop

            Leitura.Close()
            Leitura.Dispose()

        End If
        Return out.ToArray
    End Function


    ''' <summary>
    ''' Leitura do Arquivo de restricoes
    ''' </summary>
    Friend Shared Function ReadFileJson(repos As String,
                                        file As String,
                                        author As String) As Object()
        Dim contentJson As String = ""
        Dim fi As New FileInfo(repos & "\hooks\" & file)
        Dim out As New ArrayList

        'Dim fi As New FileInfo("C:\Inetpub\CockpitDesk\ApoioCockpit\SVN_Hooks\ignore-folders.txt")

        If fi.Exists Then
            Dim arquivo As New IO.FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            Dim Leitura As New StreamReader(arquivo, System.Text.Encoding.GetEncoding("ISO-8859-1"))

            contentJson = Leitura.ReadToEnd

            Leitura.Close()
            Leitura.Dispose()

            Try
                Dim json As JObject = JObject.Parse(contentJson)
                For Each temp In json("config")

                    Console.Error.WriteLine("Entrei no laco: " & temp("user").ToString)

                    If temp("user").ToString.ToLower.Trim = author.Trim Then
                        For Each ext1 As String In temp("extensions")
                            out.Add(ext1)
                        Next
                        Exit For
                    End If
                Next

                'Dim ext As Object = From j In json("config")
                '                    Where j("user") = user
                '                    Select j

            Catch ex As Exception
            End Try
        End If

        Return out.ToArray
    End Function

#Region "Comandos auxiliares usados durante o desenvolvimento para identificar o trace do codigo"

    Private Function GetFileNameErrors(ByVal changedPaths As String) As String
        Dim changeRows = Text.RegularExpressions.Regex.Split(changedPaths.TrimEnd(), Environment.NewLine)
        For Each changeRow As Object In changeRows
            Dim changeType = changeRow(0)
            Dim filePath = changeRow.Substring(4, changeRow.Length - 4)
            Dim fileName = IO.Path.GetFileName(filePath)
            If changeType <> "D"c AndAlso fileName = "Thumbs.db" Then
                Return "Thumbs.db file was found."
            End If
        Next
        Return Nothing
    End Function

#End Region

End Class
