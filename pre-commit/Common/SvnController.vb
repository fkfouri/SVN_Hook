Imports System.Text

Friend Class SvnController

    ''' <summary>
    ''' Executa codigo SVN no prompt
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetSvnLookOutput(ByVal repos As String,
                                            ByVal txn As String,
                                            ByVal subcommand As String,
                                            Optional ByVal file As String = "") As String

        Dim out As String = Nothing

        If repos.Length > 0 AndAlso txn.Length > 0 Then

            Dim p As New Process
        Dim encoding As Text.Encoding = Encoding.GetEncoding(1252)

            Try
                'Dim cmd As String = Environment.GetFolderPath(Environment.SpecialFolder.System) & "\cmd.exe"
                With p.StartInfo
                    .FileName = "svnlook.exe"
                    Select Case subcommand.ToLower
                        Case "cat"
                            .Arguments = [String].Format("{0} -t ""{1}"" ""{2}"" ""{3}""", subcommand, txn, repos, file)
                            'val.RegisterTrace(.Arguments)
                            '= New IO.StreamWriter(caminho, True,  System.Text.Encoding.GetEncoding("ISO-8859-1"))
                            'Case "changed"
                            '    .Arguments = [String].Format("{0} -t ""{1}"" ""{2}"" > ""{2}\hooks\pre-commit.txt"" ", subcommand, txn, repos)
                        Case Else
                            .Arguments = [String].Format("{0} -t ""{1}"" ""{2}""", subcommand, txn, repos)
                    End Select

                    .CreateNoWindow = True
                    .RedirectStandardOutput = True
                    .RedirectStandardError = True
                    .UseShellExecute = False
                    .StandardOutputEncoding = encoding
                    .StandardErrorEncoding = encoding


                End With

                p.Start()
                out = p.StandardOutput.ReadToEnd()

                p.WaitForExit()

            Catch ex As Exception
                'RegisterTrace(ex.Message)
                Debug.WriteLine(ex.ToString())
            Finally
                If p IsNot Nothing Then
                    p.Dispose()
                    p = Nothing
                End If
            End Try

        End If
        Return out
    End Function

    ''' <summary>
    ''' Obtem a lista de arquivos/caminhos modificados
    ''' </summary>
    Public Shared Function GetChangedPaths(ByVal repos As String,
                                           ByVal txn As String) As String()
        Dim changedPaths As String = GetSvnLookOutput(repos, txn, "changed")
        Return changedPaths.Replace(Chr(10), "").Split(Chr(13))
    End Function

End Class
