Imports System.Text

''' <summary>
''' Reponsavel por gerenciar a correcao de LOG do SVN
''' </summary>
Module Main
    Sub Main()
        Dim args() As String = Environment.GetCommandLineArgs
        Console.OutputEncoding = Encoding.GetEncoding(1252)

        Dim REPOS = "", REV = "", USER = "", PROPNAME = "", ACTION = "", AUTHOR = "", originalText = "", commitDate As String = ""
        Dim logChangeDate As Date = Now

        Try
            REPOS = args(1)
            REV = args(2)
            USER = args(3)
            PROPNAME = args(4)
            ACTION = args(5)
            AUTHOR = getSvnLook("author", REPOS, REV)
            originalText = getSvnLook("log", REPOS, REV)
            commitDate = getSvnLook("date", REPOS, REV)

            If IsDate(Left(commitDate, 19)) Then
                logChangeDate = CDate(Left(commitDate, 19))
            End If
        Catch ex As Exception

        End Try

        Dim diff As Double = DateDiff(DateInterval.Hour, logChangeDate, Now)

        'RegisterTrace(String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", REPOS, REV, USER, PROPNAME, ACTION, AUTHOR, originalText, dt, dtRef))

        If ACTION = "M" AndAlso PROPNAME = "svn:log" AndAlso AUTHOR = USER Then
            If diff < 3 Then
                Environment.Exit(0)
            Else
                Console.Error.WriteLine("Allow the author to change own svn:log until 3 hours after original commit")
                Environment.Exit(1)
            End If

        ElseIf ACTION = "M" AndAlso PROPNAME = "svn:log" AndAlso AUTHOR <> USER Then

            Console.Error.WriteLine("Only the author '" & AUTHOR & "' is allowed to change this log.")
            Environment.Exit(1)
        Else
            Console.Error.WriteLine("Changing revision properties other than svn:log is prohibited")
            Environment.Exit(1)
        End If

    End Sub

    Function getSvnLook(what As String, repos As String, rev As String) As String
        Dim p As New Process
        Dim out As String = ""
        Dim encoding As Text.Encoding = Encoding.GetEncoding(1252)
        With p.StartInfo
            .FileName = "svnlook.exe"
            .Arguments = [String].Format("{0} {1} -r {2}", what, repos, rev)
            .CreateNoWindow = True
            .RedirectStandardOutput = True
            .RedirectStandardError = True
            .UseShellExecute = False
            .StandardOutputEncoding = encoding
            .StandardErrorEncoding = encoding


            p.Start()
            out = p.StandardOutput.ReadToEnd()

            p.WaitForExit()
        End With
        Return out.Trim.Replace(Chr(13), "").Replace(Chr(10), "")
    End Function


    ''' <summary>
    ''' Registra o Trace, escreve um arquivo TXT para saber ate onde o codigo rodou no SVN Server
    ''' </summary>
    Friend Sub RegisterTrace(ByVal ref As String)
        Try
            Dim oEscrever As System.IO.StreamWriter

            Dim caminho As String = "D:\Repositories\Cockpit\hooks\Teste.txt"
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

End Module
