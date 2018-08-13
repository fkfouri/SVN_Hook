Imports System.Text
Imports System.IO
Imports SvnPreCommitHooks.Common
Imports System.Text.RegularExpressions

''' <summary>
''' Verifica a inclusao de Arquivos cuja extensoes nao sao desejadas, será negado mesmo que seja uma inclusao de um Admin
''' </summary>
Friend Class Ignore_files_admin

    ''' <summary>
    ''' Verifica a inclusao de Arquivos cuja extensoes nao sao desejadas
    ''' </summary>
    Public Sub New(ByVal repos As String,
                   ByVal txn As String,
                   ByVal filesCollection() As String)

        Dim ErrorsCollection As New ArrayList

        Dim ignoredTypes As Object() = ReadFile(repos, "_ignore-files-admin.txt")

        For Each type In ignoredTypes
            Dim ignoredType As String = type
            If Array.Exists(filesCollection, Function(x) Regex.IsMatch(x, ignoredType)) Then
                ErrorsCollection.Add(ignoredType.Replace("^[^D].*\.", "").Replace("$", "").ToUpper)
            End If
        Next

        If ErrorsCollection.Count > 0 Then
            Dim msg As String = <msg>
=============================================================== 
O SVN está configurado para ignorar as seguintes extensões de 
arquivo:
{0}
Estas extensões foram ignoradas pois inflam o repositório do SVN.

Para o versionamento destes tipos de arquivo, favor entrar em
contato com o Fábio Kfouri ou o Willian Henrique.

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
