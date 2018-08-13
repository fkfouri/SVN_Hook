Imports SvnPreCommitHooks.SvnController

''' <summary>
''' Identifica e classifica o Author do Commit
''' </summary>
Friend Class AuthorValidation

    ''' <summary>
    ''' Retorna o Author autenticado no SVN
    ''' </summary>
    Public ReadOnly Property Author As String
        Get
            Return _author
        End Get
    End Property
    Private _author As String = Nothing


    ''' <summary>
    ''' Mensagem/Comentários do Commit - Identificacao do tamanho do texto
    ''' </summary>
    Friend Function CheckAuthorType(ByVal repos As String,
                                    ByVal txn As String) As AuthorTypes

        _author = GetSvnLookOutput(repos, txn, "author")
        Dim authorType As AuthorTypes = CheckAuthor(Author)

        Select Case CheckAuthor(Author)
            Case AuthorTypes.Administrator, AuthorTypes.User
                Return authorType
            Case AuthorTypes.NotDefined
                Dim msg As String = <msg>
=============================================================== 
O SVN está configurado para bloquear seu commit pois não
foi identificado uma utilização válida do SVN

Por favor, procure pelo Fábio Kfouri ou Willian Henrique Barbosa 
para esclarecimentos e alinhamento.

Obrigado!
Cockpit Team 
===============================================================                                     
                                </msg>

                'Console.OutputEncoding = Text.Encoding.GetEncoding(1252)
                Console.OutputEncoding = Text.Encoding.GetEncoding("ISO-8859-1")
                Console.Error.WriteLine(msg)
                Environment.Exit(1)
                Return authorType

            Case Else
                Dim msg As String = <msg>
=============================================================== 
O SVN está configurado para bloquear seu commit pois precisamos 
alinhar alguns requisitos de desenvolvimento.

Por favor, procure pelo Fábio Kfouri ou Willian Henrique Barbosa 
para esclarecimentos e alinhamento.

Obrigado!
Cockpit Team 
===============================================================                                     
                                </msg>

                'Console.OutputEncoding = Text.Encoding.GetEncoding(1252)
                Console.OutputEncoding = Text.Encoding.GetEncoding("ISO-8859-1")
                Console.Error.WriteLine(msg)
                Environment.Exit(1)
                Return authorType
        End Select
    End Function

    ''' <summary>
    ''' Valida a mensagem/Comentário do commit
    ''' </summary>
    Private Function CheckAuthor(ByVal author As String) As AuthorTypes
        If author IsNot Nothing Then
            author = Trim(LCase(author.Replace(Chr(10), "").Replace(Chr(13), "")))
        End If

        Select Case author
            Case Nothing
                Return AuthorTypes.NotDefined
            Case "fkfouri", "whrocha", "laucosta"
                Return AuthorTypes.Administrator
            Case "" 'colocar o login de quem precisa ser ignorado
                Return AuthorTypes.Limited
            Case Else
                Return AuthorTypes.User
        End Select
    End Function

End Class

Friend Enum AuthorTypes
    Administrator
    User
    Limited
    NotDefined
End Enum
