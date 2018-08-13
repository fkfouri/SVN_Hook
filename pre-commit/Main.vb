Imports System.Text
Imports SvnPreCommitHooks.Common
Imports SvnPreCommitHooks.SvnController

Module Main

    ''' <summary>
    ''' http://www.troyhunt.com/2010/02/creating-subversion-pre-commit-hooks-in.html
    ''' </summary>
    Sub Main()

        ''Obtem a lista de arquivos/caminhos modificados
        'Dim filesTest() As String = {"script.js", "style.css"}
        'Dim test As New Ignore_Extensions("C:\Inetpub\CockpitDesk\ApoioCockpit\SVN\SVN_Hooks\SvnPreCommitHooks", "", filesTest, "fkfouri1")




        Dim args() As String = Environment.GetCommandLineArgs
        Dim repos As String = ""
        Dim txn As String = ""
        Try
            repos = args(1)
            txn = args(2)
        Catch ex As Exception

        End Try


        Dim av As New AuthorValidation
        Dim authorType As AuthorTypes = av.CheckAuthorType(repos, txn)

        'Obtem a lista de arquivos/caminhos modificados
        Dim filesCollection() As String = GetChangedPaths(repos, txn)

        'Verifica o tamanho do comentario inserido
        Dim cv As New CommentsValidation(repos, txn)

        'Verifica se esta ocorrendo a exclusao de pastas nao desejadas
        Dim pv As New Ignore_folders(repos, txn, filesCollection)

        'Verifica Extensao dos arquivos
        Dim fvAllUser As New Ignore_files_admin(repos, txn, filesCollection)

        'Basicamente tres caminhos para percorrer conforme author
        Select Case authorType
            Case AuthorTypes.Limited 'Restrição no Pre-Commit por login
                'O aviso de erro sera dado pelo AuthorValidation

            Case AuthorTypes.User 'Usuarios NAO admin mas com permissao de commit
                'Verifica Extensao dos arquivos
                Dim fv As New Ignore_Extensions(repos, txn, filesCollection, av.Author)

                'Verifica o conteudo do que esta sendo submetido
                Dim contentV As New ContentValidation(repos, txn, filesCollection)

            Case AuthorTypes.Administrator 'Em teoria tem mais folga para commits

        End Select

        Environment.Exit(0)

    End Sub

End Module
