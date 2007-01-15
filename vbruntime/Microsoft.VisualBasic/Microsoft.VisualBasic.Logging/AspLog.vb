'
' AspLog.vb
'
' Authors:
'   Rolf Bjarne Kvinge (RKvinge@novell.com>
'
' Copyright (C) 2007 Novell (http://www.novell.com)
'
' Permission is hereby granted, free of charge, to any person obtaining
' a copy of this software and associated documentation files (the
' "Software"), to deal in the Software without restriction, including
' without limitation the rights to use, copy, modify, merge, publish,
' distribute, sublicense, and/or sell copies of the Software, and to
' permit persons to whom the Software is furnished to do so, subject to
' the following conditions:
' 
' The above copyright notice and this permission notice shall be
' included in all copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
' EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
' MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
' NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
' LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
' OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
' WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
'
#If NET_2_0 Then

Namespace Microsoft.VisualBasic.Logging
    Public Class AspLog
        Inherits Log

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal name As String)
            MyBase.New(name)
        End Sub

        Protected Friend Overrides Sub InitializeWithDefaultsSinceNoConfigExists()
#If mono_not_yet Then
            TraceSource.Listeners.Add(New System.Web.WebPageTraceListener())
            TraceSource.Switch.Level = SourceLevels.Information
#Else
            throw new NotImplementedException
#End If
        End Sub
    End Class
End Namespace
#End If