Imports SvnPreCommitHooks.SvnController

''' <summary>
''' Mensagem/Comentários do Commit - Identificacao do tamanho do texto
''' </summary>
Friend Class CommentsValidation

    ''' <summary>
    ''' Mensagem/Comentários do Commit - Identificacao do tamanho do texto
    ''' </summary>
    Friend Sub New(ByVal repos As String,
                                ByVal txn As String)

        Dim providedComments As String = GetSvnLookOutput(repos, txn, "log")

        If Not CheckComments(providedComments) Then
            Dim msg As String = <msg>
=============================================================== 
O SVN está configurado para bloquear o seu commit por nao haver 
um comentario adequado.

Por favor, adicione um comentario de log descrevendo as razoes 
que o fizeram modificar e/ou criar novos codigos. 
            
PREFERENCIALMENTE - Utilizar técnica de escrita de User Stories
            
------ /// ----------- /// ----------- /// ---------- /// ----- 
Ex.: Como  um [ator] eu quero/preciso/gostaria de [ação] para
     [funcionalidade].

ou

Ex.: Projeto desenvolvido para atender a solicitação do Joao 
com objetivo criar visibilidade de evolucao da pintura das 
aeronaves na cabine de pintura.
------ /// ----------- /// ----------- /// ---------- /// ----- 


Cockpit Team 
===============================================================                                     
                                </msg>

            'Console.OutputEncoding = Text.Encoding.GetEncoding(1252)
            Console.OutputEncoding = Text.Encoding.GetEncoding("ISO-8859-1")
            Console.Error.WriteLine(msg)
            Environment.Exit(1)
        End If

    End Sub

    ''' <summary>
    ''' Valida a mensagem/Comentário do commit
    ''' </summary>
    Private Function CheckComments(ByVal providedComments As String) As Boolean

        Select Case True
            Case providedComments.Length < 20
                Return False
            Case providedComments.Split(" "c).Length < 5
                Return False
            Case Else
                Return True
        End Select

    End Function

End Class
