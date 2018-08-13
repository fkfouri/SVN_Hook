Imports SvnPreCommitHooks.SvnController
Imports System.Text.RegularExpressions

''' <summary>
''' Identifica o conteudo dos arquivos commitados para identifcar trecho indevido.
''' </summary>
Friend Class ContentValidation

    ''' <summary>
    ''' Identifica o conteudo dos arquivos commitados para identifcar trecho indevido.
    ''' </summary>
    Friend Sub New(ByVal repos As String,
                   ByVal txn As String,
                   ByVal filesCollection() As String)

        Dim ErrorsCollection As New ArrayList
        Dim ErrosType As New ArrayList

        For i As Integer = 0 To filesCollection.Length - 1

            Dim file As String = filesCollection(i)
            Try
                file = Mid(file, 5, file.Length - 1)
                Dim content As String = GetSvnLookOutput(repos, txn, "cat", file)

                Select Case True
                    Case content.ToLower.Contains("datareader")
                        'Evita o uso de datareader, que pode detonar o Oracle
                        ErrorsCollection.Add(file)
                        ErrosType.Add("DataReader")

                        ''==> Remoção da trava do IFRAME -> para atender demandas do FELIPE MATHEUS
                    'Case content.ToLower.Contains("<iframe")
                    '    'Evita o uso de Iframe
                    '    ErrorsCollection.Add(file)
                    '    ErrosType.Add("iframe")

                    Case Regex.IsMatch(content, "\b(?ix)data:image\b")
                        'evita a incorporacao de imagens no corpo do HTML,CSS
                        ErrorsCollection.Add(file)
                        ErrosType.Add("data:image - " & Regex.Matches(content, "\b(?ix)data:image\b").Count & " vezes")

                    Case Regex.IsMatch(content, "\b(?ix)thread.sleep\b") AndAlso repos.ToLower = "c:\repositories\cockpit"
                        'Evita o uso de Threading no Cockpit
                        ErrorsCollection.Add(file)
                        ErrosType.Add("thread.sleep - " & Regex.Matches(content, "\b(?ix)thread.sleep\b").Count & " vezes")

                    Case Regex.IsMatch(content, "\b(?ix)GOTO\b")
                        '\b     - inicio e fim da palavra
                        '(?ix)  - 'i'gnora Case sentitive
                        '       - 'x' ignora Espaco em branco. Acho que pode ser dispensavel
                        'Evita o uso de GoTo
                        ErrorsCollection.Add(file)
                        ErrosType.Add("GoTo - " & Regex.Matches(content, "\b(?ix)GOTO\b").Count & " vezes")

                End Select


                'Common.RegisterTrace(content)
            Catch ex As Exception

            End Try
        Next

        If ErrorsCollection.Count > 0 Then
            Dim msg As String = <msg>
=============================================================== 
Foi identificado o uso de um código que pode causar perda de 
performance do Cockpit nos seguintes arquivos:
{0}
Favor entrar em contato com o Fábio Kfouri ou Willian Henrique 
Barbosa para esclarecimentos.

Obrigado!
Cockpit Team 
===============================================================      
                                            </msg>

            For i As Integer = 0 To ErrorsCollection.Count - 1
                msg = String.Format(msg, "#" & (i + 1) & " - " & ErrorsCollection(i) & " - Uso de " & ErrosType(i) & "." & vbNewLine & "{0}")
            Next
            msg = String.Format(msg, "")

            'Console.OutputEncoding = Text.Encoding.GetEncoding(1252)
            Console.OutputEncoding = Text.Encoding.GetEncoding("ISO-8859-1")
            Console.Error.WriteLine(msg)
            Environment.Exit(1)
        End If


    End Sub

End Class
