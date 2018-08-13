Imports System.Text
Imports System.IO
Imports SvnPreCommitHooks.Common
Imports System.Text.RegularExpressions

''' <summary>
''' Verifica a inclusao de Arquivos cuja extensoes nao sao desejadas
''' </summary>
Friend Class Ignore_Extensions

    ''' <summary>
    ''' Verifica a inclusao de Arquivos cuja extensoes nao sao desejadas
    ''' </summary>
    Public Sub New(ByVal repos As String,
                   ByVal txn As String,
                   ByVal filesCollection() As String,
                   ByVal author As String)

        Dim ErrorsCollection As New ArrayList


        Dim ignoredExtensions As Object() = ReadFile(repos, "_ignore-extensions.txt")
        Dim specialReleased As Object() = ReadFileJson(repos, "_released-files-by-users.json", author.ToLower.Trim)


        For Each type In ignoredExtensions
            Dim ignoredType As String = type
            'Identifica se a extensao esta na lista de ignorados
            If Array.Exists(filesCollection, Function(x) Regex.IsMatch(x, ignoredType)) Then

                'Identifica se o usuario possi autorizacao especial para este tipo de arquivo
                If Not Array.Exists(specialReleased, Function(x) ignoredType.ToString.ToLower.Contains(x.ToString.ToLower.Trim)) Then
                    ErrorsCollection.Add(ignoredType.Replace("^[^D].*\.", "").Replace("$", "").ToUpper)
                End If

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
