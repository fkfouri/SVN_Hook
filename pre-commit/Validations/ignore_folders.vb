Imports System.Text
Imports System.IO
Imports SvnPreCommitHooks.Common

''' <summary>
''' Verifica a inclusao de Pastas nao desejadas
''' </summary>
Friend Class Ignore_folders

    ''' <summary>
    ''' Verifica a inclusao de Pastas nao desejadas
    ''' </summary>
    Public Sub New(ByVal repos As String,
                   ByVal txn As String,
                   ByVal filesCollection() As String)

        Dim ErrorsCollection As New ArrayList

        Dim ignoredTypes As Object() = ReadFile(repos, "_ignore-folders.txt")

        For i As Integer = 0 To filesCollection.Length - 1
            Dim file As String = filesCollection(i)

            If ignoredTypes.Length > 0 AndAlso
                ignoredTypes.Any(Function(folder) file.ToLower.Contains(folder.ToLower)) Then
                ErrorsCollection.Add(file)
            End If

        Next

        If ErrorsCollection.Count > 0 Then
            Dim msg As String = <msg>
=============================================================== 
O SVN está configurado para ignorar as seguintes pastas:
{0}
Estas pastas são ignoradas pois inflam denecessariamente 
o repositório do SVN.

Favor entrar em contato com o Fábio Kfouri ou o Willian Henrique,
caso tenha dúvidas.

Cockpit Team 
===============================================================      
                                            </msg>

            For i As Integer = 0 To ErrorsCollection.Count - 1
                msg = String.Format(msg, "#" & (i + 1) & " - " & ErrorsCollection(i) & vbNewLine & "{0}")
            Next
            msg = String.Format(msg, "")

            Console.OutputEncoding = Encoding.GetEncoding(1252)
            Console.Error.WriteLine(msg)
            Environment.Exit(1)
        End If


    End Sub

End Class
